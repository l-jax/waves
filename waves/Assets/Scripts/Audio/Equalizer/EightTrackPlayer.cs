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
            if (_mixer.GetFloat(volumeParamNames[i], out float currentDbVolume))
            {
                float limitedDbVolume = currentDbVolume;
                
                if (maxRevealedHeights != null && i < maxRevealedHeights.Length)
                {
                    // Convert the max revealed height (0-11) to a max dB value
                    float maxAllowedDb = ConvertElevenToDb(maxRevealedHeights[i]);
                    
                    // Clamp the current volume to not exceed the revealed maximum
                    limitedDbVolume = Mathf.Min(currentDbVolume, maxAllowedDb);
                    
                    // Set the limited volume back to the mixer
                    if (limitedDbVolume != currentDbVolume)
                    {
                        _mixer.SetFloat(volumeParamNames[i], limitedDbVolume);
                    }
                }
                
                _displayVolumes[i] = ConvertDbToEleven(limitedDbVolume);
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

    private float ConvertElevenToDb(int elevenValue)
    {
        float clampedEleven = Mathf.Clamp(elevenValue, 0, MaxEleven);
        float normalizedValue = clampedEleven / MaxEleven;
        float dB = MinDB + (normalizedValue * (MaxDB - MinDB));
        return dB;
    }
}