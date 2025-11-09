public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public readonly PaddleController PaddleController;
    public readonly BallController BallController;
    public readonly EffectsPlayer EffectsPlayer;

    public GameContext(
        PaddleController paddleController,
        BallController ballController,
        EffectsPlayer effectsPlayer
    )
    {
        PaddleController = paddleController;
        BallController = ballController;
        EffectsPlayer = effectsPlayer;
    }
}