using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] public float startRadius;
    [SerializeField] public float endRadius;
    [SerializeField] public float lifetime;
    [SerializeField] public Vector3 velocity;

    private Transform _transform;
    private float _timeAlive;
    
    void Start()
    {
        _transform = transform;
        SetRadius(startRadius);
        Invoke(nameof(OnExpiration), lifetime);
    }

    void Update()
    {
        _timeAlive += Time.deltaTime;

        var ratio = Mathf.Clamp(_timeAlive / lifetime, 0.0f, 1.0f);
        SetRadius(Mathf.Lerp(startRadius, endRadius, ratio));

        _transform.position = _transform.position + velocity * Time.deltaTime;
    }

    private void OnExpiration()
    {
        Destroy(gameObject);
    }

    private void SetRadius(float radius)
    {
        _transform.localScale = new Vector3(radius, radius, radius);
    }
}
