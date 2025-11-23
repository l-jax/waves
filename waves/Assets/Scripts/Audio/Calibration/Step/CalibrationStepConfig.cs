public enum CalibrationStep
{
    Welcome,
    CalibrateSilence,
    CalibrateQuiet,
    CalibrateLoud,
    Complete
}

public struct StepConfig
{
    public CalibrationStep Step;
    public string InstructionText;
    public string ButtonText;
    public bool ShowVolumeMeter;
}

public static class CalibrationStepConfig
{

    public static StepConfig WelcomeStep = new()
    {
        Step = CalibrationStep.Welcome,
        InstructionText =
        "Let's calibrate your voice controls.\n\n" +
        "Silent: paddle stays still\n" +
        "Quiet: paddle moves left\n" +
        "Loud: paddle moves right",
        ButtonText = "Begin",
        ShowVolumeMeter = false
    };

    public static StepConfig CalibrateSilenceStep = new()
    {
        Step = CalibrationStep.CalibrateSilence,
        InstructionText =
        "Stay completely silent for two seconds.\n\n" +
        "We'll measure your background noise.",
        ButtonText = "Record",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateQuietStep = new()
    {
        Step = CalibrationStep.CalibrateQuiet,
        InstructionText =
        "Make a quiet sound for two seconds.\n\n" +
        "Try a whisper or soft hum.",
        ButtonText = "Record",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateLoudStep = new()
    {
        Step = CalibrationStep.CalibrateLoud,
        InstructionText =
        "Make a loud sound for two seconds.\n\n" +
        "Try shouting or singing.",
        ButtonText = "Record",
        ShowVolumeMeter = true
    };

    public static StepConfig CompleteStep = new()
    {
        Step = CalibrationStep.Complete,
        InstructionText =
        "Calibration complete. Ready to play?",
        ButtonText = "Play",
        ShowVolumeMeter = false
    };
}