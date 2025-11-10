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
        "Press start and keep quiet for two seconds\n\n" +
        "We need to measure your background noise",
        ButtonText = "Start",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateQuietStep = new()
    {
        InstructionText =
        "Press start and make a quiet sound for two seconds\n\n" +
        "Try a whisper or a soft hum",
        ButtonText = "Start",
        ShowVolumeMeter = true
    };

    public static StepConfig CalibrateLoudStep = new()
    {
        InstructionText =
        "Press start and make a loud sound for two seconds\n\n" +
        "You could shout, sing, or hum loudly",
        ButtonText = "Start",
        ShowVolumeMeter = true
    };

    public static StepConfig CompleteStep = new()
    {
        InstructionText =
        "Try moving the paddle\n" +
        "Does it move left when you make a quiet sound?\n\n" +
        "And right when you make a loud sound?" +
        "\n\n" +
        "Yes? Press Play to start the game",
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