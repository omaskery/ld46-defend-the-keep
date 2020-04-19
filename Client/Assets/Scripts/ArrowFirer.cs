using UnityEngine;

public class ArrowFirer : MonoBehaviour
{
    [SerializeField] private Rigidbody arrowPrefab;
    [SerializeField] private Transform arrowsParent;

    public void Fire(Vector3 origin, Vector3 initialVelocity)
    {
        var rotation = Quaternion.LookRotation(initialVelocity);
        var rigidBody = Instantiate(arrowPrefab, origin, rotation, arrowsParent);
        rigidBody.velocity = initialVelocity;
    }
}
