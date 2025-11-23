using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _menuMusicSource;
    [SerializeField] private EffectsPlayer _effectsPlayer;
    [SerializeField] private Equalizer _equalizer;
    [SerializeField] private EightTrackPlayer _eightTrackPlayer;
    [SerializeField] private WaveformVisualizer _waveformVisualizer;
    private ControlSystem _currentControlSystem;

    public void SetControlSystem(ControlSystem controlSystem)
    {
        _equalizer.SetControlSystem(controlSystem);
        _currentControlSystem = controlSystem;
    }

    public void EnableWaveform(bool enable)
    {
        _waveformVisualizer.EnableWaveform(enable);
    }

    public void WinGame()
    {
        _effectsPlayer.PlayEffect(Effect.Win, Vector3.zero, Quaternion.identity);
    }

    public void PlayMenuMusic()
    {
        _menuMusicSource.Play();
    }
    
    public void PauseMenuMusic()
    {
        _menuMusicSource.Pause();
    }

    public void FadeOutMenuMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    public void PlayGameMusic()
    {
        if (_currentControlSystem == ControlSystem.Voice) return;
        _eightTrackPlayer.Play();
    }

    public void PauseGameMusic()
    {
        _eightTrackPlayer.Pause();
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = _menuMusicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _menuMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        _menuMusicSource.volume = 0f;
        _menuMusicSource.Stop();
    }
}