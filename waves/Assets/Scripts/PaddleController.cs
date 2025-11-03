using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class PaddleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float _leftBoundary = 4f;

    [SerializeField]
    private float _rightBoundary = -4f;

    [SerializeField]
    private float _speed = 8f;

    [Header("Components")]
    [SerializeField]
    private GameObject _ballPrefab;

    void Start()
    {
        Vector3 ballPosition = transform.position + transform.up * 1.5f;
        Instantiate(_ballPrefab, ballPosition, Quaternion.identity);
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.aKey.isPressed ? -1 : keyboard.dKey.isPressed ? 1 : 0;

        transform.position += transform.right * moveInput * _speed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, _rightBoundary, _leftBoundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}