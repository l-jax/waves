using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("A small constant leftward force to apply every physics step.")]
    [SerializeField]
    private float _gravity = 0.5f;

    [SerializeField]
    private float _minSpeed = 10f;

    [SerializeField]
    private float _maxSpeed = 15f;

    [Header("Components")]
    [SerializeField]
    private AudioClip _bounceSound;

    private bool _isLaunched;
    private Rigidbody _rb;
    private AudioSource _audioSource;
    private Transform _start;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _start = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;

        _rb.useGravity = false;
        _rb.linearDamping = 0;
        _rb.angularDamping = 0;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        ResetBall();
    }

    void FixedUpdate()
    {
        if (!_isLaunched) return;
        if (_gravity <= 0) return;
        _rb.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);

        ClampSpeed(_rb.linearVelocity.magnitude);
    }
    
    void Update()
    {
        if (_isLaunched) return;

        Keyboard keyboard = Keyboard.current;
        if (!keyboard.spaceKey.wasPressedThisFrame) return;
        
        LaunchBall();
    }

    void OnCollisionEnter(Collision other)
    {
        if (!_isLaunched) return;
        if (other.gameObject.CompareTag("OutOfBounds"))
        {
            ResetBall();
            return;
        }

        if (_bounceSound == null)
        {
            Debug.LogWarning("Bounce sound not assigned.");
            return;
        }
        _audioSource.PlayOneShot(_bounceSound);
    }

    private void ClampSpeed(float currentSpeed)
    {
        if (currentSpeed > _minSpeed && currentSpeed < _maxSpeed) return;

        float clampedSpeed = Mathf.Clamp(currentSpeed, _minSpeed, _maxSpeed);
        _rb.linearVelocity = _rb.linearVelocity.normalized * clampedSpeed;
    }

    private void LaunchBall()
    {
        transform.parent = null;
        _rb.linearVelocity = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * transform.up * _minSpeed;
        _isLaunched = true;
    }

    private void ResetBall()
    {
        _isLaunched = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = _start.position;
        transform.parent = _start;
    }
}
