using System;
using System.Collections.Generic;

public class CalibrationStateMachine
{
    public CalibrationStep CurrentStep => _currentStep;
    public bool CanGoBack => _stepHistory.Count > 0;
    public event Action<CalibrationStep, CalibrationStep> StepChanged;
    private CalibrationStep _currentStep;
    private readonly Stack<CalibrationStep> _stepHistory = new();
    
    public CalibrationStateMachine(CalibrationStep initialStep)
    {
        _currentStep = initialStep;
    }
    
    public void TransitionTo(CalibrationStep nextStep)
    {
        CalibrationStep previousStep = _currentStep;
        if (_currentStep == nextStep) return;

        if (!CanTransitionTo(nextStep))
        {
            throw new InvalidOperationException($"Cannot transition from {_currentStep} to {nextStep}");
        }
        
        _stepHistory.Push(_currentStep);
        _currentStep = nextStep;
        StepChanged?.Invoke(previousStep, nextStep);
    }
    
    public CalibrationStep GoBack()
    {
        if (!CanGoBack)
        {
            throw new InvalidOperationException("Cannot go back - no history available");
        }

        CalibrationStep previousStep = _stepHistory.Pop();
        CalibrationStep currentStep = _currentStep;
        
        _currentStep = previousStep;
        StepChanged?.Invoke(currentStep, previousStep);
        return previousStep;
    }
    
    public bool CanTransitionTo(CalibrationStep nextStep)
    {
        return _currentStep switch
        {
            CalibrationStep.Welcome => nextStep == CalibrationStep.CalibrateSilence,
            CalibrationStep.CalibrateSilence => nextStep == CalibrationStep.CalibrateQuiet,
            CalibrationStep.CalibrateQuiet => nextStep == CalibrationStep.CalibrateLoud,
            CalibrationStep.CalibrateLoud => nextStep == CalibrationStep.Complete,
            _ => false
        };
    }
}