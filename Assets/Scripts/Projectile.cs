using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask destroyMask;
    [SerializeField] private float safeTime;

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
        
        Debug.Log($"collision {other.gameObject} {objectLayer:X} vs {gameObject} {mask:X} ({objectLayer & mask:X})");
        if ((objectLayer & mask) != 0)
        {
            Destroy(gameObject);
        }
    }
}
