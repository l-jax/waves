using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;

    void Start()
    {
        _originalPos = transform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
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