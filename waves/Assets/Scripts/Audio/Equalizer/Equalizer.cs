using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EightTrackPlayer))]
public class Equalizer : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundModel;
    private MicrophoneAdaptor _microphoneAdaptor;
    private GameObject[][] _tracks;
    private EightTrackPlayer _eightTrackPlayer;

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

        //StartCoroutine(RandomizeBarsCoroutine());
        StartCoroutine(UpdateBarsCoroutine());
    }

    public IEnumerator RandomizeBarsCoroutine()
    {
        while (true)
        {
            if (_tracks == null || _tracks.Length == 0) yield return null;

            foreach (GameObject[] track in _tracks)
            {
                int randomIndex = Random.Range(0, track.Length);
                for (int j = 0; j < track.Length; j++)
                {
                    track[j].SetActive(j <= randomIndex);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator UpdateBarsCoroutine()
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

    public IEnumerator UpdateBarsEightBandCoroutine()
    {
        while (true)
        {
            float[] bandPowers = _microphoneAdaptor.GetEightBandSpectrum();
            for (int i = 0; i < _tracks.Length; i++)
            {
                float bandPower = bandPowers[i];
                int activeBlocks = Mathf.CeilToInt(bandPower * 11); 
                activeBlocks = Mathf.Min(activeBlocks, 11);

                for (int j = 0; j < _tracks[i].Length; j++)
                {
                    _tracks[i][j].SetActive(j <= activeBlocks);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}