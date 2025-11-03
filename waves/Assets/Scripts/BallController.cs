using System;
using UnityEngine;

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

    private Rigidbody _rb;
    private AudioSource _audioSource;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        _rb.useGravity = false;
        _rb.linearDamping = 0;
        _rb.angularDamping = 0;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _rb.linearVelocity = transform.right * _minSpeed;
    }

     void FixedUpdate()
    {
        if (_gravity <= 0) return;
        _rb.AddForce(Vector3.right * -_gravity, ForceMode.Acceleration);

        ClampSpeed(_rb.linearVelocity.magnitude);
    }

    void OnCollisionEnter(Collision other)
    {
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
}
