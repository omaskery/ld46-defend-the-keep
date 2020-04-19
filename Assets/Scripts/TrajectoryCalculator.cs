using UnityEngine;

static class TrajectoryCalculator
{
    public static Vector3 CalculateInitialVelocity(Vector3 target, Vector3 originPosition, float apogeeHeight,
        out float totalTimeToTarget)
    {
        var targetHorizontalPosition = new Vector3(target.x, 0, target.z);
        var originHorizontalPosition = new Vector3(originPosition.x, 0, originPosition.z);
        var horizontalDirection = (targetHorizontalPosition - originHorizontalPosition).normalized;
        var horizontalDistance = (targetHorizontalPosition - originHorizontalPosition).magnitude;
        
        var heightFromOriginToApogee = apogeeHeight - originPosition.y;
        var initialUpwardVelocity = Mathf.Sqrt(-2.0f * heightFromOriginToApogee * Physics.gravity.y);
        var timeToApogee = -initialUpwardVelocity / Physics.gravity.y;
        var heightFromApogeeToTarget = target.y - apogeeHeight;
        var timeFromApogeeToTarget = Mathf.Sqrt(2.0f * heightFromApogeeToTarget / Physics.gravity.y);
        totalTimeToTarget = timeToApogee + timeFromApogeeToTarget;
        var horizontalSpeed = horizontalDistance / totalTimeToTarget;

        return (horizontalDirection * horizontalSpeed) + (Vector3.up * initialUpwardVelocity);
    }
}