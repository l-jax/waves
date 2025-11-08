using UnityEngine;

public class PitchDetector(PitchDetectionSettings settings)
{
    private readonly PitchDetectionSettings _settings = settings;
    private float[] _sampleBuffer = new float[settings.sampleWindow];

    public float DetectPitch(MicrophoneInput microphone)
    {
        int effectiveWindow = GetEffectiveWindowSize();
        
        if (microphone.GetCurrentPosition() < effectiveWindow) return 0f;

        if (_sampleBuffer.Length < effectiveWindow)
        {
            _sampleBuffer = new float[effectiveWindow];
        }

        microphone.GetAudioData(_sampleBuffer, effectiveWindow);

        int bestLag = FindBestAutocorrelationLag(effectiveWindow, microphone.SampleRate);
        
        return bestLag > 0 ? (float)microphone.SampleRate / bestLag : 0f;
    }

    private int GetEffectiveWindowSize()
    {
        return _settings.sampleWindow;
    }

    private int FindBestAutocorrelationLag(int windowSize, int sampleRate)
    {
        float maxCorrelation = 0f;
        int bestLag = 0;

        int minLag = (int)(sampleRate / _settings.maxDetectionFrequency);
        int maxLag = (int)(sampleRate / _settings.minDetectionFrequency);
        maxLag = Mathf.Min(maxLag, windowSize - 1);

        for (int lag = minLag; lag < maxLag; lag++)
        {
            float correlation = CalculateNormalizedAutocorrelation(lag, windowSize);
            
            if (correlation > maxCorrelation)
            {
                maxCorrelation = correlation;
                bestLag = lag;
            }
        }

        return maxCorrelation >= _settings.confidenceThreshold ? bestLag : 0;
    }

    private float CalculateNormalizedAutocorrelation(int lag, int windowSize)
    {
        float correlation = 0f;
        float energy = 0f;
        
        for (int i = 0; i < windowSize - lag; i++)
        {
            correlation += _sampleBuffer[i] * _sampleBuffer[i + lag];
            energy += _sampleBuffer[i] * _sampleBuffer[i] + 
                     _sampleBuffer[i + lag] * _sampleBuffer[i + lag];
        }
        
        return energy > float.Epsilon ? correlation / (energy * 0.5f) : 0f;
    }
}