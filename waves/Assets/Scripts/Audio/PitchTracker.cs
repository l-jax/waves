using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchTracker : MonoBehaviour
{
    [Tooltip("The final, smoothed paddle position (0=Left, 0.5=Center, 1=Right)")]
    [Range(0f, 1f)]
    public float NormalizedPosition { get; private set; }

    [Header("Gameplay Pitch Control")]
    [SerializeField] private PitchMappingSettings _pitchMapping;
    
    [Header("Microphone Settings")]
    [SerializeField] private MicrophoneSettings _microphoneSettings;
    
    [Header("Pitch Detection Settings")]
    [SerializeField] private PitchDetectionSettings _detectionSettings;
    
    [Header("Anti-Jitter Settings")]
    [SerializeField] private JitterReductionSettings _jitterSettings;

    [Header("Debug Info (Read-Only)")]
    [SerializeField] private float _currentLoudness;
    [SerializeField] private float _currentRawPitch;
    [SerializeField] private float _currentFilteredPitch;

    private MicrophoneInput _microphone;
    private PitchDetector _detector;
    private PitchFilter _filter;

    void Start()
    {
        NormalizedPosition = 0.5f;
        
        _microphone = new MicrophoneInput(GetComponent<AudioSource>(), _microphoneSettings);
        _detector = new PitchDetector(_detectionSettings);
        _filter = new PitchFilter(_jitterSettings);
        
        _microphone.Initialize();
    }

    void Update()
    {
        _microphone.Update();
        
        _currentLoudness = _microphone.GetLoudness();
        
        if (_currentLoudness > _microphoneSettings.pitchDetectionThreshold)
        {
            _currentRawPitch = _detector.DetectPitch(_microphone);
            _filter.AddSample(_currentRawPitch);
        }
        else
        {
            _currentRawPitch = 0f;
        }

        _currentFilteredPitch = _filter.GetFilteredPitch();
        
        UpdateNormalizedPosition(_currentFilteredPitch);
    }

    private void UpdateNormalizedPosition(float pitch)
    {
        if (pitch < _detectionSettings.minDetectionFrequency || 
            pitch > _detectionSettings.maxDetectionFrequency)
        {
            return;
        }

        float targetPosition = Mathf.InverseLerp(
            _pitchMapping.minFrequency, 
            _pitchMapping.maxFrequency, 
            pitch
        );
        targetPosition = Mathf.Clamp01(targetPosition);

        float positionDiff = Mathf.Abs(targetPosition - NormalizedPosition);
        if (positionDiff > _jitterSettings.movementDeadzone)
        {
            NormalizedPosition = Mathf.Lerp(
                NormalizedPosition, 
                targetPosition, 
                _pitchMapping.smoothingRate
            );
        }
    }
}