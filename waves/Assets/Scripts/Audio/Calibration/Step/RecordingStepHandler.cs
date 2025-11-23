using UnityEngine;

public abstract class RecordingCalibrationStepHandler : ICalibrationStepHandler
{
    protected abstract StepConfig StepConfig();
    protected abstract void SaveResult(CalibrationContext context, RecordingResult result);
    protected abstract CalibrationStep GetNextStep();
    private bool _isComplete;

    public void OnEnter(CalibrationContext context)
    {
        _isComplete = false;
        StepConfig stepConfig = StepConfig();
        context.SetInstructionText(stepConfig.InstructionText);
        context.SetButtonText(stepConfig.ButtonText);
        context.ShowVolumeMeter(stepConfig.ShowVolumeMeter);
        context.SetButtonsEnabled(true);
    }

    public void OnUpdate(CalibrationContext context)
    {
        context.Recorder.UpdateRecording(context.DeltaTime);

        float currentVolume = context.GetCurrentVolume();
        context.UpdateVolumeMeter(currentVolume);

        if (context.Recorder.IsRecording)
        {
            float remaining = context.RecordingDuration - context.Recorder.ElapsedTime;

            StepConfig stepConfig = StepConfig();

            context.SetInstructionText($"{remaining:F1}s");

            if (context.Recorder.IsComplete)
            {
                context.Recorder.StopRecording();
                RecordingResult result = context.Recorder.GetResult();

                SaveResult(context, result);
                
                context.SetInstructionText("Done");

                context.SetButtonText("Next");
                context.SetButtonsEnabled(true);
                _isComplete = true;
            }
        }
    }

    public void OnNextClicked(CalibrationContext context)
    {
        if (_isComplete)
        {
            context.TransitionToStep(GetNextStep());
            return;
        }

        Debug.Log("Starting recording...");
        context.Recorder.StartRecording(
            volumeProvider: context.GetCurrentVolume,
            duration: context.RecordingDuration
        );
        context.SetButtonsEnabled(false);
    }

    public void OnExit(CalibrationContext context)
    {
        _isComplete = false;
        if (!context.Recorder.IsRecording) return;
        
        context.Recorder.StopRecording();
    }

    public void OnSkipClicked(CalibrationContext context)
    {
        if (context.Persistence.HasSavedData())
        {
            context.Data = context.Persistence.Load();
        }

        context.TransitionToStep(CalibrationStep.Complete);
    }

    public bool CanSkip => true;
}

public class SilenceCalibrationStepHandler : RecordingCalibrationStepHandler
{
    protected override StepConfig StepConfig() => CalibrationStepConfig.CalibrateSilenceStep;

    protected override void SaveResult(CalibrationContext context, RecordingResult result)
    {
        context.Data.MaxRecordedBackground = result.MaxVolume;
    }

    protected override CalibrationStep GetNextStep() => CalibrationStep.CalibrateQuiet;
}

public class QuietCalibrationStepHandler : RecordingCalibrationStepHandler
{
    protected override StepConfig StepConfig() => CalibrationStepConfig.CalibrateQuietStep;

    protected override void SaveResult(CalibrationContext context, RecordingResult result)
    {
        context.Data.AvgRecordedQuiet = result.AverageVolume;
    }

    protected override CalibrationStep GetNextStep() => CalibrationStep.CalibrateLoud;
}

public class LoudCalibrationStepHandler : RecordingCalibrationStepHandler
{
    protected override StepConfig StepConfig() => CalibrationStepConfig.CalibrateLoudStep;

    protected override void SaveResult(CalibrationContext context, RecordingResult result)
    {
        context.Data.AvgRecordedLoud = result.AverageVolume;
    }

    protected override CalibrationStep GetNextStep() => CalibrationStep.Complete;
}