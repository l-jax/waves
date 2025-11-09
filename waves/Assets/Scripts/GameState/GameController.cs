using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PaddleController _paddleController;
    [SerializeField] private BallController _ballController;

    private GameStateMachine _stateMachine;
    private Dictionary<GameState, IGameStateHandler> _stateHandler;
    private GameContext _context;

    void Awake()
    {
        InitializeStateMachine();
        InitializeGameStateHandlers();
        InitializeContext();

        IGameStateHandler handler = _stateHandler[_stateMachine.CurrentState];
        handler.OnEnter(_context);
    }

    void Update()
    {
        IGameStateHandler handler = _stateHandler[_stateMachine.CurrentState];
        handler.OnUpdate(_context);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new GameStateMachine(GameState.Playing); //TODO: set properly
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
        _context = new GameContext(_paddleController, _ballController);
    }

    private void OnStateChanged(GameState previousState, GameState newState)
    {
        IGameStateHandler previousHandler = _stateHandler[previousState];
        previousHandler.OnExit(_context);

        IGameStateHandler newHandler = _stateHandler[newState];
        newHandler.OnEnter(_context);
    }
}