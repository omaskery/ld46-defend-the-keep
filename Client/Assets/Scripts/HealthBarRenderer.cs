using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthBarRenderer : MonoBehaviour
{
    [SerializeField] public HealthBar healthBarPrefab;
    
    private readonly Dictionary<HealthBar, Health> _displayedHealthBars = new Dictionary<HealthBar, Health>();
    private readonly List<HealthBar> _availableHealthbars = new List<HealthBar>();
    private Transform _transform;

    void Start()
    {
        _transform = transform;
    }
    
    void Update()
    {
        var entitiesWithHealth = FindObjectsOfType<Health>()
            .Where(h => h.wantsHealthbar);

        foreach (var entity in entitiesWithHealth)
        {
            if (_displayedHealthBars.ContainsValue(entity))
            {
                continue;
            }

            if (_availableHealthbars.Count < 1)
            {
                var healthBar = Instantiate(healthBarPrefab, _transform);
                healthBar.Complete += OnHealthBarComplete;
                _availableHealthbars.Add(healthBar);
            }

            var allocatedHealthBar = _availableHealthbars[Random.Range(0, _availableHealthbars.Count - 1)];
            allocatedHealthBar.health = entity;
            allocatedHealthBar.gameObject.SetActive(true);
            _displayedHealthBars[allocatedHealthBar] = entity;

            _availableHealthbars.Remove(allocatedHealthBar);
        }
    }

    private void OnHealthBarComplete(HealthBar healthBar)
    {
        healthBar.gameObject.SetActive(false);
        _displayedHealthBars.Remove(healthBar);
        _availableHealthbars.Add(healthBar);
    }
}
