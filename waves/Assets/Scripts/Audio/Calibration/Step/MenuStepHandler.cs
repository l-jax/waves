public class WelcomeStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.SetInstructionText(CalibrationStepConfig.WelcomeStep.InstructionText);
        context.SetButtonText(CalibrationStepConfig.WelcomeStep.ButtonText);
        context.ShowVolumeMeter(CalibrationStepConfig.WelcomeStep.ShowVolumeMeter);
        context.SetButtonEnabled(true);
    }

    public void OnNextClicked(CalibrationContext context)
    {
        context.TransitionToStep(CalibrationStep.CalibrateSilence);
    }

    public void OnSkipClicked(CalibrationContext context)
    {
        if (context.Persistence.HasSavedData())
        {
            context.Data = context.Persistence.Load();
            context.Data.Sanitize();

        } else
        {
            context.Data.ApplyDefaults();
        }
        context.TransitionToStep(CalibrationStep.Complete);
    }
}

public class CompleteStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.Data.Sanitize();

        if (!context.Data.IsValid())
        {
            context.Data.ApplyDefaults();
        }

        context.Persistence.Save(context.Data);
        context.Tracker.ApplyCalibration(context.Data);

        context.SetInstructionText(CalibrationStepConfig.CompleteStep.InstructionText);
        context.SetButtonText(CalibrationStepConfig.CompleteStep.ButtonText);
        context.SetButtonEnabled(true);
        context.ShowVolumeMeter(CalibrationStepConfig.CompleteStep.ShowVolumeMeter);
    }

    public void OnNextClicked(CalibrationContext context)
    {
        context.CalibrationPanel.SetActive(false);
    }
    
    public bool CanSkip => false;
}
