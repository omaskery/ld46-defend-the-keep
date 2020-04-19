using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask destroyMask;
    [SerializeField] private float safeTime;
    [SerializeField] public float damage;

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
            
            Destroy(gameObject);
        }
    }
}

public interface ITakeDamage
{
    void ReceiveDamageFromProjectile(Projectile projectile);
}
