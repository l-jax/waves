using System.Collections.Generic;
using UnityEngine;

public class PitchFilter
{
    private readonly JitterReductionSettings _settings;
    private readonly Queue<float> _recentPitches;
    private float _lastValidPitch;

    public PitchFilter(JitterReductionSettings settings)
    {
        _settings = settings;
        _recentPitches = new Queue<float>();
        _lastValidPitch = 0f;
    }

    public void AddSample(float pitch)
    {
        if (pitch == 0f) return;

        if (_lastValidPitch > 0 && _recentPitches.Count > 0)
        {
            float pitchDiff = Mathf.Abs(pitch - _lastValidPitch);
            if (pitchDiff > _settings.maxPitchChangePerFrame)
            {
                return;
            }
        }

        _recentPitches.Enqueue(pitch);
        if (_recentPitches.Count > _settings.medianFilterSize)
        {
            _recentPitches.Dequeue();
        }
        
        _lastValidPitch = pitch;
    }

    public float GetFilteredPitch()
    {
        if (_recentPitches.Count == 0) return 0f;

        List<float> pitchList = new(_recentPitches);
        pitchList.Sort();

        int middleIndex = pitchList.Count / 2;
        if (pitchList.Count % 2 == 0)
        {
            return (pitchList[middleIndex - 1] + pitchList[middleIndex]) / 2f;
        }
        
        return pitchList[middleIndex];
    }
}