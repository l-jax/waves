using System;

public enum GameState
{
    TitleScreen,
    MainMenu,
    KeyboardSetup,
    VoiceCalibration,
    Playing,
    GameOver
}

public class GameStateMachine
{
    public GameState CurrentState => _currentState;
    public event Action<GameState, GameState> StateChanged;
    private GameState _currentState;

    public GameStateMachine(GameState initialState = GameState.MainMenu)
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
            GameState.TitleScreen => nextState == GameState.MainMenu,
            GameState.MainMenu => nextState == GameState.KeyboardSetup || nextState == GameState.VoiceCalibration ,
            GameState.KeyboardSetup => nextState == GameState.Playing,
            GameState.VoiceCalibration => nextState == GameState.Playing,
            GameState.Playing => nextState == GameState.GameOver,
            GameState.GameOver => nextState == GameState.MainMenu,
            _ => false
        };
    }

}