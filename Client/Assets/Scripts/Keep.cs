using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Health))]
public class Keep : MonoBehaviour
{
    [SerializeField] private float healthToHeightCoefficient = 0.1f;
    [SerializeField] private Transform keepTop;
    [SerializeField] private float healInterval = 10.0f;
    [SerializeField] private int healAmount = 1;

    [NonSerialized] private Transform _transform;
    [field: NonSerialized] public Health Health { get; private set; }

    public float CurrentHeight => Health.CurrentHealth * healthToHeightCoefficient;

    void Start()
    {
        _transform = transform;
        
        Health = GetComponent<Health>();
        Health.Damaged += OnHealthChanged;
        Health.Destroyed += OnDestroyed;

        ScheduleHealing();
    }

    private void ScheduleHealing()
    {
        Invoke(nameof(OnScheduledHeal), healInterval);
    }

    private void OnScheduledHeal()
    {
        ScheduleHealing();

        Health.currentHealth += healAmount;
        UpdateHeight();
    }

    private void OnDestroyed(Health health)
    {
        Health.Damaged -= OnHealthChanged;
        Health.Destroyed -= OnDestroyed;
    }

    private void OnHealthChanged(Health health, IApplyDamage projectile)
    {
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        var scale = _transform.localScale;
        _transform.localScale = new Vector3(scale.x, CurrentHeight, scale.z);

        keepTop.position = Vector3.up * CurrentHeight;
    }
}
