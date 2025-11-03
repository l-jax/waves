using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class PaddleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float _topBoundary = 4f;

    [SerializeField]
    private float _bottomBoundary = -4f;

    [SerializeField]
    private float _speed = 8f;

    [Header("Components")]
    [SerializeField]
    private GameObject _ballPrefab;

    void Start()
    {
        Vector3 ballPosition = transform.position + transform.right * 1.5f;
        Instantiate(_ballPrefab, ballPosition, Quaternion.identity);
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        float moveInput = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;

        transform.position += transform.up * moveInput * _speed * Time.deltaTime;

        float clampedY = Mathf.Clamp(transform.position.y, _bottomBoundary, _topBoundary);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }
}