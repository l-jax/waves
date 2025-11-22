using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class PaddleController : MonoBehaviour
{

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

    private bool _isMovementEnabled = true;

    void Update()
    {
        if (!_isMovementEnabled) return;

        switch (_controlSystem)
        {
            case ControlSystem.Keyboard:
                HandleKeyboardInput();
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

    private void HandleKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1 : keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0;
        transform.position += moveInput * _speed * Time.deltaTime * transform.right;
    }

    private void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary);
        transform.position = new(clampedX, transform.position.y, transform.position.z);
    }
}