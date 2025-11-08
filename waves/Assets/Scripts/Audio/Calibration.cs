using TMPro;
using UnityEngine;

public class Calibration : MonoBehaviour
{
[Header("UI References")]
    [SerializeField] private GameObject _calibrationPanel;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private UnityEngine.UI.Slider _volumeMeter;
    [SerializeField] private UnityEngine.UI.Button _nextButton;
    [SerializeField] private UnityEngine.UI.Button _skipButton;
    [SerializeField] private UnityEngine.UI.Slider _speedSlider;

    [Header("Target")]
    [SerializeField] private Tracker _tracker;

    [Header("Settings")]
    [SerializeField] private float _calibrationDuration = 2f;
    [SerializeField] private float _safetyMargin = 1.2f;

    private enum CalibrationStep
    {
        Welcome,
        CalibrateSilence,
        CalibrateQuiet,
        CalibrateLoud,
        TuneSpeed,
        Complete
    }

    private readonly CalibrationData _calibrationData = new();
    private CalibrationStep _currentStep = CalibrationStep.Welcome;
    private bool _isRecording = false;
    private float _recordingTimer = 0f;

    void Start()
    {
        SetupButtons();
        LoadPreviousCalibration();

        OpenCalibration();

        // // Auto-open if never calibrated
        // if (!PlayerPrefs.HasKey("VelCal_Silence"))
        // {
        //     OpenCalibration();
        // }
    }

    void Update()
    {
        UpdateVolumeMeter();

        if (!_isRecording) return;
        UpdateRecording();
    }

    private void SetupButtons()
    {
        _nextButton.onClick.RemoveAllListeners();
        _nextButton.onClick.AddListener(OnNextClicked);
        
        _skipButton.onClick.RemoveAllListeners();
        _skipButton.onClick.AddListener(OnSkipClicked);

        if (_speedSlider == null) return;
        _speedSlider.onValueChanged.RemoveAllListeners();
        _speedSlider.onValueChanged.AddListener(OnSpeedChanged);
    }

    private void UpdateVolumeMeter()
    {
        if (_volumeMeter == null || !_volumeMeter.gameObject.activeSelf) return;
        if (_tracker == null) return;

        float loudness = _tracker.GetCurrentLoudness();
        _volumeMeter.value = Mathf.Lerp(_volumeMeter.value, loudness * 1000f, 0.3f);
    }

    private void OnNextClicked()
    {
        switch (_currentStep)
        {
            case CalibrationStep.Welcome:
                StartStep(CalibrationStep.CalibrateSilence);
                break;
            case CalibrationStep.CalibrateSilence:
                if (!_isRecording) StartRecording();
                break;
            case CalibrationStep.CalibrateQuiet:
                if (!_isRecording) StartRecording();
                break;
            case CalibrationStep.CalibrateLoud:
                if (!_isRecording) StartRecording();
                break;
            case CalibrationStep.TuneSpeed:
                StartStep(CalibrationStep.Complete);
                break;
            case CalibrationStep.Complete:
                CompleteCalibration();
                break;
        }
    }

    private void OnSkipClicked()
    {
        ApplyDefaultCalibration();
        CompleteCalibration();
    }

    private void OnSpeedChanged(float value)
    {
        _calibrationData.Speed = value;
        if (_tracker != null)
        {
            _tracker.ApplyCalibration(_calibrationData);
        }
    }

    private void StartStep(CalibrationStep step)
    {
        _currentStep = step;
        _isRecording = false;
        _recordingTimer = 0f;

        // Hide/show UI elements
        if (_volumeMeter != null) 
            _volumeMeter.gameObject.SetActive(
                step != CalibrationStep.Welcome && 
                step != CalibrationStep.Complete
            );
        if (_speedSlider != null) 
            _speedSlider.gameObject.SetActive(step == CalibrationStep.TuneSpeed);

        switch (step)
        {
            case CalibrationStep.Welcome:
                _instructionText.text = "Welcome! Let's set up your voice control.\n\n" +
                    "You'll make 3 sounds:\n" +
                    "• Silence (stop)\n" +
                    "• Quiet sound (move left)\n" +
                    "• Loud sound (move right)\n\n" +
                    "Takes 20 seconds!";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateSilence:
                _instructionText.text = "Click START and stay completely SILENT for 2 seconds.\n\n" +
                    "This lets the paddle STOP when you're quiet.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateQuiet:
                _instructionText.text = "Click START and make a QUIET sound for 2 seconds.\n\n" +
                    "(Whisper, soft hum, gentle 'shhh')\n\n" +
                    "This will move the paddle LEFT.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateLoud:
                _instructionText.text = "Click START and make a LOUD sound for 2 seconds!\n\n" +
                    "(Talk loudly, shout, clap)\n\n" +
                    "This will move the paddle RIGHT.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.TuneSpeed:
                _instructionText.text = "Try making quiet and loud sounds.\nAdjust the slider to control how fast the paddle moves.\n\n" +
                    $"Silence: {_calibrationData.SilenceThreshold:F4}\n" +
                    $"Quiet (left): {_calibrationData.MidpointLoudness:F4}\n" +
                    $"Loud (right): {_calibrationData.MaxLoudness:F4}";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                
                // Initialize speed slider
                if (_speedSlider != null)
                {
                    _speedSlider.minValue = 0.5f;
                    _speedSlider.maxValue = 5f;
                    _speedSlider.value = _calibrationData.Speed > 0 ? _calibrationData.Speed : 2f;
                }
                break;

            case CalibrationStep.Complete:
                _instructionText.text = "Perfect! Your controls:\n\n" +
                    "• Silence = STOP\n" +
                    "• Quiet sounds = Move LEFT\n" +
                    "• Loud sounds = Move RIGHT\n" +
                    "• Louder = Faster!\n\n" +
                    "Ready to play!";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play!";
                break;
        }
    }

    private void StartRecording()
    {
        _isRecording = true;
        _recordingTimer = 0f;
        _nextButton.interactable = false;
    }

    private void UpdateRecording()
    {
        _recordingTimer += Time.deltaTime;
        float loudness = _tracker.GetCurrentLoudness();
        float remaining = _calibrationDuration - _recordingTimer;

        switch (_currentStep)
        {
            case CalibrationStep.CalibrateSilence:
                _instructionText.text = $"Stay silent... {remaining:F1}s\n\nCurrent: {loudness:F4}";
                
                // Track maximum silence level (we want the highest "silent" reading)
                if (_recordingTimer == 0 || loudness > _calibrationData.SilenceThreshold)
                {
                    _calibrationData.SilenceThreshold = loudness;
                }
                break;

            case CalibrationStep.CalibrateQuiet:
                _instructionText.text = $"Quiet sound... {remaining:F1}s\n\nCurrent: {loudness:F4}";
                
                // Track average quiet level
                if (loudness > _calibrationData.MidpointLoudness)
                {
                    _calibrationData.MidpointLoudness = loudness;
                }
                break;

            case CalibrationStep.CalibrateLoud:
                _instructionText.text = $"LOUD sound! {remaining:F1}s\n\nCurrent: {loudness:F4}";
                
                // Track maximum loud level
                if (loudness > _calibrationData.MaxLoudness)
                {
                    _calibrationData.MaxLoudness = loudness;
                }
                break;
        }

        if (_recordingTimer >= _calibrationDuration)
        {
            FinishRecording();
        }
    }

    private void FinishRecording()
    {
        _isRecording = false;
        _nextButton.interactable = true;

        switch (_currentStep)
        {
            case CalibrationStep.CalibrateSilence:
                _calibrationData.SilenceThreshold *= _safetyMargin;
                _instructionText.text = $"Silence threshold set: {_calibrationData.SilenceThreshold:F4}\n\nClick Next.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                Invoke(nameof(GoToQuiet), 1.5f);
                break;

            case CalibrationStep.CalibrateQuiet:
                _calibrationData.MidpointLoudness *= _safetyMargin;
                
                // Ensure it's above silence threshold
                if (_calibrationData.MidpointLoudness <= _calibrationData.SilenceThreshold)
                {
                    _calibrationData.MidpointLoudness = _calibrationData.SilenceThreshold * 2f;
                }
                
                _instructionText.text = $"Quiet level set: {_calibrationData.MidpointLoudness:F4}\n\nClick Next.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                Invoke(nameof(GoToLoud), 1.5f);
                break;

            case CalibrationStep.CalibrateLoud:
                _calibrationData.MaxLoudness *= _safetyMargin;
                
                // Ensure valid hierarchy
                if (_calibrationData.MaxLoudness <= _calibrationData.MidpointLoudness)
                {
                    _calibrationData.MaxLoudness = _calibrationData.MidpointLoudness * 2f;
                }
                
                _instructionText.text = $"Loud level set: {_calibrationData.MaxLoudness:F4}\n\nClick Next.";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                Invoke(nameof(GoToSpeed), 1.5f);
                break;
        }
    }

    private void GoToQuiet() => StartStep(CalibrationStep.CalibrateQuiet);
    private void GoToLoud() => StartStep(CalibrationStep.CalibrateLoud);
    private void GoToSpeed() => StartStep(CalibrationStep.TuneSpeed);

    private void CompleteCalibration()
    {
        if (!_calibrationData.IsValid())
        {
            Debug.LogWarning("Invalid calibration, using defaults");
            ApplyDefaultCalibration();
        }

        _tracker.ApplyCalibration(_calibrationData);
        SaveCalibration();
        CloseCalibration();
    }

    private void ApplyDefaultCalibration()
    {
        _calibrationData.SilenceThreshold = 0.005f;
        _calibrationData.MidpointLoudness = 0.05f;
        _calibrationData.MaxLoudness = 0.2f;
        _calibrationData.Speed = 2f;
    }

    private void SaveCalibration()
    {
        PlayerPrefs.SetFloat("VelCal_Silence", _calibrationData.SilenceThreshold);
        PlayerPrefs.SetFloat("VelCal_Mid", _calibrationData.MidpointLoudness);
        PlayerPrefs.SetFloat("VelCal_Max", _calibrationData.MaxLoudness);
        PlayerPrefs.SetFloat("VelCal_Speed", _calibrationData.Speed);
        PlayerPrefs.Save();
    }

    private void LoadPreviousCalibration()
    {
        if (PlayerPrefs.HasKey("VelCal_Silence"))
        {
            _calibrationData.SilenceThreshold = PlayerPrefs.GetFloat("VelCal_Silence");
            _calibrationData.MidpointLoudness = PlayerPrefs.GetFloat("VelCal_Mid");
            _calibrationData.MaxLoudness = PlayerPrefs.GetFloat("VelCal_Max");
            _calibrationData.Speed = PlayerPrefs.GetFloat("VelCal_Speed");
            
            if (_tracker != null)
            {
                _tracker.ApplyCalibration(_calibrationData);
            }
        }
    }

    private void CloseCalibration()
    {
        _calibrationPanel.SetActive(false);
    }

    public void OpenCalibration()
    {
        _calibrationPanel.SetActive(true);
        StartStep(CalibrationStep.Welcome);
    }
}