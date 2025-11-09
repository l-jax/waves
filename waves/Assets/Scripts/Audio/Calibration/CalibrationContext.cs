using System;

public class CalibrationContext
{
    public CalibrationData Data { get; set; }
    
    public CalibrationRecorder Recorder { get; set; }
    public bool IsRecording => Recorder?.IsRecording ?? false;
    public float RecordingProgress => Recorder?.Progress ?? 0f;
    
    public Func<float> GetCurrentVolume { get; set; }
    
    public Action<string> SetInstructionText { get; set; }
    public Action<string> SetButtonText { get; set; }
    public Action<bool> SetButtonEnabled { get; set; }
    public Action<bool> ShowVolumeMeter { get; set; }
    public Action<bool> ShowSpeedSlider { get; set; }
    public Action<float, float, float> SetSpeedSliderRange { get; set; } // min, max, value
    public Action<float> UpdateVolumeMeter { get; set; }
    public Action<string> ShowErrorMessage { get; set; }
    
    public Action<CalibrationStep> TransitionToStep { get; set; }
    public Action CloseCalibration { get; set; }
    
    public ICalibrationPersistence Persistence { get; set; }
    
    public float CalibrationDuration { get; set; } = 2f;
    
    public float DeltaTime { get; set; }
}