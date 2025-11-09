using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BlockController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _breakParticles;

    private EffectsPlayer _effectsPlayer;

    void Start()
    {
        _effectsPlayer = GameObject.Find("EffectsPlayer").GetComponent<EffectsPlayer>();
    }

    void OnCollisionEnter(Collision other)
    {
        _effectsPlayer.PlaySound(SoundEffect.Break);
        if (_breakParticles == null)
        {
            Debug.LogWarning("Break particles not assigned.");
        } else
        {
            Instantiate(_breakParticles, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }
}