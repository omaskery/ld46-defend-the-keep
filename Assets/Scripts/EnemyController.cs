using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    [Header("References")] [SerializeField]
    public Transform target;

    [Header("Configuration")] [SerializeField]
    public float movementSpeed;

    [SerializeField] private float turnRate = 0.5f;

    private void Start()
    {
        transform.position.Set(transform.position.x, 0, transform.position.z);
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MoveTowardTarget();
    }

    private void MoveTowardTarget()
    {
        if (!target)
        {
            return;
        }

        var desiredFacing = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up
        );

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredFacing, turnRate);

        _rigidbody.velocity = transform.TransformVector(Vector3.forward * movementSpeed);
    }
}
