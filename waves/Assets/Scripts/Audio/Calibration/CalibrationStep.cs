using System;
using System.Collections.Generic;

public enum CalibrationStep
{
    Welcome,
    CalibrateSilence,
    CalibrateQuiet,
    CalibrateLoud,
    Complete
}

public class StepInfo
{
    public string InstructionText;
    public string ButtonText;
    public bool ShowVolumeMeter;
}

public interface ICalibrationStepHandler
{
    void OnEnter(CalibrationContext context);
    void OnUpdate(CalibrationContext context);
    void OnExit(CalibrationContext context);
    void OnNextClicked(CalibrationContext context);
    void OnSkipClicked(CalibrationContext context) { }
    bool CanSkip => true;
}

public class WelcomeStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.SetInstructionText(
            "Let's set up your voice control.\n\n" +
            "Silence makes the paddle stop\n" +
            "Quiet sounds move left\n" +
            "Loud sounds move right\n\n"
        );

        context.SetButtonText("Start");
        context.SetButtonEnabled(true);
        context.ShowVolumeMeter(false);
    }

    public void OnUpdate(CalibrationContext context)
    {
        // Nothing to update on welcome screen
    }

    public void OnExit(CalibrationContext context)
    {
        // Nothing to clean up
    }

    public void OnNextClicked(CalibrationContext context)
    {
        context.TransitionToStep(CalibrationStep.CalibrateSilence);
    }

    public void OnSkipClicked(CalibrationContext context)
    {
        context.Data.ApplyDefaults();
        context.TransitionToStep(CalibrationStep.Complete);
    }
}

public class RecordingCalibrationStepHandler : ICalibrationStepHandler
{
    private static readonly Dictionary<CalibrationStep, string> _validSteps = new()
    {
        { CalibrationStep.CalibrateSilence, "Please remain silent for two seconds.\n\nYour silence will set the baseline volume." },
        { CalibrationStep.CalibrateQuiet, "Please make quiet sounds for two seconds.\n\nYour quiet sounds will set the minimum volume." },
        { CalibrationStep.CalibrateLoud, "Please make loud sounds for two seconds.\n\nYour loud sounds will set the maximum volume." }
    };

    private readonly CalibrationStep _step;

    public RecordingCalibrationStepHandler(CalibrationStep step)
    {
        if (!_validSteps.ContainsKey(step))
        {
            throw new ArgumentException("Invalid step for RecordingCalibrationStepHandler");
        }

        _step = step;
    }

    public void OnEnter(CalibrationContext context)
    {
        context.SetInstructionText(_validSteps[_step]);
        context.SetButtonText("Start");
        context.SetButtonEnabled(true);
        context.ShowVolumeMeter(true);
    }

    public void OnUpdate(CalibrationContext context)
    {
        float currentVolume = context.GetCurrentVolume();
        context.UpdateVolumeMeter(currentVolume);

        if (context.IsRecording)
        {
            var recorder = context.Recorder;
            float remaining = context.CalibrationDuration - recorder.ElapsedTime;

            context.SetInstructionText(
                $"Two seconds: {remaining:F1}s\n\n" +
                $"Current: {currentVolume:F4}"
            );

            if (recorder.IsComplete)
            {
                FinishRecording(context);
            }
        }
    }

    public void OnExit(CalibrationContext context)
    {
        if (context.IsRecording)
        {
            context.Recorder.StopRecording();
        }
    }

    public void OnNextClicked(CalibrationContext context)
    {
        if (context.IsRecording)
        {
            return;
        }

        StartRecording(context);
    }

    private void StartRecording(CalibrationContext context)
    {
        context.Recorder.StartRecording(
            volumeProvider: context.GetCurrentVolume,
            duration: context.CalibrationDuration
        );

        context.SetButtonEnabled(false);
    }

    private void FinishRecording(CalibrationContext context)
    {
        context.Recorder.StopRecording();
        RecordingResult result = context.Recorder.GetResult();

        switch (_step)
        {
            case CalibrationStep.CalibrateSilence:
                context.Data.BackgroundVolume = result.MaxVolume;
                break;
            case CalibrationStep.CalibrateQuiet:
                context.Data.MinVolume = result.MaxVolume;
                break;
            case CalibrationStep.CalibrateLoud:
                context.Data.MaxVolume = result.AverageVolume;
                break;
        }

        context.SetInstructionText(
            $"Your volume is {result.AverageVolume:F4}\n\n"
        );

        context.SetButtonText("Next");
        context.SetButtonEnabled(true);
    }
}

public class CompleteStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.Data.Sanitize();
        
        if (!context.Data.IsValid())
        {
            context.ShowErrorMessage("Calibration data invalid. Using defaults.");
            context.Data.ApplyDefaults();
        }
        
        context.Persistence.Save(context.Data);
        
        context.SetInstructionText("All done!\n\nClick PLAY to start the game");
        context.SetButtonText("Play");
        context.SetButtonEnabled(true);
        context.ShowVolumeMeter(false);
    }
    
    public void OnUpdate(CalibrationContext context)
    {
        // Nothing to update
    }
    
    public void OnExit(CalibrationContext context)
    {
        // Nothing to clean up
    }
    
    public void OnNextClicked(CalibrationContext context)
    {
        context.CloseCalibration();
    }
    
    public bool CanSkip => false;
}



