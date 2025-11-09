using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameStateMachine _stateMachine;
    private Dictionary<GameState, IGameStateHandler> _stateHandler;
    private GameContext _context;

    void Awake()
    {
        InitializeStateMachine();
        InitializeGameStateHandlers();
        InitializeContext();
    }

    void Update()
    {
        IGameStateHandler handler = _stateHandler[_stateMachine.CurrentState];
        handler.OnUpdate(_context);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new GameStateMachine();
        _stateMachine.StateChanged += OnStateChanged;
    }

    private void InitializeGameStateHandlers()
    {
        _stateHandler = new Dictionary<GameState, IGameStateHandler>
        {
            { GameState.TitleScreen, new TitleScreenHandler() },
            { GameState.MainMenu, new MainMenuHandler() },
            { GameState.KeyboardSetup, new KeyboardSetupHandler() },
            { GameState.VoiceCalibration, new VoiceCalibrationHandler() },
            { GameState.Playing, new PlayHandler() },
            { GameState.GameOver, new GameOverHandler() }
        };
    }

    private void InitializeContext()
    {
        _context = new GameContext
        {
            CurrentState = _stateMachine.CurrentState,
            CurrentControlSystem = ControlSystem.Keyboard
        };
    }

    private void OnStateChanged(GameState previousState, GameState newState)
    {
        Debug.Log($"Transitioned from {previousState} to {newState}");
    }
}