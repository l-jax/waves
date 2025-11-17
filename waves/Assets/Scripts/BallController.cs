using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public Action OnOutOfBounds;

    [Header("Movement")]
    [Tooltip("A small constant downward force to apply every physics step.")]
    [SerializeField]
    private float _gravity = 0.6f;

    [SerializeField]
    private float _minSpeed = 5f;

    [SerializeField]
    private float _maxSpeed = 10f;

    private bool _isLaunched;
    private bool _isMovementEnabled;

    private Vector3 _startPosition;
    private Rigidbody _rb;

    private Transform _paddle;
    private EffectsPlayer _effectsPlayer;
    private CameraController _cameraController;

    void Awake()
    {
        _startPosition = transform.localPosition;

        _paddle = GameObject.FindGameObjectWithTag("Player").transform;
        _effectsPlayer = GameObject.Find("EffectsPlayer").GetComponent<EffectsPlayer>();
        _cameraController = Camera.main.GetComponent<CameraController>();

        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.linearDamping = 0;
        _rb.angularDamping = 0;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        ResetBall();
    }

    void FixedUpdate()
    {
        if (!_isLaunched || !_isMovementEnabled) return;
        if (_gravity <= 0) return;
        _rb.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);

        ClampSpeed(_rb.linearVelocity.magnitude);
    }
    
    void Update()
    {
        if (_isLaunched || !_isMovementEnabled) return;

        Keyboard keyboard = Keyboard.current;
        if (!keyboard.spaceKey.wasPressedThisFrame) return;
        
        LaunchBall();
    }

    void OnCollisionEnter(Collision other)
    {
        if (!_isLaunched || !_isMovementEnabled) return;

        if (other.gameObject.CompareTag("OutOfBounds"))
        {
            _effectsPlayer.PlayEffect(Effect.OutOfBounds, transform.position, Quaternion.identity);
            OnOutOfBounds?.Invoke();
            ResetBall();
            return;
        }

        PlayCollisionEffects(other);

        if (!other.gameObject.CompareTag("Player")) return;

        BounceOffPaddle(other.contacts[0].point, other.transform);
    }
    
    public void EnableMovement()
    {
        _isMovementEnabled = true;
    }

    public void DisableMovement()
    {
        _isMovementEnabled = false;
        ResetBall();
    }

    private void PlayCollisionEffects(Collision other)
    {
        ShakeType shakeType = other.gameObject.CompareTag("Block") ? ShakeType.Large : ShakeType.Small;
        _cameraController.Shake(shakeType);

        _effectsPlayer.PlayEffect(Effect.Bounce, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
    }

    private void BounceOffPaddle(Vector3 collisionPoint, Transform paddleTransform)
    {
        float paddleWidth = paddleTransform.GetComponent<Collider>().bounds.size.x;
        float hitFactor = (collisionPoint.x - paddleTransform.position.x) / (paddleWidth / 2f);
        hitFactor = Mathf.Clamp(hitFactor, -1f, 1f);

        Vector3 newDirection = new Vector3(hitFactor, 1, 0).normalized;
        float currentSpeed = _rb.linearVelocity.magnitude;
        _rb.linearVelocity = newDirection * currentSpeed;
        ClampSpeed(_rb.linearVelocity.magnitude);
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
        _rb.linearVelocity = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-45f, 45f)) * transform.up * _minSpeed;
        transform.GetComponent<TrailRenderer>().enabled = true;
        _isLaunched = true;
    }

    private void ResetBall()
    {
        _isLaunched = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        transform.parent = _paddle;
        transform.localPosition = _startPosition;

        transform.GetComponent<TrailRenderer>().enabled = false;
    }
}