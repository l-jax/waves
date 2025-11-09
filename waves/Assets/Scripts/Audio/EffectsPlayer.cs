using UnityEngine;

public enum Effect
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
    private ParticleSystem _breakParticles;

    [SerializeField]
    private AudioClip _bounceSound;

    [SerializeField]
    private ParticleSystem _bounceParticles;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayEffect(Effect effect, Vector3 position, Quaternion rotation)
    {
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
            return;
        }

        switch (effect)
        {
            case Effect.Break:
                PlayBreakEffect(position, rotation);
                break;
            case Effect.Bounce:
                PlayBounceEffect(position, rotation);
                break;
            default:
                Debug.LogWarning("Unknown sound effect: " + effect);
                break;
        }
    }

    private void PlayBreakEffect(Vector3 position, Quaternion rotation)
    {
        if (_breakSound == null)
        {
            Debug.LogWarning("Break sound not assigned.");
        }
        else
        {
            _audioSource.PlayOneShot(_breakSound);
        }

        if (_breakParticles == null)
        {
            Debug.LogWarning("Break particles not assigned.");
        }
        else
        {
            Instantiate(_breakParticles, position, rotation);
        }
    }

    private void PlayBounceEffect(Vector3 position, Quaternion rotation)
    {
        if (_bounceSound == null)
        {
            Debug.LogWarning("Bounce sound not assigned.");
        }
        else
        {
            _audioSource.PlayOneShot(_bounceSound);
        }

        if (_bounceParticles == null)
        {
            Debug.LogWarning("Bounce particles not assigned.");
        }
        else
        {
            Instantiate(_bounceParticles, position, rotation);
        }
    }
}