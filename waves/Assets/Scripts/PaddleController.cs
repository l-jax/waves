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

    [Tooltip("Use additional smoothing in paddle movement (disable for direct control).")]
    [SerializeField]
    private bool _usePaddleSmoothing = false;

    [Tooltip("The paddle's smoothing speed (only used if smoothing is enabled).")]
    [SerializeField]
    private float _speed = 15f;

    [Header("Visuals")]
    [SerializeField]
    private GameObject _ballPrefab;

    private PitchTracker _pitchTracker;

    void Start()
    {
        Vector3 ballPosition = transform.position + transform.up * 1.5f;
        Instantiate(_ballPrefab, ballPosition, Quaternion.identity);

        _pitchTracker = GameObject.Find("Microphone").GetComponent<PitchTracker>();
        if (_pitchTracker == null)
        {
            Debug.LogError("No PitchTracker found in the scene");
        }
    }

    void Update()
    {
        if (_debugMode)
        {
            HandleDebugInput();
            return;
        }
        
        if (_pitchTracker == null) return;

        float normalizedTarget = _pitchTracker.NormalizedPosition;

        MovePaddle(normalizedTarget);
    }

    private void HandleDebugInput()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.aKey.isPressed ? -1 : keyboard.dKey.isPressed ? 1 : 0;

        transform.position += moveInput * 10f * Time.deltaTime * transform.right;

        float clampedX = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        transform.position = new(clampedX, transform.position.y, transform.position.z);
    }

    private void MovePaddle(float normalizedTarget)
    {
        float targetXPosition = Mathf.Lerp(_leftBoundary, _rightBoundary, normalizedTarget);
        Vector3 targetPos = new(targetXPosition, transform.position.y, transform.position.z);

        if (_usePaddleSmoothing)
        {
            // Additional smoothing layer (can add lag)
            transform.position = Vector3.Lerp(transform.position, targetPos, _speed * Time.deltaTime);
        }
        else
        {
            // Direct control - PitchTracker handles all smoothing
            transform.position = targetPos;
        }
    }
}