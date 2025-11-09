using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BlockController : MonoBehaviour
{
    private EffectsPlayer _effectsPlayer;

    void Start()
    {
        _effectsPlayer = GameObject.Find("EffectsPlayer").GetComponent<EffectsPlayer>();
    }

    void OnCollisionEnter(Collision other)
    {
        _effectsPlayer.PlayEffect(Effect.Break, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}