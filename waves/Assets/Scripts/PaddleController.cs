using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class PaddleController : MonoBehaviour
{
    [Header("Movement Settings")]

    [Tooltip("Is debug mode enabled? If true, keyboard controls replace microphone input.")]
    [SerializeField]
    private bool _debugMode = false;

    [Tooltip("The left limit of the paddle's movement.")]
    [SerializeField]
    private float _leftBoundary = -4f;

    [Tooltip("The right limit of the paddle's movement.")]
    [SerializeField]
    private float _rightBoundary = 4f;

    [Tooltip("The minimum frequency needed to move the paddle.")]
    [SerializeField]
    private float _minFrequency = 100f;

    [Tooltip("The maximum frequency allowed to move the paddle.")]
    [SerializeField]
    private float _maxFrequency = 400f;

    [Tooltip("The paddle movement speed.")]
    [SerializeField]
    private float _speed = 8f;

    [Header("Visuals")]
    [SerializeField]
    private GameObject _ballPrefab;

    private PitchTracker _pitchTracker;

    void Start()
    {
        Vector3 ballPosition = transform.position + transform.up * 1.5f;
        Instantiate(_ballPrefab, ballPosition, Quaternion.identity);

        _pitchTracker = GameObject.Find("Microphone").GetComponent<PitchTracker>();
    }

    void Update()
    {
        if (_debugMode)
        {
            HandleDebugInput();
            return;
        }

        if (_pitchTracker.CurrentPitch < _minFrequency) return;
        MovePaddle(_pitchTracker.CurrentPitch);
    }

    private void HandleDebugInput()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.aKey.isPressed ? -1 : keyboard.dKey.isPressed ? 1 : 0;

        transform.position += transform.right * moveInput * _speed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, _rightBoundary, _leftBoundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void MovePaddle(float detectedFrequency)
    {
        float pitchNormalized = Mathf.InverseLerp(_minFrequency, _maxFrequency, detectedFrequency);
        pitchNormalized = Mathf.Clamp01(pitchNormalized);

        float targetXPosition = Mathf.Lerp(_leftBoundary, _rightBoundary, pitchNormalized);

        Vector3 targetPos = new (targetXPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
    }
}