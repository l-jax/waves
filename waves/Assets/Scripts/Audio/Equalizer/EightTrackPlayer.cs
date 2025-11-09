using UnityEngine;
using UnityEngine.Audio;

public class EightTrackPlayer : MonoBehaviour
{
    public int[] DisplayVolumes => displayVolumes;
    [Header("Audio Setup")]
    [Tooltip("The main AudioMixer asset used to control the tracks.")]
    public AudioMixer mixer;

    [Tooltip("The 8 AudioSource components playing the tracks.")]
    public AudioSource[] trackSources = new AudioSource[8];

    private readonly string[] volumeParamNames = new string[8]
    {
        "VolTrack1", "VolTrack2", "VolTrack3", "VolTrack4",
        "VolTrack5", "VolTrack6", "VolTrack7", "VolTrack8"
    };

    private int[] displayVolumes = new int[8];
    private const float MinDB = -80f;
    private const float MaxDB = 0f;
    private const float MaxEleven = 11f;

    private void Start()
    {
        if (mixer == null)
        {
            Debug.LogError("AudioMixer not assigned. Please assign the mixer asset.");
            return;
        }
        if (trackSources.Length != 8)
        {
            Debug.LogError($"Expected 8 AudioSources, found {trackSources.Length}. Please ensure the array size is 8.");
            return;
        }

        for (int i = 0; i < trackSources.Length; i++)
        {
            if (trackSources[i] != null && trackSources[i].clip != null)
            {
                trackSources[i].Play();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < volumeParamNames.Length; i++)
        {
            if (mixer.GetFloat(volumeParamNames[i], out float currentDbVolume))
            {
                displayVolumes[i] = ConvertDbToEleven(currentDbVolume);
            }
            else
            {
                Debug.LogError($"Mixer parameter '{volumeParamNames[i]}' not found.");
            }
        }
    }

    private int ConvertDbToEleven(float dB)
    {
        float clampedDb = Mathf.Clamp(dB, MinDB, MaxDB);
        float normalizedValue = (clampedDb - MinDB) / (MaxDB - MinDB);
        int scaledValue = Mathf.RoundToInt(normalizedValue * MaxEleven);
        return scaledValue;
    }
}