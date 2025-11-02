using UnityEngine;
public class MicrophoneController : MonoBehaviour
{
    private bool _isEnabled = false;
    private string _microphoneDevice;
    private AudioClip _microphoneClip;
        
    void Start()
    {
        _microphoneDevice = Microphone.devices[0];
        _microphoneClip = Microphone.Start(_microphoneDevice, true, 10, 44100);
        _isEnabled = true;
    }

    void OnDisable()
    {
        Microphone.End(_microphoneDevice);
    }

    void Update()
    {
        if (!_isEnabled) return;

        float loudness = GetMicrophoneLoudness();
        Debug.Log("Microphone Loudness: " + loudness);
    }

    private float GetMicrophoneLoudness()
    {
        int sampleWindow = 128;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(_microphoneDevice) - (sampleWindow + 1);
        if (micPosition < 0) return 0f;
        _microphoneClip.GetData(waveData, micPosition);

        float totalLoudness = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        return totalLoudness / sampleWindow;
    }
}