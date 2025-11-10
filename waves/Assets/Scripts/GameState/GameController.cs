using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PaddleController _paddleController;
    [SerializeField] private BallController _ballController;
    [SerializeField] private EffectsPlayer _effectsPlayer;
    [SerializeField] private GameObject _titleScreenUI;
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _keyboardSetupUI;
    [SerializeField] private GameObject _calibrationUI;
    [SerializeField] private GameObject _gameOverUI;

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
        _stateMachine = new GameStateMachine(GameState.TitleScreen);
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
        _context = new GameContext(
            _stateMachine,
            _paddleController,
            _ballController,
            _effectsPlayer,
            _titleScreenUI,
            _mainMenuUI,
            _keyboardSetupUI,
            _calibrationUI,
            _gameOverUI
        );
    }

    private void OnStateChanged(GameState previousState, GameState newState)
    {
        IGameStateHandler previousHandler = _stateHandler[previousState];
        previousHandler.OnExit(_context);

        IGameStateHandler newHandler = _stateHandler[newState];
        newHandler.OnEnter(_context);
    }
}