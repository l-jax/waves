using UnityEngine;
using System;

[Serializable]
public class ControlSettings
{
    [Tooltip("Silence threshold - below this, paddle stops")]
    [Range(0f, 0.1f)]
    public float SilenceThreshold = 0.005f;

    [Tooltip("Midpoint - sounds below move LEFT, above move RIGHT")]
    [Range(0f, 0.5f)]
    public float MidpointLoudness = 0.05f;

    [Tooltip("Maximum loudness for speed scaling")]
    [Range(0f, 1f)]
    public float MaxLoudness = 0.2f;
}

[Serializable]
public class MicrophoneSettings
{
    [Tooltip("Microphone device name (leave empty for default)")]
    public string DeviceName = "";

    [Tooltip("Sample window size for loudness calculation")]
    public int SampleWindow = 256;
}
