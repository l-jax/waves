using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(AudioSource))]
public class BallController : MonoBehaviour
{
    [Tooltip("The sound played when the ball bounces.")]
    public AudioClip _bounceSound;

    private readonly float _speed = 8f;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        transform.right = new Vector3(0.5f, 0.5f, 0f);
    }

    void Update()
    {
        transform.position += transform.right * _speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision other)
    {
        _audioSource.PlayOneShot(_bounceSound);
        Vector3 reflectDirection = Vector3.Reflect(transform.right, other.contacts[0].normal);
        transform.right = reflectDirection.normalized;
    }
}