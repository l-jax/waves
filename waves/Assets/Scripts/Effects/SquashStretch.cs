using System.Collections;
using UnityEngine;

public class SquashStretch : MonoBehaviour
{
    [Header("Squash & Stretch")]
    [Tooltip("How 'wide' the ball gets on squash (e.g., 1.2 = 120% width).")]
    [SerializeField]
    private float _squashAmount = 1.2f;

    [Tooltip("How 'tall' the ball gets on stretch (e.g., 1.2 = 120% height).")]
    [SerializeField]
    private float _stretchAmount = 1.2f;

    [Tooltip("How fast the ball goes from squash to stretch.")]
    [SerializeField]
    private float _squashToStretchTime = 0.08f;

    [Tooltip("How fast the ball goes from stretch back to normal.")]
    [SerializeField]
    private float _stretchToNormalTime = 0.08f;

    private Vector3 _originalScale;

    private Coroutine _squashStretchCoroutine;

    void Awake()
    {
        _originalScale = transform.localScale;
    }

    void OnCollisionEnter(Collision other)
    {
        if (_squashStretchCoroutine != null)
        {
            StopCoroutine(_squashStretchCoroutine);
        }
        _squashStretchCoroutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 squashScale = new(_originalScale.x * _squashAmount, _originalScale.y / _squashAmount, _originalScale.z);
        Vector3 stretchScale = new(_originalScale.x / _stretchAmount, _originalScale.y * _stretchAmount, _originalScale.z);

        float timer = 0f;

        transform.localScale = squashScale;

        while (timer < _squashToStretchTime)
        {
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, timer / _squashToStretchTime);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = stretchScale;

        while (timer < _stretchToNormalTime)
        {
            transform.localScale = Vector3.Lerp(stretchScale, _originalScale, timer / _stretchToNormalTime);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _originalScale;
        _squashStretchCoroutine = null;
    }
}