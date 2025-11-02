using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchTracker : MonoBehaviour
{
    [Tooltip("The microphone device to use. Leave empty for default.")]
    public string _microphoneDevice;

    [Tooltip("The sample window size for the loudness calculation.")]
    public int _loudnessSampleWindow = 128;

    [Header("FFT Pitch Tracking (Method 1)")]
    [Tooltip("The size of the FFT window. Must be a power of 2 (e.g., 512, 1024, 2048).")]
    public int _fftSize = 2048;

    [Tooltip("The minimum loudness threshold to trigger pitch detection.")]
    public float _pitchDetectionThreshold = 0.02f;

    [Header("Autocorrelation Pitch Tracking (Method 2)")]
    [Tooltip("The sample window size for autocorrelation. Larger windows are better for low frequencies.")]
    public int _autocorrelationSampleWindow = 2048;

    [Tooltip("The minimum frequency (in Hz) to detect. Clamps the search range.")]
    public float _minFrequency = 70f;

    [Tooltip("The maximum frequency (in Hz) to detect. Clamps the search range.")]
    public float _maxFrequency = 800f;

    private AudioSource _audioSource;
    private AudioClip _microphoneClip;
    private string _currentMicrophoneDevice;
    private float[] _micSampleBuffer;
    private float[] _spectrumData;
    private float[] _autocorrelationBuffer;
    private int _sampleRate;

    public float CurrentLoudness { get; private set; }
    public float CurrentPitch_FFT { get; private set; }
    public float CurrentPitch_Autocorrelation { get; private set; }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // Initialize buffers
        _micSampleBuffer = new float[_autocorrelationSampleWindow]; // Use the largest window
        _spectrumData = new float[_fftSize];
        _autocorrelationBuffer = new float[_autocorrelationSampleWindow];

        // Start microphone
        StartMicrophone();
    }

    void StartMicrophone()
    {
        // Use default device if none specified
        _currentMicrophoneDevice = string.IsNullOrEmpty(_microphoneDevice) ? Microphone.devices[0] : _microphoneDevice;

        // Stop any existing mic
        if (Microphone.IsRecording(_currentMicrophoneDevice))
        {
            Microphone.End(_currentMicrophoneDevice);
        }

        // Start the microphone, looping.
        // We set a 1-second clip length and it will loop.
        // The sample rate is crucial.
        _microphoneClip = Microphone.Start(_currentMicrophoneDevice, true, 1, AudioSettings.outputSampleRate);
        _sampleRate = _microphoneClip.frequency;

        if (_microphoneClip == null)
        {
            Debug.LogError($"Could not start microphone device: {_currentMicrophoneDevice}");
            return;
        }

        // --- Setup for GetSpectrumData (FFT Method) ---
        // We must play the AudioClip from an AudioSource to use GetSpectrumData.
        // We can mute the AudioSource if we don't want to hear the raw mic input.
        _audioSource.clip = _microphoneClip;
        _audioSource.loop = true;
        _audioSource.mute = true; // Mute if you don't want to hear the mic playback

        // Wait until the microphone starts recording
        while (!(Microphone.GetPosition(_currentMicrophoneDevice) > 0))
        {
            // Wait...
        }

        Debug.Log("Microphone started successfully.");
        _audioSource.Play();
    }

    void Update()
    {
        if (!Microphone.IsRecording(_currentMicrophoneDevice))
        {
            StartMicrophone(); // Restart if it gets disconnected
            return;
        }

        // --- 1. Get Loudness ---
        CurrentLoudness = GetMicrophoneLoudness();

        // --- 2. Get Pitch (FFT Method) ---
        // Only process if we're loud enough
        if (CurrentLoudness > _pitchDetectionThreshold)
        {
            CurrentPitch_FFT = GetMicrophonePitch_FFT();
            CurrentPitch_Autocorrelation = GetMicrophonePitch_Autocorrelation();
        }
        else
        {
            CurrentPitch_FFT = 0f;
            CurrentPitch_Autocorrelation = 0f;
        }

        Debug.Log($"Loudness: {CurrentLoudness:F4}, Pitch FFT: {CurrentPitch_FFT:F2} Hz, Pitch Autocorr: {CurrentPitch_Autocorrelation:F2} Hz");
    }

    /// <summary>
    /// This is the method from your sample, updated to use the class-level clip.
    /// It calculates the average amplitude (loudness) of the signal.
    /// </summary>
    private float GetMicrophoneLoudness()
    {
        if (_microphoneClip == null) return 0f;

        // Get the current position in the clip
        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        if (micPosition < _loudnessSampleWindow) return 0f;

        // We want the most recent data, so we go backwards from the current position
        int startSample = micPosition - _loudnessSampleWindow;

        // Ensure we have enough data in our buffer
        if (_micSampleBuffer.Length < _loudnessSampleWindow)
        {
            _micSampleBuffer = new float[_loudnessSampleWindow];
        }

        // Get the data
        _microphoneClip.GetData(_micSampleBuffer, startSample);

        float totalLoudness = 0f;
        for (int i = 0; i < _loudnessSampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(_micSampleBuffer[i]);
        }
        return totalLoudness / _loudnessSampleWindow;
    }

    /// <summary>
    /// METHOD 1: Finds the fundamental frequency (pitch) using FFT.
    /// This method requires the AudioSource to be playing the clip.
    /// </summary>
    private float GetMicrophonePitch_FFT()
    {
        if (_audioSource == null) return 0f;

        // Get the frequency spectrum data
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);

        float maxSpectrumAmplitude = 0f;
        int peakIndex = 0;

        // Find the peak in the spectrum
        for (int i = 0; i < _fftSize; i++)
        {
            if (_spectrumData[i] > maxSpectrumAmplitude)
            {
                maxSpectrumAmplitude = _spectrumData[i];
                peakIndex = i;
            }
        }

        // Convert the peak index into a frequency
        // The frequency at a given index 'i' is: f = i * (sampleRate / 2) / fftSize
        float fundamentalFrequency = (float)peakIndex * (_sampleRate / 2f) / (float)_fftSize;

        return fundamentalFrequency;
    }

    /// <summary>
    /// METHOD 2: Finds the fundamental frequency (pitch) using Autocorrelation.
    /// This works on the raw sample data.
    /// </summary>
    private float GetMicrophonePitch_Autocorrelation()
    {
        if (_microphoneClip == null) return 0f;

        // --- 1. Get the raw sample data ---
        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        if (micPosition < _autocorrelationSampleWindow) return 0f;

        int startSample = micPosition - _autocorrelationSampleWindow;

        // Ensure buffer is large enough
        if (_micSampleBuffer.Length < _autocorrelationSampleWindow)
        {
            _micSampleBuffer = new float[_autocorrelationSampleWindow];
        }
        _microphoneClip.GetData(_micSampleBuffer, startSample);


        // --- 2. Perform Autocorrelation ---
        // The autocorrelation function measures the similarity of the signal
        // with a delayed copy of itself, at different time lags (delays).
        // The lag that gives the highest correlation corresponds to the
        // signal's fundamental period.

        float maxCorrelation = 0f;
        int bestLag = 0;

        // Define the search range in samples, based on min/max frequency
        int minLag = (int)(_sampleRate / _maxFrequency); // e.g., 44100 / 800 = ~55 samples
        int maxLag = (int)(_sampleRate / _minFrequency); // e.g., 44100 / 70  = ~630 samples

        // Ensure we don't go out of bounds
        maxLag = Mathf.Min(maxLag, _autocorrelationSampleWindow - 1);

        // We only search within the 'valid' lag range.
        // We start at minLag because lag 0 is always 100% correlated (signal with itself)
        // and very small lags are just noise.
        for (int lag = minLag; lag < maxLag; lag++)
        {
            float correlation = 0f;

            // Calculate correlation for this lag
            for (int i = 0; i < _autocorrelationSampleWindow - lag; i++)
            {
                correlation += _micSampleBuffer[i] * _micSampleBuffer[i + lag];
            }

            // Store the correlation result (optional, good for debugging)
            // _autocorrelationBuffer[lag] = correlation;

            // Find the *first* (and hopefully highest) peak
            if (correlation > maxCorrelation)
            {
                maxCorrelation = correlation;
                bestLag = lag;
            }
        }

        // --- 3. Convert Lag to Frequency ---
        // If we found a significant peak, convert it to frequency
        if (bestLag == 0) return 0f;

        // Frequency = SampleRate / Period (in samples)
        float fundamentalFrequency = (float)_sampleRate / (float)bestLag;

        return fundamentalFrequency;
    }
}
