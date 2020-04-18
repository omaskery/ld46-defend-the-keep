using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(LineRenderer))]
public class AimGuide : MonoBehaviour
{
    private enum TargetingState
    {
        Inactive,
        Targeting,
    }
    
    private LineRenderer _lineRenderer;
    private float _targetingTimer;
    private TargetingState _state = TargetingState.Inactive;
    private Vector3 _targetPoint;
    private Vector3 _initialVelocity;
    
    [SerializeField] private LayerMask clickMask;
    [SerializeField] private float closestDistance;

    [Header("Configuration")] [SerializeField]
    private float closestHeight;

    [SerializeField] private Camera currentCamera;
    [SerializeField] private float furthestDistance;
    [SerializeField] private float furthestHeight;
    [SerializeField] private float maxRaycastDistance;

    [Header("References")] [SerializeField]
    private Transform origin;

    [SerializeField] private float targetingDuration;

    public event Action<Vector3, bool> FiringSolutionFound;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (_state == TargetingState.Inactive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttemptToStartTargeting();
            }
        }
        else if (_state == TargetingState.Targeting)
        {
            if (!Input.GetMouseButton(0))
            {
                FinishTargeting(true);
            }
            else
            {
                _targetingTimer += Time.deltaTime;

                if (_targetingTimer < targetingDuration)
                {
                    UpdateTargeting();
                }
                else
                {
                    FinishTargeting(false);
                }
            }
        }
    }

    private void UpdateTargeting()
    {
        var currentTargetPoint = CalculateCurrentTargetPoint();

        GuideToTarget(currentTargetPoint, out var points, out _initialVelocity);
        SetLineRenderer(points);
    }

    private void FinishTargeting(bool userSelected)
    {
        var target = CalculateCurrentTargetPoint();

        var condition = "";
        if (userSelected)
        {
            condition = "user accepted";
        }
        else
        {
            condition = "fallback/timeout";
        }

        var error = (_targetPoint - target).magnitude;
        Debug.Log($"{condition} firing solution: {_initialVelocity} (targetting: {target} [error: {error}])");
        FiringSolutionFound?.Invoke(_initialVelocity, userSelected);
            
        _state = TargetingState.Inactive;
        _lineRenderer.enabled = false;
    }

    private void AttemptToStartTargeting()
    {
        if (CheckForValidTarget(out var point))
        {
            _targetPoint = point;
            _targetingTimer = 0.0f;
            Debug.Log($"user clicked on point {_targetPoint}");

            UpdateTargeting();
            
            _state = TargetingState.Targeting;
        }
    }

    private Vector3 CalculateCurrentTargetPoint()
    {
        var targetingTimeRatio = _targetingTimer / targetingDuration;
        var originPosition = origin.position;
        var targetDirection = (_targetPoint - originPosition).normalized;
        var closestTargetPoint = originPosition + (targetDirection * closestDistance);
        var furthestTargetPoint = originPosition + (targetDirection * furthestDistance);
        var currentTargetPoint = Vector3.Lerp(closestTargetPoint, furthestTargetPoint, targetingTimeRatio);
        return currentTargetPoint;
    }

    private bool CheckForValidTarget(out Vector3 point)
    {
        var mouseRay = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out var hitInfo, maxRaycastDistance, clickMask))
        {
            if (hitInfo.collider.gameObject.GetComponent<Targetable>() != null)
            {
                point = hitInfo.point;
                return true;
            }
        }

        point = Vector3.zero;
        return false;
    }

    private void GuideToTarget(Vector3 target, out Vector3[] points, out Vector3 initialVelocity)
    {
        var originPosition = origin.position;

        var targetHorizontalPosition = new Vector3(target.x, 0, target.z);
        var originHorizontalPosition = new Vector3(originPosition.x, 0, originPosition.z);
        var horizontalDirection = (targetHorizontalPosition - originHorizontalPosition).normalized;
        var horizontalDistance = (targetHorizontalPosition - originHorizontalPosition).magnitude;

        initialVelocity = CalculateInitialVelocity(target, horizontalDistance, originPosition,
            horizontalDirection, out var totalTimeToTarget);

        points = ExtrapolateTrajectory(totalTimeToTarget, originPosition, initialVelocity, target);
    }

    private static Vector3[] ExtrapolateTrajectory(float totalTimeToTarget, Vector3 originPosition,
        Vector3 initialVelocity, Vector3 target)
    {
        var expectedTimeSteps = 100;
        var timeStep = totalTimeToTarget / expectedTimeSteps;
        var points = new List<Vector3>(expectedTimeSteps);
        var position = originPosition;
        var velocity = initialVelocity;
        var maxTimeSteps = (int) (expectedTimeSteps * 1.1f);
        
        points.Add(position);
        
        for (int i = 0; i < maxTimeSteps; i++)
        {
            position += velocity * timeStep;
            velocity += Physics.gravity * timeStep;

            // once we're past the expected time steps, we'll let it do a few more if it decreases the error
            if (i >= expectedTimeSteps)
            {
                var lastError = (points[points.Count - 1] - target).sqrMagnitude;
                var error = (position - target).sqrMagnitude;
                if (error > lastError)
                {
                    break;
                }
            }
            
            points.Add(position);
        }

        return points.ToArray();
    }

    private Vector3 CalculateInitialVelocity(Vector3 target, float horizontalDistance, Vector3 originPosition,
        Vector3 horizontalDirection, out float totalTimeToTarget)
    {
        var distanceRatio = (horizontalDistance - closestDistance) / (furthestDistance - closestDistance);
        var apogeeHeight = Mathf.Lerp(closestHeight, furthestHeight, distanceRatio);

        _lineRenderer.enabled = true;

        var heightFromOriginToApogee = apogeeHeight - originPosition.y;
        var initialUpwardVelocity = Mathf.Sqrt(-2.0f * heightFromOriginToApogee * Physics.gravity.y);
        var timeToApogee = -initialUpwardVelocity / Physics.gravity.y;
        var heightFromApogeeToTarget = target.y - apogeeHeight;
        var timeFromApogeeToTarget = Mathf.Sqrt(2.0f * heightFromApogeeToTarget / Physics.gravity.y);
        totalTimeToTarget = timeToApogee + timeFromApogeeToTarget;
        var horizontalSpeed = horizontalDistance / totalTimeToTarget;

        var initialVelocity = (horizontalDirection * horizontalSpeed) + (Vector3.up * initialUpwardVelocity);
        return initialVelocity;
    }

    private void SetLineRenderer(Vector3[] points)
    {
        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }
}