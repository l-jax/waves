using UnityEngine;
using System.Collections;

public enum ShakeType
{
    Small,
    Large
}

public class CameraController : MonoBehaviour
{
    [Tooltip("Camera shake duration on bounce.")]
    [SerializeField]
    private float _smallShakeDuration = 0.05f;

    [Tooltip("Camera shake magnitude on bounce.")]
    [SerializeField]
    private float _smallShakeMagnitude = 0.05f;

    [Tooltip("Camera shake duration on block break.")]
    [SerializeField]
    private float _largeShakeDuration = 0.1f;

    [Tooltip("Camera shake magnitude on block break.")]
    [SerializeField]
    private float _largeShakeMagnitudes = 0.1f;

    private Vector3 _originalPos = new(0, 1, -10);
    private Coroutine _shakeCoroutine;

    public void Shake(ShakeType size)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        switch (size)
        {
            case ShakeType.Small:
                _shakeCoroutine = StartCoroutine(Shake(_smallShakeDuration, _smallShakeMagnitude));
                break;
            case ShakeType.Large:
                _shakeCoroutine = StartCoroutine(Shake(_largeShakeDuration, _largeShakeMagnitudes));
                break;
        }
    }

    public void StopShake()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        transform.localPosition = _originalPos;
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float decay = 1.0f - (elapsedTime / duration);

            Vector2 randomOffset = Random.insideUnitCircle;


            transform.localPosition = _originalPos + new Vector3(
                randomOffset.x * magnitude * decay,
                randomOffset.y * magnitude * decay,
                0f
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originalPos;
        _shakeCoroutine = null;
    }
}