using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody), typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    public Transform target;
    [SerializeField] private AudioClip[] hurtSounds;

    [Header("Configuration")] [SerializeField]
    public float movementSpeed;

    [SerializeField] private float turnRate = 0.5f;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private Health _health;

    private void Start()
    {
        _transform = transform;

        var position = _transform.position;
        _transform.position = new Vector3(position.x, 0, position.z);

        _rigidbody = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();

        _health.Damaged += OnDamaged;
        _health.Destroyed += OnDestroyed;
    }

    private void OnDestroyed(Health obj)
    {
        _health.Damaged -= OnDamaged;
        _health.Destroyed -= OnDestroyed;
    }

    private void OnDamaged(Health health, IApplyDamage projectile)
    {
        AudioController.Instance.PlayOneOf(hurtSounds, _transform.position);
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
            target.position - _transform.position,
            Vector3.up
        );

        _transform.rotation = Quaternion.Lerp(_transform.rotation, desiredFacing, turnRate);

        _rigidbody.velocity = _transform.TransformVector(Vector3.forward * movementSpeed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Keep>(out var keep))
        {
            keep.Health.ReceiveDamageFromProjectile(new KeepAttack());
            Destroy(gameObject);
        }
    }

    private class KeepAttack : IApplyDamage
    {
        public float Damage => 1.0f;
    }
}
