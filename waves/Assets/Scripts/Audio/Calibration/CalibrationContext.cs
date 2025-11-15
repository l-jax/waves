using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationContext
{
    public CalibrationData Data { get; set; }

    public readonly float RecordingDuration = 2.0f;
    public readonly GameObject CalibrationPanel;
    public readonly CalibrationRecorder Recorder;
    public readonly ICalibrationPersistence Persistence;

    public readonly Action<string> SetInstructionText;
    public readonly Action<string> SetButtonText;
    public readonly Action<bool> SetButtonEnabled;

    public readonly Action<bool> ShowVolumeMeter;
    public readonly Action<float> UpdateVolumeMeter;
    public readonly Func<float> GetCurrentVolume;
    
    public Action<CalibrationStep> TransitionToStep { get; set; }
    public Action<CalibrationData> ApplyCalibrationData { get; set; }

    public Action StartGame { get; set; }
    
    public float DeltaTime { get; set; }

    public CalibrationContext(
        GameController gameController,
        CalibrationStateMachine calibrationStateMachine,
        MicrophoneAdaptor microphoneAdaptor,
        GameObject calibrationPanel,
        CalibrationRecorder recorder,
        ICalibrationPersistence persistence,
        Slider volumeMeter,
        TextMeshProUGUI instructionText,
        Button button
    ) {
        CalibrationPanel = calibrationPanel;
        Recorder = recorder;
        Persistence = persistence;

        SetInstructionText = text => instructionText.text = text;
        SetButtonText = text => {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = text;
        };
        SetButtonEnabled = enabled => button.interactable = enabled;

        ShowVolumeMeter = show => volumeMeter.gameObject.SetActive(show);
        UpdateVolumeMeter = volume =>
        {
            volumeMeter.value = Mathf.Lerp(volumeMeter.value, volume * 1000f, 0.3f);
        };

        GetCurrentVolume = () => microphoneAdaptor.CurrentVolume;

        ApplyCalibrationData = data => microphoneAdaptor.SetVolumeThresholds(data.GetBackgroundVolume(), data.GetMidVolume());

        TransitionToStep = step => calibrationStateMachine.TransitionTo(step);

        StartGame = () => gameController.StartGame();

        Data = new CalibrationData();
    }
}