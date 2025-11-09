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

        float volume = _tracker.GetCurrentVolume();
        _volumeMeter.value = Mathf.Lerp(_volumeMeter.value, volume * 1000f, 0.3f);
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
        _tracker?.ApplyCalibration(_calibrationData);
    }

    private void StartStep(CalibrationStep step)
    {
        _currentStep = step;
        _isRecording = false;
        _recordingTimer = 0f;

        _volumeMeter?.gameObject.SetActive(step != CalibrationStep.Welcome && step != CalibrationStep.Complete);
        _speedSlider?.gameObject.SetActive(step == CalibrationStep.TuneSpeed);

        switch (step)
        {
            case CalibrationStep.Welcome:
                _instructionText.text =
                "Let's set up your voice control.\n\n" +
                "Silence makes the paddle stop\n" +
                "Quiet sounds move left\n" +
                "Loud sounds move right\n\n";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateSilence:
                _instructionText.text =
                "First, we need to measure the background noise\n\n" +
                "Click START and stay SILENT for 2 seconds\n\n"; ;
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateQuiet:
                _instructionText.text =
                "Now, we'll measure your quiet sounds\n\n" +
                "Click START and whisper, hum, or make a soft sound for 2 seconds\n\n";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.CalibrateLoud:
                _instructionText.text =
                "Finally, let's measure your loud sounds\n\n" +
                "Click START and speak, shout, or make a loud sound for 2 seconds\n\n";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                break;

            case CalibrationStep.TuneSpeed:
                _instructionText.text =
                "Make some sounds to see how it feels\n\n" +
                "Use the slider to adjust how fast the paddle moves\n\n" +
                "When you're happy with the speed, click NEXT";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                
                if (_speedSlider != null)
                {
                    _speedSlider.minValue = 0.5f;
                    _speedSlider.maxValue = 5f;
                    _speedSlider.value = _calibrationData.Speed > 0 ? _calibrationData.Speed : 2f;
                }
                break;

            case CalibrationStep.Complete:
                _instructionText.text = "All done\n\nClick PLAY to start the game";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
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
        float volume = _tracker.GetCurrentVolume();
        float remaining = _calibrationDuration - _recordingTimer;

        switch (_currentStep)
        {
            case CalibrationStep.CalibrateSilence:
                _instructionText.text = $"Stay silent for two seconds {remaining:F1}s\n\nCurrent: {volume:F4}";
                _calibrationData.BackgroundVolume += volume;
                break;

            case CalibrationStep.CalibrateQuiet:
                _instructionText.text = $"Make a quiet sound for two seconds {remaining:F1}s\n\nCurrent: {volume:F4}";
                _calibrationData.MinVolume += volume;
                break;

            case CalibrationStep.CalibrateLoud:
                _instructionText.text = $"Make a loud sound for two seconds {remaining:F1}s\n\nCurrent: {volume:F4}";
                _calibrationData.MaxVolume += volume;
                break;
        }

        if (_recordingTimer >= _calibrationDuration)
        {
            int frameCount = Mathf.RoundToInt(_calibrationDuration / Time.deltaTime);
            switch (_currentStep)
            {
                case CalibrationStep.CalibrateSilence:
                    _calibrationData.BackgroundVolume /= frameCount;
                    break;
                case CalibrationStep.CalibrateQuiet:
                    _calibrationData.MinVolume /= frameCount;
                    break;
                case CalibrationStep.CalibrateLoud:
                    _calibrationData.MaxVolume /= frameCount;
                    break;
            }
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
                _instructionText.text = $"Your background volume is {_calibrationData.BackgroundVolume:F4}\n\nClick Next to keep calibrating";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                Invoke(nameof(GoToQuiet), 1.5f);
                break;

            case CalibrationStep.CalibrateQuiet:
                if (_calibrationData.MinVolume <= _calibrationData.BackgroundVolume)
                {
                    _calibrationData.MinVolume = _calibrationData.BackgroundVolume * 2f;
                }
                _instructionText.text = $"Your quiet volume is {_calibrationData.MinVolume:F4}\n\nClick Next to keep calibrating";
                _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                Invoke(nameof(GoToLoud), 1.5f);
                break;

            case CalibrationStep.CalibrateLoud:
                if (_calibrationData.MaxVolume <= _calibrationData.MinVolume)
                {
                    _calibrationData.MaxVolume = _calibrationData.MinVolume * 2f;
                }
                _instructionText.text = $"Your loud volume is {_calibrationData.MaxVolume:F4}\n\nClick Next to keep calibrating";
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
        _calibrationData.BackgroundVolume = 0.005f;
        _calibrationData.MinVolume = 0.05f;
        _calibrationData.MaxVolume = 0.2f;
        _calibrationData.Speed = 2f;
    }

    private void SaveCalibration()
    {
        PlayerPrefs.SetFloat("VelCal_Silence", _calibrationData.BackgroundVolume);
        PlayerPrefs.SetFloat("VelCal_Min", _calibrationData.MinVolume);
        PlayerPrefs.SetFloat("VelCal_Max", _calibrationData.MaxVolume);
        PlayerPrefs.SetFloat("VelCal_Speed", _calibrationData.Speed);
        PlayerPrefs.Save();
    }

    private void LoadPreviousCalibration()
    {
        if (PlayerPrefs.HasKey("VelCal_Silence"))
        {
            _calibrationData.BackgroundVolume = PlayerPrefs.GetFloat("VelCal_Silence");
            _calibrationData.MinVolume = PlayerPrefs.GetFloat("VelCal_Min");
            _calibrationData.MaxVolume = PlayerPrefs.GetFloat("VelCal_Max");
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