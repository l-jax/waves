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
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}

public class MainMenuHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}

public class KeyboardSetupHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}

public class VoiceCalibrationHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}

public class PlayHandler : IGameStateHandler
{

    public void OnEnter(GameContext context)
    {

    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}

public class GameOverHandler : IGameStateHandler
{
    public void OnEnter(GameContext context)
    {
    }

    public void OnUpdate(GameContext context)
    {

    }

    public void OnExit(GameContext context)
    {

    }
}