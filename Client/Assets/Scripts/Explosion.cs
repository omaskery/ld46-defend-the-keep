using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private Smoke smokePrefab;
    [SerializeField] private float maxDamage;
    [SerializeField] private float minDamage;
    [SerializeField] private float startRadius;
    [SerializeField] private float endRadius;
    [SerializeField] private float lifetime;
    
    [SerializeField] private float secondaryExplosionProbability;
    [SerializeField] private float secondaryExplosionRange;
    [SerializeField] private float secondaryExplosionRadiusCoefficient;
    
    [SerializeField] private float smokeProbability;
    [SerializeField] private float smokeRange;
    [SerializeField] private float smokeRadiusMin;
    [SerializeField] private float smokeRadiusMax;
    [SerializeField] private float smokeQuantityMin;
    [SerializeField] private float smokeQuantityMax;
    [SerializeField] private float smokeLifetimeMin;
    [SerializeField] private float smokeLifetimeMax;
    
    private readonly HashSet<GameObject> _damaged = new HashSet<GameObject>();
    private float _timeAlive = 0.0f;
    private Transform _transform;
    
    void Start()
    {
        _transform = transform;
        SetRadius(startRadius);
        Invoke(nameof(OnExpiration), lifetime);
    }

    private void OnExpiration()
    {
        GenerateSecondaryExplosions();
        GenerateSmoke();

        Destroy(gameObject);
    }

    private void GenerateSmoke()
    {
        var count = Random.Range(smokeQuantityMin, smokeQuantityMax);
        for (int i = 0; i < count; i++)
        {
            if (Random.value <= smokeProbability)
            {
                var offset = Random.value * smokeRange;
                var radius = Mathf.Lerp(smokeRadiusMin, smokeRadiusMax, Random.value);
                var theta = Random.value * 2.0f * Mathf.PI;
                var position = _transform.position + new Vector3(
                    Mathf.Cos(theta) * offset,
                    0.0f,
                    Mathf.Sin(theta) * offset
                );

                var smoke = Instantiate(smokePrefab, position, _transform.rotation, _transform.parent);
                smoke.startRadius = radius;
                smoke.endRadius = 0.0f;
                smoke.lifetime = Mathf.Lerp(smokeLifetimeMin, smokeLifetimeMax, Random.value);
                smoke.velocity = new Vector3(
                    Random.value,
                    0.2f + Random.value * 1.0f,
                    Random.value
                );
            }
        }
    }

    private void GenerateSecondaryExplosions()
    {
        if (Random.value <= secondaryExplosionProbability)
        {
            var theta = Random.value * Mathf.PI * 2.0f;
            var radius = Random.value * secondaryExplosionRange;
            var position = _transform.position + new Vector3(
                Mathf.Cos(theta) * radius,
                0.0f,
                Mathf.Sin(theta) * radius
            );

            var explosion = Instantiate(explosionPrefab, position, _transform.rotation, _transform.parent);
            explosion.endRadius = endRadius * secondaryExplosionRadiusCoefficient;
            explosion.startRadius = startRadius * secondaryExplosionRadiusCoefficient;
            explosion.lifetime *= 0.5f;
            explosion.maxDamage = 0.0f;
            explosion.minDamage = 0.0f;
        }
    }

    private void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }

    void Update()
    {
        _timeAlive += Time.deltaTime;

        var ratio = Mathf.Clamp(_timeAlive / lifetime, 0.0f, 1.0f);
        SetRadius(Mathf.Lerp(startRadius, endRadius, ratio));
    }

    public void OnCollisionEnter(Collision other)
    {
        if (_damaged.Contains(other.gameObject))
        {
            return;
        }

        if (other.gameObject.TryGetComponent<ITakeDamage>(out var damageable))
        {
            var explosionPosition = transform.position;
            var distanceSquared = (other.contacts[0].point - explosionPosition).sqrMagnitude;
            var distanceRatio = Mathf.Clamp(distanceSquared / (endRadius * endRadius), 0.0f, 1.0f);
            var damage = Mathf.Lerp(maxDamage, minDamage, distanceRatio);
            Debug.Log($"damage ray {{damage: {damage} distance: {Mathf.Sqrt(distanceSquared)} ratio: {distanceRatio}}}");
            damageable.ReceiveDamageFromProjectile(new DamageRay
            {
                Damage = damage,
            });
        }

        _damaged.Add(other.gameObject);
    }

    private class DamageRay : IApplyDamage
    {
        public float Damage { get; set; }
    }
}
