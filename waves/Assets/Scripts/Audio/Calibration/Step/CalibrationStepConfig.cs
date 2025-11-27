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
}

public static class CalibrationStepConfig
{

    public static StepConfig WelcomeStep = new()
    {
        Step = CalibrationStep.Welcome,
        InstructionText =
        "Let's calibrate your controls.",
        ButtonText = "Begin"
    };

    public static StepConfig CalibrateSilenceStep = new()
    {
        Step = CalibrationStep.CalibrateSilence,
        InstructionText =
        "Stay silent for two seconds.\n\n" +
        "We'll measure your background noise.",
        ButtonText = "Record"
    };

    public static StepConfig CalibrateQuietStep = new()
    {
        Step = CalibrationStep.CalibrateQuiet,
        InstructionText =
        "Now, make a quiet sound.\n\n" +
        "Try a whisper or soft hum.",
        ButtonText = "Record"
    };

    public static StepConfig CalibrateLoudStep = new()
    {
        Step = CalibrationStep.CalibrateLoud,
        InstructionText =
        "Next, make a loud sound.\n\n" +
        "Try shouting or singing.",
        ButtonText = "Record"
    };

    public static StepConfig CompleteStep = new()
    {
        Step = CalibrationStep.Complete,
        InstructionText =
        "Calibration complete.",
        ButtonText = "Play"
    };
}