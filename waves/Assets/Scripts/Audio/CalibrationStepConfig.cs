using System.Collections.Generic;
public enum CalibrationStep
{
    Welcome,
    CalibrateSilence,
    CalibrateQuiet,
    CalibrateLoud,
    AdjustSpeed,
    Complete
}

public class StepInfo
{
    public string InstructionText;
    public string ButtonText;
    public bool ShowVolumeMeter;
    public bool ShowSpeedSlider;
}

public class CalibrationStepConfig
{
    public static readonly Dictionary<CalibrationStep, StepInfo> Steps = new()
    {
        [CalibrationStep.Welcome] = new StepInfo
        {
            InstructionText =
                "Let's set up your voice control.\n\n" +
                "Silence makes the paddle stop\n" +
                "Quiet sounds move left\n" +
                "Loud sounds move right\n\n",
            ButtonText = "Start",
            ShowVolumeMeter = false,
            ShowSpeedSlider = false
        },
        [CalibrationStep.CalibrateSilence] = new StepInfo
        {
            InstructionText =
                "First, we'll measure your background noise.\n\n" +
                "Please stay silent for 2 seconds.",
            ButtonText = "Calibrate",
            ShowVolumeMeter = true,
            ShowSpeedSlider = false
        },
        [CalibrationStep.CalibrateQuiet] = new StepInfo
        {
            InstructionText =
                "Next, we'll measure a quiet sound.\n\n" +
                "Please make a quiet sound for 2 seconds.",
            ButtonText = "Calibrate",
            ShowVolumeMeter = true,
            ShowSpeedSlider = false
        },
        [CalibrationStep.CalibrateLoud] = new StepInfo
        {
            InstructionText =
                "Now, we'll measure a loud sound.\n\n" +
                "Please make a loud sound for 2 seconds.",
            ButtonText = "Calibrate",
            ShowVolumeMeter = true,
            ShowSpeedSlider = false
        },
        [CalibrationStep.AdjustSpeed] = new StepInfo
        {
            InstructionText =
                "Finally, adjust the speed of the paddle movement to your liking.\n\n" +
                "Move the slider to set how fast the paddle responds to your voice.",
            ButtonText = "Finish",
            ShowVolumeMeter = false,
            ShowSpeedSlider = true
        },
        [CalibrationStep.Complete] = new StepInfo
        {
            InstructionText =
                "Calibration complete\n\n" +
                "You're all set to control the paddle with your voice.",
            ButtonText = "Play",
            ShowVolumeMeter = false,
            ShowSpeedSlider = false
        }
    };
}