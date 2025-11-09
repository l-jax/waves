using UnityEngine;

public enum SoundEffect
{
    Break,
    Bounce
}

[RequireComponent(typeof(AudioSource))]
public class EffectsPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip _breakSound;

    [SerializeField]
    private AudioClip _bounceSound;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundEffect soundEffect)
    {
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
            return;
        }
        
        switch (soundEffect)
        {
            case SoundEffect.Break:
                if (_breakSound == null) {
                    Debug.LogWarning("Break sound not assigned.");
                    return;
                }
                _audioSource.PlayOneShot(_breakSound);
                break;
            case SoundEffect.Bounce:
                if (_bounceSound == null) {
                    Debug.LogWarning("Bounce sound not assigned.");
                    return;
                }
                _audioSource.PlayOneShot(_bounceSound);
                break;
            default:
                Debug.LogWarning("Unknown sound effect: " + soundEffect);
                break;
        }
    }
}