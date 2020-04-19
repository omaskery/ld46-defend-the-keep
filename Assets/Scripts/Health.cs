using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, ITakeDamage
{
    [SerializeField] public float maximumHealth = 100.0f;
    [SerializeField] public float currentHealth;
    [SerializeField] public bool wantsHealthbar = true;
    [SerializeField] private bool destroyOnZeroHealth = true;

    public float MaximumHealth => maximumHealth;
    public float CurrentHealth => currentHealth;

    public void Start()
    {
        currentHealth = maximumHealth;
    }
    
    public void ReceiveDamageFromProjectile(IApplyDamage projectile)
    {
        currentHealth -= projectile.Damage;
        
        Debug.Log($"{gameObject.name} took {projectile.Damage} damage from {projectile}");
        
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
