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
        // show UI
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
        // enable paddle controller
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
        // enable paddle controller
        // enable ball release
        // start game music
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {
        // disable paddle controller
        // disable ball release
        // pause game music
    }
}

public class GameOverHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
        // show game over UI
        // start game music
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