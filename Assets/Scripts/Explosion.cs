using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float maxDamage;
    [SerializeField] private float minDamage;
    [SerializeField] private float startRadius;
    [SerializeField] private float endRadius;
    [SerializeField] private float lifetime;
    
    private HashSet<GameObject> _damaged = new HashSet<GameObject>();
    private float _timeAlive = 0.0f;
    private float _currentRadius;
    
    void Start()
    {
        SetRadius(startRadius);
        Destroy(gameObject, lifetime);
    }

    private void SetRadius(float radius)
    {
        _currentRadius = radius;
        transform.localScale = new Vector3(radius, radius, radius);
    }

    void Update()
    {
        _timeAlive += Time.deltaTime;

        var ratio = Mathf.Clamp(_timeAlive / lifetime, 0.0f, 1.0f);
        SetRadius(Mathf.Lerp(startRadius, endRadius, ratio));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_damaged.Contains(other.gameObject))
        {
            return;
        }

        if (other.gameObject.TryGetComponent<ITakeDamage>(out var damageable))
        {
            var explosionPosition = transform.position;
            var distance = (other.ClosestPoint(explosionPosition) - explosionPosition).sqrMagnitude;
            var distanceRatio = Mathf.Clamp(distance / (endRadius * endRadius), 0.0f, 1.0f);
            damageable.ReceiveDamageFromProjectile(new DamageRay
            {
                Damage = Mathf.Lerp(maxDamage, minDamage, distanceRatio),
            });
        }

        _damaged.Add(other.gameObject);
    }

    private class DamageRay : IApplyDamage
    {
        public float Damage { get; set; }
    }
}
