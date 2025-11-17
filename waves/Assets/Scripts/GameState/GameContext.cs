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
    public readonly Action WinGame;
    public readonly Action Reset;
    private int _lives = 3;

    public GameContext(
        GameStateMachine stateMachine,
        PaddleController paddleController,
        BallController ballController,
        EffectsPlayer effectsPlayer,
        GameObject blocksContainer,
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

        ballController.OnOutOfBounds = () => {
            _lives--;
            if (_lives >0) return;
            StateMachine.TransitionTo(GameState.GameOver);
        };

        WinGame = () => {
            if (_lives <= 0) return;
            effectsPlayer.PlayEffect(Effect.Win, Vector3.zero, Quaternion.identity);
        };

        Reset = () => {   
            _lives = 3;
            foreach (Transform column in blocksContainer.transform)
            {
                foreach (Transform block in column)
                {
                    block.gameObject.SetActive(true);
                }
            }
        };
    }
}