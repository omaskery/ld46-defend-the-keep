using System;
using UnityEngine;

public class Health : MonoBehaviour, ITakeDamage
{
    [SerializeField] public float maximumHealth = 100.0f;
    [SerializeField] public float currentHealth;
    [SerializeField] public bool wantsHealthbar = true;
    [SerializeField] private bool destroyOnZeroHealth = true;

    public float MaximumHealth => maximumHealth;
    public float CurrentHealth => currentHealth;

    public event Action<Health> Destroyed;
    public event Action<Health, IApplyDamage> Damaged;

    public void Start()
    {
        if (Math.Abs(currentHealth) < Mathf.Epsilon)
        {
            currentHealth = maximumHealth;
        }
    }
    
    public void ReceiveDamageFromProjectile(IApplyDamage projectile)
    {
        currentHealth = Mathf.Clamp(currentHealth - projectile.Damage, 0.0f, maximumHealth);
        
        Damaged?.Invoke(this, projectile);

        if (currentHealth <= 0 && destroyOnZeroHealth)
        {
            Destroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
