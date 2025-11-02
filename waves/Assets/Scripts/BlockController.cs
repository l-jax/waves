using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BlockController : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        gameObject.SetActive(false);
    }
}