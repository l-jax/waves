public enum ControlSystem
{
    Keyboard,
    Voice
}

public class GameContext
{
    public GameState CurrentState { get; set; }
    public ControlSystem CurrentControlSystem { get; set; }
}