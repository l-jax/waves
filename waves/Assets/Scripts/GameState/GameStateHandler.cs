using TMPro;
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
        context.AudioController.PlayMenuMusic();
    }

    public void OnExit(GameContext context)
    {
        context.TitleScreenUI.SetActive(false);
        context.AudioController.PauseMenuMusic();
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
        context.AudioController.PlayMenuMusic();
    }

    public void OnExit(GameContext context)
    {
        context.MainMenuUI.SetActive(false);
        context.AudioController.FadeOutMenuMusic(2f);
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
    }

    public void OnExit(GameContext context)
    {
        context.KeyboardSetupUI.SetActive(false);
    }
}

public class VoiceCalibrationHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.CalibrationUI.SetActive(true);
        context.SetControlSystem(ControlSystem.Voice);
        context.EnableWaveform(true);
    }

    public void OnExit(GameContext context)
    {
        context.CalibrationUI.SetActive(false);
    }
}

public class PlayHandler : IGameStateHandler
{

    public void OnEnter(GameContext context)
    {
        context.EnableMovement(true);
        context.AudioController.PlayGameMusic();
    }

    public void OnExit(GameContext context)
    {
        context.EnableMovement(false);
        context.EnableWaveform(false);
        context.AudioController.PauseGameMusic();
    }
}

public class GameOverHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        if (context.AllBlocksBroken())
        {
            context.AudioController.WinGame();
        }

        context.GameOverUI.SetActive(true);
        context.GameOverUI.GetComponentInChildren<TextMeshProUGUI>().text = context.AllBlocksBroken() ? "You Win" : "Game Over";
        context.GameOverUI.GetComponentInChildren<Button>().onClick.AddListener(() => {
            context.StateMachine.TransitionTo(GameState.MainMenu);
        });
        context.AudioController.PlayGameMusic();
    }

    public void OnExit(GameContext context)
    {
        context.GameOverUI.SetActive(false);
        context.Reset();
        context.AudioController.PauseGameMusic();
    }
}