public static class CalibrationStepConfig
{

    public static StepConfig WelcomeStep = new()
    {
        InstructionText =
        "Let's set up your voice control.\n\n" +
        "Silence makes the paddle stop\n" +
        "Quiet sounds move left\n" +
        "Loud sounds move right\n\n",
        ButtonText = "Start",
        ShowVolumeMeter = false
    };

    public static StepConfig CalibrateSilenceStep = new()
    {
        InstructionText =
        "Please remain silent for two seconds.\n\n" +
        "Your silence will set the baseline volume.",
        ButtonText = "Recording...",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateQuietStep = new()
    {
        InstructionText =
        "Please make quiet sounds for two seconds.\n\n" +
        "Your quiet sounds will set the minimum volume.",
        ButtonText = "Recording...",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateLoudStep = new()
    {
        InstructionText =
        "Please make loud sounds for two seconds.\n\n" +
        "Your loud sounds will set the maximum volume.",
        ButtonText = "Recording...",
        ShowVolumeMeter = true
    };

    public static StepConfig CompleteStep = new()
    {
        InstructionText = "All done!\n\nClick PLAY to start the game",
        ButtonText = "Play",
        ShowVolumeMeter = false
    };
}

public struct StepConfig
{
    public string InstructionText;
    public string ButtonText;
    public bool ShowVolumeMeter;
}