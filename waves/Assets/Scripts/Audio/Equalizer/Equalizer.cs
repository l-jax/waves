using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EightTrackPlayer))]
[RequireComponent(typeof(BlockTracker))]
public class Equalizer : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundModel;
    [SerializeField] private AudioSource[] _eightTrackSources = new AudioSource[8];
    
    private MicrophoneAdaptor _microphoneAdaptor;
    private GameObject[][] _tracks;
    private BlockTracker _blockTracker;

    private readonly float[] _currentVisualHeight = new float[8];
    private readonly float[] _targetHeight = new float[8];

    public void Awake()
    {
        _microphoneAdaptor = FindFirstObjectByType<MicrophoneAdaptor>();
        _blockTracker = GetComponent<BlockTracker>();
    }

    public void Start()
    {
        _blockTracker.Initialize();

        // ignore the back plate
        _tracks = new GameObject[_backgroundModel.transform.childCount - 1][];

        for (int i = 0; i < _tracks.Length; i++)
        {
            _tracks[i] = new GameObject[_backgroundModel.transform.GetChild(i + 1).childCount];
            for (int j = 0; j < _tracks[i].Length; j++)
            {
                _tracks[i][j] = _backgroundModel.transform.GetChild(i + 1).GetChild(j).gameObject;
            }
        }
    }

    public void SetControlSystem(ControlSystem controlSystem)
    {
        StopAllCoroutines();
        switch (controlSystem)
        {
            case ControlSystem.Keyboard:
                StartCoroutine(DisplayEightTrackDataCoroutine());
                break;
            case ControlSystem.Voice:
                StartCoroutine(DisplayMicrophoneDataCoroutine());
                break;
        }
    }

    private IEnumerator DisplayEightTrackDataCoroutine()
    {
        while (true)
        {
            float[] bandPowers = GetEightTrackAmplitudes();
            DisplayBars(bandPowers);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private float[] GetEightTrackAmplitudes()
    {
        float[] amplitudes = new float[8];
        
        for (int i = 0; i < 8; i++)
        {
            if (_eightTrackSources != null && i < _eightTrackSources.Length && _eightTrackSources[i] != null)
            {
                amplitudes[i] = GetAudioSourceAmplitude(_eightTrackSources[i]);
            }
            else
            {
                amplitudes[i] = 0f;
            }
        }
        
        return amplitudes;
    }

    private float GetAudioSourceAmplitude(AudioSource source)
    {
        if (source.clip == null || !source.isPlaying)
        {
            return 0f;
        }

        float[] samples = new float[256];
        int samplePosition = source.timeSamples;
        
        if (samplePosition < 256)
        {
            return 0f;
        }
        
        source.clip.GetData(samples, samplePosition - 256);

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        float rms = Mathf.Sqrt(sum / samples.Length);
        return Mathf.Clamp01(rms * 5f);
    }

    private IEnumerator DisplayMicrophoneDataCoroutine()
    {
        while (true)
        {
            float[] bandPowers = _microphoneAdaptor.GetEightBandSpectrum();
            DisplayBars(bandPowers);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void DisplayBars(float[] bandPowers)
    {
        for (int i = 0; i < _tracks.Length; i++)
        {
            _targetHeight[i] = bandPowers[i];

            if (_currentVisualHeight[i] < _targetHeight[i])
            {
                // Instant rise
                _currentVisualHeight[i] = _targetHeight[i];
            }
            else
            {
                // Exponential decay
                float decayRate = 0.95f;
                _currentVisualHeight[i] *= decayRate;

                // Clamp to target if very close
                if (_currentVisualHeight[i] < _targetHeight[i] + 0.01f)
                {
                    _currentVisualHeight[i] = _targetHeight[i];
                }
            }

            // Get the maximum revealed height for this track
            int maxRevealedHeight = _blockTracker.GetMaxRevealedHeight(i);
            
            // Calculate the index, but cap it at the maximum revealed height
            int idx = Mathf.CeilToInt(_currentVisualHeight[i] * (_tracks[i].Length - 1));
            idx = Mathf.Clamp(idx, 0, Mathf.Min(maxRevealedHeight, _tracks[i].Length - 1));

            for (int j = 0; j < _tracks[i].Length; j++)
            {
                _tracks[i][j].SetActive(j <= idx);
            }
        }
    }

    public int[] GetMaxRevealedHeights()
    {
        if (_blockTracker != null)
        {
            return _blockTracker.GetAllMaxRevealedHeights();
        }
        return new int[8];
    }
}