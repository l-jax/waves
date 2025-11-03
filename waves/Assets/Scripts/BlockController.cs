using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class BlockController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private AudioClip _breakSound;

    [SerializeField]
    private ParticleSystem _breakParticles;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision other)
    {
        _audioSource.PlayOneShot(_breakSound);
        if (_breakParticles == null)
        {
            Debug.LogWarning("Break particles not assigned.");
        } else
        {
            Instantiate(_breakParticles, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }
}