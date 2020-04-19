using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] public Keep keep;
    [SerializeField] public Text survivalTimeText;

    private Color _initialColour;
    private float _survivalTime;

    void Start()
    {
        _initialColour = survivalTimeText.color;
    }

    void Update()
    {
        if (keep)
        {
            _survivalTime += Time.deltaTime;
        }

        var timespan = TimeSpan.FromSeconds(_survivalTime);
        survivalTimeText.text = $"Survival Time: {timespan}";

        if (keep)
        {
            survivalTimeText.color = _initialColour;
        }
        else
        {
            survivalTimeText.color = Color.red;
        }
    }
}
