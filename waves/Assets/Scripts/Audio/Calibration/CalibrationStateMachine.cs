using System;
using System.Collections.Generic;

public class CalibrationStateMachine
{
    private CalibrationStep _currentStep;
    private readonly Stack<CalibrationStep> _stepHistory = new();
    
    public CalibrationStep CurrentStep => _currentStep;
    public bool CanGoBack => _stepHistory.Count > 0;
    public bool IsInRecordingStep => IsRecordingStep(_currentStep);
    
    public event Action<CalibrationStep, CalibrationStep> StepChanged;
    
    public CalibrationStateMachine(CalibrationStep initialStep = CalibrationStep.Welcome)
    {
        _currentStep = initialStep;
    }
    
    public void TransitionTo(CalibrationStep nextStep, bool trackHistory = true)
    {
        var previousStep = _currentStep;
        
        if (trackHistory && _currentStep != nextStep)
        {
            _stepHistory.Push(_currentStep);
        }
        
        _currentStep = nextStep;
        StepChanged?.Invoke(previousStep, nextStep);
    }
    
    public CalibrationStep GoBack()
    {
        if (!CanGoBack)
        {
            throw new InvalidOperationException("Cannot go back - no history available");
        }
        
        var previousStep = _stepHistory.Pop();
        var currentStep = _currentStep;
        _currentStep = previousStep;
        
        StepChanged?.Invoke(currentStep, previousStep);
        return previousStep;
    }
    
    public CalibrationStep GetNextStep()
    {
        return _currentStep switch
        {
            CalibrationStep.Welcome => CalibrationStep.CalibrateSilence,
            CalibrationStep.CalibrateSilence => CalibrationStep.CalibrateQuiet,
            CalibrationStep.CalibrateQuiet => CalibrationStep.CalibrateLoud,
            CalibrationStep.CalibrateLoud => CalibrationStep.Complete,
            CalibrationStep.Complete => CalibrationStep.Complete, // Terminal state
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public void Reset()
    {
        _stepHistory.Clear();
        _currentStep = CalibrationStep.Welcome;
    }
    
    private static bool IsRecordingStep(CalibrationStep step)
    {
        return step is CalibrationStep.CalibrateSilence 
                    or CalibrationStep.CalibrateQuiet 
                    or CalibrationStep.CalibrateLoud;
    }
}