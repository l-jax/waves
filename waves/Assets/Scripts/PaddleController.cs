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

    [Tooltip("The paddle's maximum movement speed.")]
    [SerializeField]
    private float _speed = 10f;

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

        transform.position += transform.right * moveInput * 10f * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void MovePaddle(float normalizedTarget)
    {
        float targetXPosition = Mathf.Lerp(_leftBoundary, _rightBoundary, normalizedTarget);

        Vector3 targetPos = new (targetXPosition, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, _speed * Time.deltaTime);
    }
}