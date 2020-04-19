using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleAimGuide : MonoBehaviour, IReportFiringSolutions
{
    [Header("Configuration")]
    [SerializeField] private float maxRaycastDistance;
    [SerializeField] private float closestHeight;
    [SerializeField] private float closestDistance;
    [SerializeField] private float furthestHeight;
    [SerializeField] private float furthestDistance;
    [SerializeField] private float fireRate = 4.0f;
    [SerializeField] private float errorRadius = 1.0f;
    [SerializeField] private float apogeeOffset = 5.0f;
    [SerializeField] private LayerMask clickMask;

    [Header("References")]
    [SerializeField] private Transform origin;
    [SerializeField] private Camera currentCamera;
    
    public event Action<FiringSolution> FiringSolutionFound;

    private bool _canFire = true;

    private void Update()
    {
        if (Input.GetMouseButton(0) && _canFire)
        {
            if (CheckForValidTarget(out var clickPoint))
            {
                _canFire = false;
                var interval = 1.0f / fireRate;
                Invoke(nameof(OnFireTimerExpired), interval);
            
                var originPosition = origin.position;

                var theta = Random.value * 2.0f * Mathf.PI;
                var error = Random.value * errorRadius;
                var target = clickPoint + new Vector3(
                    Mathf.Cos(theta) * error,
                    0.0f,
                    Mathf.Sin(theta) * error
                );

                var apogeeHeight = Mathf.Max(originPosition.y, target.y) + apogeeOffset;
                var initialVelocity = TrajectoryCalculator.CalculateInitialVelocity(
                    target,
                    originPosition,
                    apogeeHeight,
                    out _
                );
                
                FiringSolutionFound?.Invoke(new FiringSolution
                {
                    InitialVelocity = initialVelocity,
                    Condition = FiringSolution.Conditions.UserSelected,
                });
            }
        }
    }

    private void OnFireTimerExpired()
    {
        _canFire = true;
    }
    
    private bool CheckForValidTarget(out Vector3 point)
    {
        var mouseRay = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out var hitInfo, maxRaycastDistance, clickMask))
        {
            point = hitInfo.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
