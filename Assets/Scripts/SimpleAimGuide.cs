using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAimGuide : MonoBehaviour, IReportFiringSolutions
{
    [Header("Configuration")]
    [SerializeField] private float maxRaycastDistance;
    [SerializeField] private float closestHeight;
    [SerializeField] private float closestDistance;
    [SerializeField] private float furthestHeight;
    [SerializeField] private float furthestDistance;
    [SerializeField] private LayerMask clickMask;

    [Header("References")]
    [SerializeField] private Transform origin;
    [SerializeField] private Camera currentCamera;
    
    public event Action<FiringSolution> FiringSolutionFound;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckForValidTarget(out var clickPoint))
            {
                var originPosition = origin.position;
                
                var initialVelocity = TrajectoryCalculator.CalculateInitialVelocity(
                    clickPoint,
                    originPosition,
                    originPosition.y + 5.0f,
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
}
