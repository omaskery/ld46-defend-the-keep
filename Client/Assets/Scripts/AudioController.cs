using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private SoundSource soundSourcePrefab;

    private static AudioController _instance;
    
    private Transform _transform;

    public static AudioController Instance => _instance;
    
    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        
        _instance = this;
        _transform = transform;
    }

    public void PlayOneOf(AudioClip[] alternatives, Vector3 position)
    {
        if (alternatives.Length < 1)
        {
            return;
        }
        
        var clip = alternatives[Random.Range(0, alternatives.Length - 1)];
        PlaySound(clip, position);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        var sound = Instantiate(soundSourcePrefab, position, _transform.rotation, _transform);
        sound.clip = clip;
    }
}
