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
    public int Lives { get { return _lives; } }
    private int _lives = 3;

    public GameContext(
        GameStateMachine stateMachine,
        PaddleController paddleController,
        BallController ballController,
        BlockTracker blockTracker,
        AudioController audioController,
        GameObject titleScreenUI,
        GameObject mainMenuUI,
        GameObject keyboardSetupUI,
        GameObject calibrationUI,
        GameObject gameOverUI,
        GameObject livesUI
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

            if (_lives <= 0)
            {
                StateMachine.TransitionTo(GameState.GameOver);
                return;
            }

            _lives--;
            if (livesUI.transform.childCount >= _lives)
            {
                livesUI.transform.GetChild(_lives).gameObject.SetActive(false);
            }
        };

        blockTracker.OnAllBlocksBroken += () => StateMachine.TransitionTo(GameState.GameOver);

        Reset = () => {   
            _lives = 3;
            for (int i = 0; i < livesUI.transform.childCount; i++)
            {
                livesUI.transform.GetChild(i).gameObject.SetActive(true);
            }
            blockTracker.Initialize();
        };
    }
}