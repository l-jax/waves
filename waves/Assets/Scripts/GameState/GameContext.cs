using UnityEngine;

public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public readonly GameStateMachine StateMachine;
    public readonly PaddleController PaddleController;
    public readonly BallController BallController;
    public readonly EffectsPlayer EffectsPlayer;
    public readonly GameObject TitleScreenUI;
    public readonly GameObject MainMenuUI;
    public readonly GameObject KeyboardSetupUI;
    public readonly GameObject VoiceCalibrationUI;
    public readonly GameObject GameOverUI;

    public GameContext(
        GameStateMachine stateMachine,
        PaddleController paddleController,
        BallController ballController,
        EffectsPlayer effectsPlayer,
        GameObject titleScreenUI,
        GameObject mainMenuUI,
        GameObject keyboardSetupUI,
        GameObject voiceCalibrationUI,
        GameObject gameOverUI
    ) {
        StateMachine = stateMachine;
        PaddleController = paddleController;
        BallController = ballController;
        EffectsPlayer = effectsPlayer;
        TitleScreenUI = titleScreenUI;
        MainMenuUI = mainMenuUI;
        KeyboardSetupUI = keyboardSetupUI;
        VoiceCalibrationUI = voiceCalibrationUI;
        GameOverUI = gameOverUI;
    }
}