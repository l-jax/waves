using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class PaddleController : MonoBehaviour
{
    public float CurrentVolume => _microphoneAdaptor == null ? 0f : _microphoneAdaptor.GetCurrentVolume();


    [SerializeField]
    private LineRenderer _volumeVisualizer;

    [Header("Movement Settings")]
    [Tooltip("Control with keyboard or voice?")]
    [SerializeField]
    private ControlSystem _controlSystem = ControlSystem.Keyboard;

    [Tooltip("The left limit of the paddle's movement.")]
    [SerializeField]
    private float _leftBoundary = -4f;

    [Tooltip("The right limit of the paddle's movement.")]
    [SerializeField]
    private float _rightBoundary = 4f;

    [Tooltip("The speed at which the paddle moves.")]
    [SerializeField]
    private float _speed = 10f;

    private MicrophoneAdaptor _microphoneAdaptor;

    private bool _isMovementEnabled = true;

    void Start()
    {
        _microphoneAdaptor = new MicrophoneAdaptor(
            _volumeVisualizer,
            GetComponent<AudioSource>(),
            new CalibrationData()
        );
    }

    void Update()
    {
        if (!_isMovementEnabled) return;

        switch (_controlSystem)
        {
            case ControlSystem.Keyboard:
                HandleKeyboardInput();
                break;
            case ControlSystem.Voice:
                _microphoneAdaptor.Update();
                HandleVoiceInput();
                break;
        }
        ClampPosition();
    }

    public void SetControlSystem(ControlSystem controlSystem)
    {
        _controlSystem = controlSystem;
    }

    public void EnableMovement()
    {
        _isMovementEnabled = true;
    }

    public void DisableMovement()
    {
        _isMovementEnabled = false;
    }

    public void ApplyCalibrationData(CalibrationData calibrationData)
    {
        _microphoneAdaptor = new MicrophoneAdaptor(_volumeVisualizer, GetComponent<AudioSource>(), calibrationData);
    }

    private void HandleKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.aKey.isPressed ? -1 : keyboard.dKey.isPressed ? 1 : 0;
        transform.position += moveInput * _speed * Time.deltaTime * transform.right;
    }

    private void HandleVoiceInput()
    {
        if (_microphoneAdaptor == null) return;
        Direction direction = _microphoneAdaptor.GetDirection();
        switch (direction)
        {
            case Direction.Left:
                transform.position += -_speed * Time.deltaTime * transform.right;
                break;
            case Direction.Right:
                transform.position += _speed * Time.deltaTime * transform.right;
                break;
            case Direction.Stationary:
                break;
        }
    }

    private void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        transform.position = new(clampedX, transform.position.y, transform.position.z);
    }
}