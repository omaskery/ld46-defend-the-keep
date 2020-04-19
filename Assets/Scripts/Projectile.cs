using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IApplyDamage
{
    [SerializeField] private GameObject destructionPrefab;
    [SerializeField] private LayerMask destroyMask;
    [SerializeField] private float safeTime;
    [SerializeField] private float damage;

    public float Damage => damage;

    private void Update()
    {
        if (safeTime > 0.0f)
        {
            safeTime -= Time.deltaTime;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (safeTime > 0.0f)
        {
            return;
        }
        
        var objectLayer = 1 << other.collider.gameObject.layer;
        var mask = destroyMask.value;
        
        if ((objectLayer & mask) != 0)
        {
            if (other.gameObject.TryGetComponent<ITakeDamage>(out var damageable))
            {
                damageable.ReceiveDamageFromProjectile(this);
            }

            if (destructionPrefab)
            {
                Debug.Log("spawning explosion");
                Instantiate(destructionPrefab, transform.position, Quaternion.LookRotation(other.contacts[0].normal), transform.parent);
            }
            
            Destroy(gameObject);
        }
    }
}

public interface IApplyDamage
{
    float Damage { get; }
}

public interface ITakeDamage
{
    void ReceiveDamageFromProjectile(IApplyDamage projectile);
}
