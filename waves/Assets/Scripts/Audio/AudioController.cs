using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _menuMusicSource;
    [SerializeField] private EffectsPlayer _effectsPlayer;
    [SerializeField] private Equalizer _equalizer;
    [SerializeField] private EightTrackPlayer _eightTrackPlayer;
    [SerializeField] private WaveformVisualizer _waveformVisualizer;

    public void SetControlSystem(ControlSystem controlSystem)
    {
        _equalizer.SetControlSystem(controlSystem);
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

    public void PlayGameMusic()
    {
        _eightTrackPlayer.Play();
    }

    public void PauseGameMusic()
    {
        _eightTrackPlayer.Pause();
    }
}