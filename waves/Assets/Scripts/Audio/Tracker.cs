using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Tracker : MonoBehaviour
{
    [Tooltip("Current paddle position (0=Left, 0.5=Center, 1=Right)")]
    [Range(0f, 1f)]
    public float NormalizedPosition { get; private set; }

    [Header("Control Settings")]
    [SerializeField] private ControlSettings _controlSettings;

    [Header("Microphone Settings")]
    [SerializeField] private MicrophoneSettings _microphoneSettings;

    [Header("Debug Info (Read-Only)")]
    [SerializeField] private float _currentLoudness;
    [SerializeField] private float _currentVelocity;
    [SerializeField] private float _targetVelocity;

    private Input _microphone;

    void Start()
    {
        NormalizedPosition = 0.5f;
        
        _microphone = new Input(GetComponent<AudioSource>(), _microphoneSettings);
        _microphone.Initialize();
    }

    void Update()
    {
        _microphone.Update();
        _currentLoudness = _microphone.GetLoudness();

        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        // If below silence threshold, stop
        if (_currentLoudness < _controlSettings.SilenceThreshold)
        {
            _targetVelocity = 0f;
            return;
        }

        // Determine direction and speed based on loudness relative to midpoint
        if (_currentLoudness < _controlSettings.MidpointLoudness)
        {
            // QUIET = Move LEFT (negative velocity)
            // Map from silence threshold to midpoint → 0 to -MaxSpeed
            float intensity = Mathf.InverseLerp(
                _controlSettings.SilenceThreshold,
                _controlSettings.MidpointLoudness,
                _currentLoudness
            );
            _targetVelocity = -intensity * _controlSettings.MaxSpeed;
        }
        else
        {
            // LOUD = Move RIGHT (positive velocity)
            // Map from midpoint to max loudness → 0 to +MaxSpeed
            float intensity = Mathf.InverseLerp(
                _controlSettings.MidpointLoudness,
                _controlSettings.MaxLoudness,
                _currentLoudness
            );
            intensity = Mathf.Clamp01(intensity);
            _targetVelocity = intensity * _controlSettings.MaxSpeed;
        }
    }

    private void UpdatePosition()
    {
        // Smooth velocity changes to avoid jarring movements
        _currentVelocity = Mathf.Lerp(
            _currentVelocity,
            _targetVelocity,
            _controlSettings.VelocitySmoothing
        );

        // Update position based on velocity
        NormalizedPosition += _currentVelocity * Time.deltaTime;
        NormalizedPosition = Mathf.Clamp01(NormalizedPosition);
    }

    public void ApplyCalibration(CalibrationData data)
    {
        _controlSettings.SilenceThreshold = data.SilenceThreshold;
        _controlSettings.MidpointLoudness = data.MidpointLoudness;
        _controlSettings.MaxLoudness = data.MaxLoudness;
        _controlSettings.MaxSpeed = data.Speed;
        
        Debug.Log($"Calibration applied: Silence={data.SilenceThreshold:F4}, " +
                  $"Mid={data.MidpointLoudness:F4}, Max={data.MaxLoudness:F4}, Speed={data.Speed:F2}");
    }

    public float GetCurrentLoudness() => _currentLoudness;
}