using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    private AudioSource _source;
    public AudioClip clip;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        
        Invoke(nameof(OnSoundComplete), clip.length);
        
        _source.pitch = 1.0f + (Random.value - 0.5f) * 0.1f;
        _source.PlayOneShot(clip);
    }

    private void OnSoundComplete()
    {
        Destroy(gameObject);
    }
}
