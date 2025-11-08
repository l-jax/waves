using UnityEngine;
using System.Collections;

public class Equaliser : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundModel;

    [SerializeField] private GameObject[][] _tracks;
    [SerializeField] private Tracker _tracker;


    public void Start()
    {
        if (_backgroundModel == null)
        {
            Debug.LogError("Background model is not assigned.");
            return;
        }

        _tracker = GameObject.Find("Microphone").GetComponent<Tracker>();
        if (_tracker == null)
        {
            Debug.LogError("No Tracker found in the scene");
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

        StartCoroutine(RandomiseBarsCoroutine());
    }

    // void Update()
    // {
    //     int[] equaliserBands = _tracker.GetEqualiserBands();
    //     UpdateBars(equaliserBands);

    // }

    public IEnumerator RandomiseBarsCoroutine()
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

    private void UpdateBars(int[] bands)
    {
        if (_tracks == null || _tracks.Length == 0) return;

        for (int i = 0; i < _tracks.Length; i++)
        {
            int activeBars = Mathf.Clamp(bands[i], 0, _tracks[i].Length);
            for (int j = 0; j < _tracks[i].Length; j++)
            {
                _tracks[i][j].SetActive(j < activeBars);
            }
        }
    }
}