using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class PaddleController : MonoBehaviour
{
    [Tooltip("The paddle movement speed.")]
    public float _speed = 8f;

    void Update()
    {
        var keyboard = Keyboard.current;
        float moveInput = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;
        transform.position += transform.up * moveInput * _speed * Time.deltaTime;
    }
}