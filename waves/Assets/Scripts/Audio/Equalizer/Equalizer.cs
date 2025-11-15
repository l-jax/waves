using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EightTrackPlayer))]
public class Equalizer : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundModel;
    private MicrophoneAdaptor _microphoneAdaptor;
    private GameObject[][] _tracks;
    private EightTrackPlayer _eightTrackPlayer;

    private readonly float[] _currentVisualHeight = new float[8];
    private readonly float[] _targetHeight = new float[8];

    public void Awake()
    {
        _microphoneAdaptor = FindFirstObjectByType<MicrophoneAdaptor>();
    }

    public void Start()
    {
        if (_backgroundModel == null)
        {
            Debug.LogError("Background model is not assigned.");
            return;
        }

        _eightTrackPlayer = GetComponent<EightTrackPlayer>();
        if (_eightTrackPlayer == null)
        {
            Debug.LogError("No EightTrackPlayer found in the scene");
        }

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
            int[] displayVolumes = _eightTrackPlayer.DisplayVolumes;
            if (_tracks == null || _tracks.Length == 0) yield return null;

            for (int i = 0; i < _tracks.Length; i++)
            {
                for (int j = 0; j < _tracks[i].Length; j++)
                {
                    _tracks[i][j].SetActive(j <= displayVolumes[i]);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator DisplayMicrophoneDataCoroutine()
    {
        while (true)
        {
            float[] bandPowers = _microphoneAdaptor.GetEightBandSpectrum();
        
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
                
                int idx = Mathf.CeilToInt(_currentVisualHeight[i] * (_tracks[i].Length - 1));
                idx = Mathf.Clamp(idx, 0, _tracks[i].Length - 1);
                
                for (int j = 0; j < _tracks[i].Length; j++)
                {
                    _tracks[i][j].SetActive(j <= idx);
                }
            }
            yield return new WaitForSeconds(0.1f); 
        }
    }
}