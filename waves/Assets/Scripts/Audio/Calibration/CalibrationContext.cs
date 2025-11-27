using System;
using System.Collections.Generic;
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
    public readonly Action<bool> SetButtonsEnabled;

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
        Dictionary<string, Button> buttons
    ) {
        CalibrationPanel = calibrationPanel;
        Recorder = recorder;
        Persistence = persistence;

        SetInstructionText = text => instructionText.text = text;
        SetButtonText = text => {
            TextMeshProUGUI buttonText = buttons["Next"].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = text;
        };

        SetButtonsEnabled = enabled => {
            foreach (var button in buttons.Values)
            {
                button.interactable = enabled;
            }
        };
        
        UpdateVolumeMeter = volume =>
        {
            volumeMeter.value = Mathf.Lerp(volumeMeter.value, volume * 400f, 0.3f);
        };

        GetCurrentVolume = () => microphoneAdaptor.CurrentVolume;

        ApplyCalibrationData = data => microphoneAdaptor.SetVolumeThresholds(data.GetBackgroundVolume(), data.GetMidVolume());

        TransitionToStep = step => calibrationStateMachine.TransitionTo(step);

        StartGame = () => gameController.StartGame();

        Data = new CalibrationData();
    }
}