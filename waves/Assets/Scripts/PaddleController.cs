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

    [Header("Visuals")]
    [SerializeField]
    private GameObject _ballPrefab;

    private Tracker _tracker;

    void Start()
    {
        Vector3 ballPosition = transform.position + transform.up * 1.5f;
        Instantiate(_ballPrefab, ballPosition, Quaternion.identity);

        _tracker = GameObject.Find("Microphone").GetComponent<Tracker>();
        if (_tracker == null)
        {
            Debug.LogError("No Tracker found in the scene");
        }
    }

    void Update()
    {
        if (_debugMode)
        {
            HandleDebugInput();
            return;
        }
        
        if (_tracker == null) return;

        float normalizedTarget = _tracker.NormalizedPosition;

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
        transform.position = targetPos;
    }
}