using UnityEngine;

public class MicrophoneInput
{
    private const int _sampleWindow = 256;
    private readonly AudioSource _audioSource;
    private readonly float[] _sampleBuffer;
    private AudioClip _microphoneClip;
    private string _currentDevice;

    public MicrophoneInput(AudioSource audioSource)
    {
        _audioSource = audioSource;
        _sampleBuffer = new float[_sampleWindow];
    }

    public void Initialize()
    {
        _currentDevice = Microphone.devices[0];
        StartRecording();
    }

    public void Update()
    {
        if (!Microphone.IsRecording(_currentDevice))
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        if (Microphone.IsRecording(_currentDevice))
        {
            Microphone.End(_currentDevice);
        }

        _microphoneClip = Microphone.Start(_currentDevice, true, 1, AudioSettings.outputSampleRate);

        if (_microphoneClip == null)
        {
            Debug.LogError($"Could not start microphone: {_currentDevice}");
            return;
        }

        _audioSource.clip = _microphoneClip;
        _audioSource.loop = true;
        _audioSource.mute = true;

        // Wait for microphone to start
        while (!(Microphone.GetPosition(_currentDevice) > 0)) { }
        _audioSource.Play();
    }

    public float GetVolume()
    {
        if (_microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(_currentDevice);
        if (micPosition < _sampleWindow) return 0f;

        _microphoneClip.GetData(_sampleBuffer, micPosition - _sampleWindow);

        float sum = 0f;
        for (int i = 0; i < _sampleWindow; i++)
        {
            sum += Mathf.Abs(_sampleBuffer[i]);
        }

        return sum / _sampleWindow;
    }
}