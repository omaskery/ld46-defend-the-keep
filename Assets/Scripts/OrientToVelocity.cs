using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrientToVelocity : MonoBehaviour
{
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        transform.forward = _rigidbody.velocity;
    }
}
