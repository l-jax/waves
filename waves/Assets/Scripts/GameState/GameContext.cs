public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public GameState CurrentState { get; set; }

    public readonly PaddleController PaddleController;

    public GameContext(
        PaddleController paddleController
    )
    {
        CurrentState = GameState.Playing; // TODO: set properly
        PaddleController = paddleController;
    }
}