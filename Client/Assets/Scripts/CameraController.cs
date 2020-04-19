using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lookTowards;

    [SerializeField] private Transform lookFrom;

    [Header("Configuration")] [SerializeField]
    private float cameraHeight;
    
    [SerializeField] private float cameraDistance;

    [SerializeField] private float lerpAlpha = 0.1f;
    [SerializeField] private bool snapOnStart;
    
    void Start()
    {
        if (snapOnStart)
        {
            UpdateCameraPosition(true);
        }
    }

    void FixedUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition(bool snap=false)
    {
        if (!lookTowards)
        {
            return;
        }
        
        var lookFromPosition = lookFrom.position;
        var lookTowardsPosition = lookTowards.position;

        var lookFromDirection = (lookFromPosition - lookTowardsPosition).normalized;
        lookFromPosition = lookTowardsPosition + (lookFromDirection * cameraDistance);
        
        var targetPosition = new Vector3(lookFromPosition.x, cameraHeight, lookFromPosition.z);
        if (!snap)
        {
            targetPosition = Vector3.Slerp(transform.position, targetPosition, lerpAlpha);
        }

        transform.position = targetPosition;
        transform.LookAt(lookTowards);
    }
}
