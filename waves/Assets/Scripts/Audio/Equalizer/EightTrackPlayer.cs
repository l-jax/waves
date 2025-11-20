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
    
    [Tooltip("Minimum volume multiplier when one block is revealed (0-1). e.g., 0.3 = 30% volume at 1 block.")]
    [SerializeField] private float _minVolumeMultiplier = 0.5f;
    
    [Tooltip("Curve exponent for volume scaling. 1.0=linear, 2.0=quadratic, 0.5=square root.")]
    [SerializeField] private float _volumeCurveExponent = 1.0f;

    private readonly string[] volumeParamNames = new string[8]
    {
        "VolTrack1", "VolTrack2", "VolTrack3", "VolTrack4",
        "VolTrack5", "VolTrack6", "VolTrack7", "VolTrack8"
    };

    private Equalizer _equalizer;
    
    private const float MinDB = -80f;
    private const float MaxDB = 0f;
    private const float MaxBlocks = 12f;

    private bool _isPlaying = false;

    void Start()
    {
        _equalizer = GetComponent<Equalizer>();
    }

    public void Play()
    {
        foreach (var source in _trackSources)
        {
            source.Play();
        }
        _isPlaying = true;
    }

    public void Pause()
    {
        foreach (var source in _trackSources)
        {
            source.Pause();
        }
        _isPlaying = false;
    }

    void Update()
    {
        if (!_isPlaying) return;
        int[] maxRevealedHeights = _equalizer.GetMaxRevealedHeights();

        for (int i = 0; i < volumeParamNames.Length; i++)
        {
            if (_mixer.GetFloat(volumeParamNames[i], out _))
            {
                // if no blocks are revealed, silence the track
                if (maxRevealedHeights[i] == 0)
                {
                    _mixer.SetFloat(volumeParamNames[i], MinDB);
                    continue;
                }
                
                float maxAllowed = maxRevealedHeights[i];
                
                float revealPercent = (maxAllowed - 1f) / (MaxBlocks - 1f);

                float curvedPercent = Mathf.Pow(revealPercent, _volumeCurveExponent);
                float volumeMultiplier = Mathf.Lerp(_minVolumeMultiplier, 1.0f, curvedPercent);
                float scaledValue = MaxBlocks * volumeMultiplier;
                
                float targetDb = ConvertBlocksToDb(scaledValue);                
                _mixer.SetFloat(volumeParamNames[i], targetDb);
            }
            else
            {
                Debug.LogError($"Mixer parameter '{volumeParamNames[i]}' not found.");
            }
        }
    }

    private float ConvertBlocksToDb(float blocksValue)
    {
        float clampedBlocks = Mathf.Clamp(blocksValue, 0, MaxBlocks);
        float dB = MinDB + (clampedBlocks * ((MaxDB - MinDB) / MaxBlocks));
        return dB;
    }
}