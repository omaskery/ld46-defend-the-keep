using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float maximumHealth = 100.0f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool destroyOnZeroHealth = true;

    public void Start()
    {
        currentHealth = maximumHealth;
    }
    
    public void ReceiveDamageFromProjectile(IApplyDamage projectile)
    {
        currentHealth -= projectile.Damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            if (destroyOnZeroHealth)
            {
                Destroy(gameObject);
            }
        }
    }
}
