using System;

public enum GameState
{
    TitleScreen,
    KeyboardSetup,
    Playing,
    GameOver
}

public class GameStateMachine
{
    public GameState CurrentState => _currentState;
    public event Action<GameState, GameState> StateChanged;
    private GameState _currentState;

    public GameStateMachine(GameState initialState)
    {
        _currentState = initialState;
    }

    public void TransitionTo(GameState nextState)
    {
        GameState previousState = _currentState;
        if (_currentState == nextState) return;
        if (!CanTransitionTo(nextState))
        {
            throw new InvalidOperationException($"Cannot transition from {_currentState} to {nextState}");
        }

        _currentState = nextState;
        StateChanged?.Invoke(previousState, nextState);
    }

    private bool CanTransitionTo(GameState nextState)
    {
        return _currentState switch
        {
            GameState.TitleScreen => nextState == GameState.KeyboardSetup,
            GameState.KeyboardSetup => nextState == GameState.Playing,
            GameState.Playing => nextState == GameState.GameOver,
            GameState.GameOver => nextState == GameState.KeyboardSetup,
            _ => false
        };
    }

}