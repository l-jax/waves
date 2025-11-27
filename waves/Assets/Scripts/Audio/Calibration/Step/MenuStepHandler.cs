public class WelcomeStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.SetInstructionText(CalibrationStepConfig.WelcomeStep.InstructionText);
        context.SetButtonText(CalibrationStepConfig.WelcomeStep.ButtonText);
        context.SetButtonsEnabled(true);
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
        }

        context.TransitionToStep(CalibrationStep.Complete);
    }
    
    public bool CanSkip => false;
}

public class CompleteStepHandler : ICalibrationStepHandler
{
    public void OnEnter(CalibrationContext context)
    {
        context.Persistence.Save(context.Data);
        context.ApplyCalibrationData(context.Data);

        context.SetInstructionText(CalibrationStepConfig.CompleteStep.InstructionText);
        context.SetButtonText(CalibrationStepConfig.CompleteStep.ButtonText);
        context.SetButtonsEnabled(true);
    }

    public void OnNextClicked(CalibrationContext context)
    {
        context.CalibrationPanel.SetActive(false);
        context.StartGame();
    }
    
    public bool CanSkip => false;
}
