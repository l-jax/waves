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
    public readonly AudioController AudioController;

    public readonly Action<ControlSystem> SetControlSystem;
    public readonly Action<bool> EnableMovement;
    public readonly Action<bool> EnableWaveform;
    public readonly Action WinGame;
    public readonly Action Reset;
    private int _lives = 3;

    public GameContext(
        GameStateMachine stateMachine,
        PaddleController paddleController,
        BallController ballController,
        GameObject blocksContainer,
        AudioController audioController,
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
        AudioController = audioController;

        SetControlSystem = controlSystem => {
            paddleController.SetControlSystem(controlSystem);
            audioController.SetControlSystem(controlSystem);
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

        EnableWaveform = enable => {
            audioController.EnableWaveform(enable);
        };

        ballController.OnOutOfBounds = () => {
            _lives--;
            if (_lives >0) return;
            StateMachine.TransitionTo(GameState.GameOver);
        };

        WinGame = () => {
            if (_lives <= 0) return;
            audioController.WinGame();
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