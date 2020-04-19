using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] public Health health;
    [SerializeField] public Camera currentCamera;
    
    private Slider _slider;

    public event Action<HealthBar> Complete;
    
    void Start()
    {
        _slider = GetComponent<Slider>();
        if (currentCamera == null)
        {
            currentCamera = Camera.main;
        }
    }

    void Update()
    {
        if (health)
        {
            _slider.minValue = 0.0f;
            _slider.maxValue = health.MaximumHealth;
            _slider.value = health.CurrentHealth;

            var targetWorldPosition = health.transform.position + (Vector3.up * 1.5f);
            var screenSpacePosition = currentCamera.WorldToScreenPoint(targetWorldPosition);
            _slider.transform.position = screenSpacePosition;
        }
        else
        {
            Complete?.Invoke(this);
        }
    }
}
