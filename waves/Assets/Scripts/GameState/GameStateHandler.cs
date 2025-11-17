using UnityEngine;
using UnityEngine.UI;

public interface IGameStateHandler
{
    void OnEnter(GameContext context);
    void OnExit(GameContext context);
}

public class TitleScreenHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.TitleScreenUI.SetActive(true);
        context.TitleScreenUI.GetComponentInChildren<Button>().onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.MainMenu);
        });
        // play title music
    }

    public void OnExit(GameContext context)
    {
        context.TitleScreenUI.SetActive(false);
        // stop title music
    }
}

public class MainMenuHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.MainMenuUI.SetActive(true);
        Button[] buttons = context.MainMenuUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.KeyboardSetup);
        });
        buttons[1].onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.VoiceCalibration);
        });
        // play menu music
    }

    public void OnExit(GameContext context)
    {
        context.MainMenuUI.SetActive(false);
        // pause menu music
    }
}

public class KeyboardSetupHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.KeyboardSetupUI.SetActive(true);
        context.SetControlSystem(ControlSystem.Keyboard);
        context.KeyboardSetupUI.GetComponentInChildren<Button>().onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.Playing);
        });
        // start menu music
    }

    public void OnExit(GameContext context)
    {
        context.KeyboardSetupUI.SetActive(false);
        // pause menu music

    }
}

public class VoiceCalibrationHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.CalibrationUI.SetActive(true);
        context.SetControlSystem(ControlSystem.Voice);
        context.EnableWaveform(true);
        // start menu music
    }

    public void OnExit(GameContext context)
    {
        context.CalibrationUI.SetActive(false);
        // pause menu music     
    }
}

public class PlayHandler : IGameStateHandler
{

    public void OnEnter(GameContext context)
    {
        context.EnableMovement(true);
        // start game music
    }

    public void OnExit(GameContext context)
    {
        context.EnableMovement(false);
        context.EnableWaveform(false);
        // pause game music
    }
}

public class GameOverHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.WinGame();        
        context.GameOverUI.SetActive(true);
        context.GameOverUI.GetComponentInChildren<Button>().onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.MainMenu);
        });
        // play game over music
    }

    public void OnExit(GameContext context)
    {
        context.GameOverUI.SetActive(false);
        context.Reset();
        // stop game over music
    }
}