using UnityEngine;

public class Input
{
    private readonly AudioSource _audioSource;
    private readonly MicrophoneSettings _settings;
    private readonly float[] _sampleBuffer;
    private AudioClip _microphoneClip;
    private string _currentDevice;

    public Input(AudioSource audioSource, MicrophoneSettings settings)
    {
        _audioSource = audioSource;
        _settings = settings;
        _sampleBuffer = new float[settings.SampleWindow];
    }

    public void Initialize()
    {
        _currentDevice = string.IsNullOrEmpty(_settings.DeviceName)
            ? Microphone.devices[0]
            : _settings.DeviceName;

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
        if (micPosition < _settings.SampleWindow) return 0f;

        _microphoneClip.GetData(_sampleBuffer, micPosition - _settings.SampleWindow);

        float sum = 0f;
        for (int i = 0; i < _settings.SampleWindow; i++)
        {
            sum += Mathf.Abs(_sampleBuffer[i]);
        }

        return sum / _settings.SampleWindow;
    }
}