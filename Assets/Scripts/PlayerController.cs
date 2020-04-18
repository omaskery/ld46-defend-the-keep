using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform coordinatesCentredOn;

    [Header("Ground Check")] [SerializeField]
    private LayerMask groundCheckMask;

    [SerializeField] private float raycastStartHeight = 1000.0f;

    [Header("Configuration")] [SerializeField]
    private float horizontalScaling = 0.1f;

    [SerializeField] private float verticalScaling = 1.0f;

    void Start()
    {
        _polarCoordinates = polarCordinatesOf(coordinatesCentredOn, transform);
    }

    void FixedUpdate()
    {
        var inputVector = new Vector2(
            Input.GetAxisRaw("Horizontal") * horizontalScaling,
            Input.GetAxisRaw("Vertical") * verticalScaling
        );

        var newPolarCoordinate = _polarCoordinates + inputVector;
        _polarCoordinates = newPolarCoordinate;
        var newCartesianCoordinate = cartesianCoordinatesOf(coordinatesCentredOn, newPolarCoordinate);

        var nextPosition = new Vector3(
            newCartesianCoordinate.x,
            RaycastGroundHeight(),
            newCartesianCoordinate.y
        );

        transform.position = nextPosition;
    }

    private float RaycastGroundHeight()
    {
        var position = transform.position;
        var raycastOrigin = new Vector3(position.x, raycastStartHeight, position.z);

        if (Physics.Raycast(raycastOrigin, Vector3.down, out var hitInfo, raycastStartHeight * 2, groundCheckMask))
        {
            return hitInfo.point.y;
        }

        return raycastStartHeight;
    }

    private Vector2 polarCordinatesOf(Transform centre, Transform target)
    {
        var targetPosition = target.position;
        var centrePosition = centre.position;
        var theta = Mathf.Atan2(
            targetPosition.z - centrePosition.z,
            targetPosition.x - centrePosition.x
        );
        var radius = (targetPosition - centrePosition).magnitude;
        return new Vector2(theta, radius);
    }

    private Vector2 cartesianCoordinatesOf(Transform centre, Vector2 polar)
    {
        return new Vector2(
            Mathf.Cos(polar.x) * polar.y,
            Mathf.Sin(polar.x) * polar.y
        );
    }

    private Vector2 _polarCoordinates;
}