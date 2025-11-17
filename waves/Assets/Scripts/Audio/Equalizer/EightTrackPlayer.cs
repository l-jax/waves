using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Equalizer))]
public class EightTrackPlayer : MonoBehaviour
{
    [Header("Audio Setup")]
    [Tooltip("The main AudioMixer asset used to control the tracks.")]
    [SerializeField] private AudioMixer _mixer;

    [Tooltip("The 8 AudioSource components playing the tracks.")]
    [SerializeField] private AudioSource[] _trackSources = new AudioSource[8];

    [Header("Volume Control")]
    [Tooltip("The base volume parameters that the user controls (0-11 scale).")]
    [SerializeField] private float[] _userVolumes = new float[8] { 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f };

    private readonly string[] volumeParamNames = new string[8]
    {
        "VolTrack1", "VolTrack2", "VolTrack3", "VolTrack4",
        "VolTrack5", "VolTrack6", "VolTrack7", "VolTrack8"
    };

    private readonly int[] _displayVolumes = new int[8];
    private Equalizer _equalizer;
    
    private const float MinDB = -80f;
    private const float MaxDB = 0f;
    private const float MaxEleven = 11f;

    private void Start()
    {
        _equalizer = GetComponent<Equalizer>();

        for (int i = 0; i < _trackSources.Length; i++)
        {
            if (_trackSources[i] != null && _trackSources[i].clip != null)
            {
                _trackSources[i].Play();
            }
        }
    }

    private void Update()
    {
        int[] maxRevealedHeights = _equalizer.GetMaxRevealedHeights();

        for (int i = 0; i < volumeParamNames.Length; i++)
        {
            if (_mixer.GetFloat(volumeParamNames[i], out _))
            {
                float desiredElevenValue = _userVolumes[i];
                
                if (maxRevealedHeights != null && i < maxRevealedHeights.Length)
                {
                    float maxAllowedElevenValue = maxRevealedHeights[i];
                    float scaledElevenValue = desiredElevenValue * (maxAllowedElevenValue / MaxEleven);
                    
                    // Convert to dB
                    float targetDb = ConvertElevenToDb(scaledElevenValue);
                    
                    // Apply to mixer
                    _mixer.SetFloat(volumeParamNames[i], targetDb);
                    
                    _displayVolumes[i] = Mathf.RoundToInt(scaledElevenValue);
                }
                else
                {
                    // No block tracker, use full volume
                    float targetDb = ConvertElevenToDb(desiredElevenValue);
                    _mixer.SetFloat(volumeParamNames[i], targetDb);
                    _displayVolumes[i] = Mathf.RoundToInt(desiredElevenValue);
                }
            }
            else
            {
                Debug.LogError($"Mixer parameter '{volumeParamNames[i]}' not found.");
            }
        }
    }
    
    private float ConvertElevenToDb(float elevenValue)
    {
        float clampedEleven = Mathf.Clamp(elevenValue, 0, MaxEleven);
        float normalizedValue = clampedEleven / MaxEleven;
        float dB = MinDB + (normalizedValue * (MaxDB - MinDB));
        return dB;
    }
}