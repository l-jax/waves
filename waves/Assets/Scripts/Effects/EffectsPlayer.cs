using UnityEngine;

public enum Effect
{
    Break,
    Bounce,
    OutOfBounds,
    Win
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

    [SerializeField]
    private AudioClip _outOfBoundsSound;

    [SerializeField]
    private ParticleSystem _outOfBoundsParticles;

    [SerializeField]
    private AudioClip _winSound;

    [SerializeField]
    private ParticleSystem _winParticles;

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
                PlayAudio(effect, _breakSound);
                PlayParticles(effect, _breakParticles, position, rotation);
                break;
            case Effect.Bounce:
                PlayAudio(effect, _bounceSound);
                PlayParticles(effect, _bounceParticles, position, rotation);
                break;
            case Effect.OutOfBounds:
                PlayAudio(effect, _outOfBoundsSound);
                PlayParticles(effect, _outOfBoundsParticles, position, rotation);
                break;
            case Effect.Win:
                PlayAudio(effect, _winSound);
                PlayParticles(effect, _winParticles, position, rotation);
                break;
            default:
                Debug.LogWarning("Unknown effect: " + effect);
                break;
        }
    }

    private void PlayAudio(Effect effect, AudioClip sound)
    {
        if (sound == null)
        {
            Debug.LogWarning($"{effect} sound not assigned.");
        }
        else
        {
            _audioSource.PlayOneShot(sound);
        }
    }

    private void PlayParticles(Effect effect, ParticleSystem particles, Vector3 position, Quaternion rotation)
    {
        if (particles == null)
        {
            Debug.LogWarning($"{effect} particles not assigned.");
        }
        else
        {
            Instantiate(particles, position, rotation);
        }
    }
}