using UnityEngine;

public class MicrophoneInput(AudioSource audioSource, MicrophoneSettings settings)
{
    private readonly AudioSource _audioSource = audioSource;
    private readonly MicrophoneSettings _settings = settings;
    private AudioClip _microphoneClip;
    private string _currentDevice;
    public int SampleRate { get; private set; }

    public void Initialize()
    {
        _currentDevice = string.IsNullOrEmpty(_settings.deviceName)
            ? Microphone.devices[0]
            : _settings.deviceName;

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
        SampleRate = _microphoneClip.frequency;

        if (_microphoneClip == null)
        {
            Debug.LogError($"Could not start microphone device: {_currentDevice}");
            return;
        }

        _audioSource.clip = _microphoneClip;
        _audioSource.loop = true;
        _audioSource.mute = true;

        while (!(Microphone.GetPosition(_currentDevice) > 0)) { }
        _audioSource.Play();
    }

    public float GetLoudness()
    {
        if (_microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(_currentDevice);
        if (micPosition < _settings.loudnessSampleWindow) return 0f;

        float[] buffer = new float[_settings.loudnessSampleWindow];
        _microphoneClip.GetData(buffer, micPosition - _settings.loudnessSampleWindow);

        float totalLoudness = 0f;
        for (int i = 0; i < _settings.loudnessSampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(buffer[i]);
        }

        return totalLoudness / _settings.loudnessSampleWindow;
    }

    public void GetAudioData(float[] buffer, int sampleCount)
    {
        if (_microphoneClip == null) return;

        int micPosition = Microphone.GetPosition(_currentDevice);
        if (micPosition < sampleCount) return;

        _microphoneClip.GetData(buffer, micPosition - sampleCount);
    }

    public int GetCurrentPosition()
    {
        return Microphone.GetPosition(_currentDevice);
    }
}
