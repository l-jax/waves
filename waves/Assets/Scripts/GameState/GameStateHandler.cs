using UnityEngine;

public interface IGameStateHandler
{
    void OnEnter(GameContext context);
    void OnUpdate(GameContext context);
    void OnExit(GameContext context);
}

public class TitleScreenHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        // show UI
        // play title music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // hide UI
        // stop title music
    }
}

public class MainMenuHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        // show UI
        // play menu music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // hide UI
        // pause menu music
    }
}

public class KeyboardSetupHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.PaddleController.SetControlSystem(ControlSystem.Keyboard);
        // start menu music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // hide UI
        // pause menu music

    }
}

public class VoiceCalibrationHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        context.PaddleController.SetControlSystem(ControlSystem.Voice);
        // start menu music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // disable paddle controller
        // pause menu music
    }
}

public class PlayHandler : IGameStateHandler
{

    public void OnEnter(GameContext context)
    {
        context.PaddleController.EnableMovement();
        context.BallController.EnableMovement();
        // start game music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        context.PaddleController.DisableMovement();
        context.BallController.DisableMovement();
        // pause game music
    }
}

public class GameOverHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        // conditional effect
        context.EffectsPlayer.PlayEffect(Effect.Win, Vector3.zero, Quaternion.identity);
        // show game over UI
        // play game over music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // hide game over UI
        // stop game music
    }
}