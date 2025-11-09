public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public readonly PaddleController PaddleController;
    public readonly BallController BallController;

    public GameContext(
        PaddleController paddleController,
        BallController ballController
    )
    {
        PaddleController = paddleController;
        BallController = ballController;
    }
}