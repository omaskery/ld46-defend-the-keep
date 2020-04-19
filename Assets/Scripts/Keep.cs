using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Health))]
public class Keep : MonoBehaviour
{
    [SerializeField] private float healthToHeightCoefficient = 0.1f;
    [SerializeField] private Transform keepTop;

    [NonSerialized] private Transform _transform;
    [field: NonSerialized] public Health Health { get; private set; }

    public float CurrentHeight => Health.CurrentHealth * healthToHeightCoefficient;

    void Start()
    {
        _transform = transform;
        Health = GetComponent<Health>();
    }

    void Update()
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
