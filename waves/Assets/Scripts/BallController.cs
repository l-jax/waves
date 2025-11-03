using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BallController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("A small constant downward force to apply every physics step.")]
    [SerializeField]
    private float _gravity = 0.5f;

    [SerializeField]
    private float _minSpeed = 10f;

    [SerializeField]
    private float _maxSpeed = 15f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _bounceSound;

    [Tooltip("Minimum random pitch for the bounce sound.")]
    [SerializeField]
    private float _minPitch = 0.9f;

    [Tooltip("Maximum random pitch for the bounce sound.")]
    [SerializeField]
    private float _maxPitch = 1.1f;

    [Header("Squash & Stretch")]
    [Tooltip("How 'wide' the ball gets on squash (e.g., 1.2 = 120% width).")]
    [SerializeField]
    private float _squashAmount = 1.2f;

    [Tooltip("How 'tall' the ball gets on stretch (e.g., 1.2 = 120% height).")]
    [SerializeField]
    private float _stretchAmount = 1.2f;

    [Tooltip("How fast the ball goes from squash to stretch.")]
    [SerializeField]
    private float _squashToStretchTime = 0.08f;

    [Tooltip("How fast the ball goes from stretch back to normal.")]
    [SerializeField]
    private float _stretchToNormalTime = 0.08f;

    [Header("Effects")]
    [Tooltip("The particle effect prefab to spawn on collision.")]
    [SerializeField]
    private ParticleSystem _bounceParticlesPrefab;

    private bool _isLaunched;
    private Rigidbody _rb;
    private AudioSource _audioSource;
    private Transform _start;
    private Vector3 _originalScale = new (0.5f, 0.5f, 0.5f);
    private Coroutine _squashStretchCoroutine;
    private CameraController _cameraController;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _cameraController = Camera.main.GetComponent<CameraController>();
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
        
        PlayCollisionEffects(other);

        if (!other.gameObject.CompareTag("Player")) return;

        HitPaddle(other.contacts[0].point, other.transform);
    }

    private void PlayCollisionEffects(Collision collision)
    {
        _cameraController.TriggerShake(0.1f, 0.1f);

        if (_bounceSound != null)
        {
            _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
            _audioSource.PlayOneShot(_bounceSound);
        }
        else
        {
            Debug.LogWarning("Bounce sound not assigned.");
        }

        if (_bounceParticlesPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion particleRotation = Quaternion.LookRotation(contact.normal);
            Instantiate(_bounceParticlesPrefab, contact.point, particleRotation);
        }

        if (_squashStretchCoroutine != null)
        {
            StopCoroutine(_squashStretchCoroutine);
        }
        _squashStretchCoroutine = StartCoroutine(AnimateSquashStretch());
    }

    private IEnumerator AnimateSquashStretch()
    {
        Vector3 squashScale = new (_originalScale.x * _squashAmount, _originalScale.y / _squashAmount, _originalScale.z);
        Vector3 stretchScale = new (_originalScale.x / _stretchAmount, _originalScale.y * _stretchAmount, _originalScale.z);

        float timer = 0f;

        transform.localScale = squashScale;

        while (timer < _squashToStretchTime)
        {
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, timer / _squashToStretchTime);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = stretchScale;

        while (timer < _stretchToNormalTime)
        {
            transform.localScale = Vector3.Lerp(stretchScale, _originalScale, timer / _stretchToNormalTime);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _originalScale;
        _squashStretchCoroutine = null;
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

        if (_squashStretchCoroutine != null)
        {
            StopCoroutine(_squashStretchCoroutine);
            _squashStretchCoroutine = null;
        }
        transform.localScale = _originalScale;

        _cameraController.StopShake();

        transform.parent = _start;
    }

    private void HitPaddle(Vector3 collisionPoint, Transform paddleTransform)
    {
        float paddleWidth = paddleTransform.GetComponent<Collider>().bounds.size.x;
        float hitFactor = (collisionPoint.x - paddleTransform.position.x) / (paddleWidth / 2f);
        hitFactor = Mathf.Clamp(hitFactor, -1f, 1f);

        Vector3 newDirection = new Vector3(hitFactor, 1, 0).normalized;
        float currentSpeed = _rb.linearVelocity.magnitude;
        _rb.linearVelocity = newDirection * currentSpeed;
        ClampSpeed(_rb.linearVelocity.magnitude);
    }
}