using TMPro;
using UnityEngine;
using System.Collections;
using System;

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

    private readonly CalibrationData _calibrationData = new();
    private CalibrationStep _currentStep = CalibrationStep.Welcome;
    private bool _isRecording = false;
    private float _recordingTimer = 0f;

    private CalibrationRecorder _recorder;
    private ICalibrationPersistence _calibrationPersistence;

    void Start()
    {
        _calibrationPersistence = new PlayerPrefsCalibrationPersistence();
        _recorder = new CalibrationRecorder(() => _tracker.GetCurrentVolume());

        SetupButtons();
        OpenCalibration();
    }

    void Update()
    {
        UpdateVolumeMeter();
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
                StartCoroutine(Record());
                break;
            case CalibrationStep.CalibrateQuiet:
                StartCoroutine(Record());
                break;
            case CalibrationStep.CalibrateLoud:
                StartCoroutine(Record());
                break;
            case CalibrationStep.AdjustSpeed:
                StartStep(CalibrationStep.Complete);
                break;
            case CalibrationStep.Complete:
                CompleteCalibration();
                break;
        }
    }

    private void OnSkipClicked()
    {
        CompleteCalibration();
    }

    private void OnSpeedChanged(float value)
    {
        _calibrationData.Speed = value;
        _tracker?.ApplyCalibration(_calibrationData);
    }

    private void StartStep(CalibrationStep step)
    {
        _instructionText.text = CalibrationStepConfig.Steps[step].InstructionText;
        _nextButton.GetComponentInChildren<TextMeshProUGUI>().text = CalibrationStepConfig.Steps[step].ButtonText;

        if (_volumeMeter != null)
        {
            _volumeMeter.gameObject.SetActive(CalibrationStepConfig.Steps[step].ShowVolumeMeter);
        }

        if (_speedSlider != null)
        {
            _speedSlider.gameObject.SetActive(CalibrationStepConfig.Steps[step].ShowSpeedSlider);
        }

        _currentStep = step;
        _isRecording = false;
        _recordingTimer = 0f;
    }

    private IEnumerator Record()
    {
        _isRecording = true;
        _recordingTimer = 0f;
        _nextButton.interactable = false;
        _recorder.StartRecording();

        switch (_currentStep)
        {
            case CalibrationStep.CalibrateSilence:
                _instructionText.text = "Stay silent for two seconds\n\nCurrent: 0.0000";
                break;

            case CalibrationStep.CalibrateQuiet:
                _instructionText.text = "Make a quiet sound for two seconds\n\nCurrent: 0.0000";
                break;

            case CalibrationStep.CalibrateLoud:
                _instructionText.text = "Make a loud sound for two seconds\n\nCurrent: 0.0000";
                break;
        }

        while (_recordingTimer < _calibrationDuration)
        {
            _recorder.UpdateRecording();
            _recordingTimer += Time.deltaTime;
            yield return null;
        }

        float result = _recorder.GetResult();

        switch (_currentStep)
        {
            case CalibrationStep.CalibrateSilence:
                _calibrationData.BackgroundVolume = result;
                break;
            case CalibrationStep.CalibrateQuiet:
                _calibrationData.MinVolume = result;
                break;
            case CalibrationStep.CalibrateLoud:
                _calibrationData.MaxVolume = result;
                break;
        }

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
    private void GoToSpeed() => StartStep(CalibrationStep.AdjustSpeed);

    private void CompleteCalibration()
    {
        _calibrationData.Sanitize();
        if (!_calibrationData.IsValid())
        {
            Debug.LogWarning("Invalid calibration, using defaults");
            _calibrationData.ApplyDefaults();
        }

        _tracker.ApplyCalibration(_calibrationData);
        _calibrationPersistence.Save(_calibrationData);
        _calibrationPanel.SetActive(false);
    }

    public void OpenCalibration()
    {
        _calibrationPanel.SetActive(true);
        StartStep(CalibrationStep.Welcome);
    }
}