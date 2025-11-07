using UnityEngine;

[System.Serializable]
public class PitchMappingSettings
{
    [Tooltip("The LOWEST note (Hz). This will be the far-left position (0.0).")]
    public float minFrequency = 100f;

    [Tooltip("The HIGHEST note (Hz). This will be the far-right position (1.0).")]
    public float maxFrequency = 400f;

    [Tooltip("The speed at which the paddle moves to the target pitch.")]
    [Range(0.01f, 1f)]
    public float smoothingRate = 0.15f;
}

[System.Serializable]
public class MicrophoneSettings
{
    [Tooltip("The microphone device to use. Leave empty for default.")]
    public string deviceName = "";

    [Tooltip("The minimum loudness threshold to trigger pitch detection.")]
    public float pitchDetectionThreshold = 0.005f;

    [Tooltip("Sample window for loudness calculation.")]
    public int loudnessSampleWindow = 128;
}

[System.Serializable]
public class PitchDetectionSettings
{
    [Tooltip("The minimum confidence threshold for pitch detection.")]
    [Range(0f, 1f)]
    public float confidenceThreshold = 0.6f;
    
    [Tooltip("The sample window size for autocorrelation.")]
    public int sampleWindow = 2048;

    [Tooltip("Use adaptive window sizing for better balance of speed and accuracy.")]
    public bool useAdaptiveWindow = true;

    [Tooltip("The minimum frequency (in Hz) the algorithm should search for.")]
    public float minDetectionFrequency = 70f;

    [Tooltip("The maximum frequency (in Hz) the algorithm should search for.")]
    public float maxDetectionFrequency = 800f;
}

[System.Serializable]
public class JitterReductionSettings
{
    [Tooltip("Number of recent pitch samples to median filter (removes outliers).")]
    [Range(3, 11)]
    public int medianFilterSize = 5;

    [Tooltip("Maximum allowed pitch change per frame (Hz). Larger jumps are ignored.")]
    public float maxPitchChangePerFrame = 50f;

    [Tooltip("Deadzone where small movements are ignored (0-0.1 recommended).")]
    [Range(0f, 0.1f)]
    public float movementDeadzone = 0.02f;
}