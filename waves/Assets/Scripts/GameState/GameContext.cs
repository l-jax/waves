using System;
using UnityEngine;

public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public readonly GameStateMachine StateMachine;
    public readonly GameObject TitleScreenUI;
    public readonly GameObject MainMenuUI;
    public readonly GameObject KeyboardSetupUI;
    public readonly GameObject CalibrationUI;
    public readonly GameObject GameOverUI;

    public readonly Action<ControlSystem> SetControlSystem;
    public readonly Action<bool> EnableMovement;
    public readonly Action EndGame;

    public GameContext(
        GameStateMachine stateMachine,
        PaddleController paddleController,
        BallController ballController,
        EffectsPlayer effectsPlayer,
        Equalizer equalizer,
        GameObject titleScreenUI,
        GameObject mainMenuUI,
        GameObject keyboardSetupUI,
        GameObject calibrationUI,
        GameObject gameOverUI
    ) {
        StateMachine = stateMachine;
        TitleScreenUI = titleScreenUI;
        MainMenuUI = mainMenuUI;
        KeyboardSetupUI = keyboardSetupUI;
        CalibrationUI = calibrationUI;
        GameOverUI = gameOverUI;

        SetControlSystem = controlSystem => {
            paddleController.SetControlSystem(controlSystem);
            equalizer.SetControlSystem(controlSystem);
        };

        EnableMovement = enable => {
            if (enable)
            {
                paddleController.EnableMovement();
                ballController.EnableMovement();
            }
            else
            {
                paddleController.DisableMovement();
                ballController.DisableMovement();
            }
        };

        // TODO: make it possible to lose
        EndGame = () => effectsPlayer.PlayEffect(Effect.Win, Vector3.zero, Quaternion.identity);
    }
}