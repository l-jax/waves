using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CalibrationWizard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _calibrationPanel;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private UnityEngine.UI.Slider _volumeMeter;
    [SerializeField] private UnityEngine.UI.Button _nextButton;
    [SerializeField] private UnityEngine.UI.Button _skipButton;
    [SerializeField] private UnityEngine.UI.Button _backButton;
    [SerializeField] private PaddleController _paddleController;
    [SerializeField] private GameController _gameController;
    
    private CalibrationStateMachine _stateMachine;
    private Dictionary<CalibrationStep, ICalibrationStepHandler> _stepHandlers;
    private CalibrationContext _context;


    void Awake()
    {
        InitializeStateMachine();
        InitializeContext();
        InitializeStepHandlers();

        ICalibrationStepHandler handler = _stepHandlers[_stateMachine.CurrentStep];
        handler.OnEnter(_context);
    }

    void Start()
    {
        _nextButton.onClick.AddListener(() => {
            _stepHandlers[_stateMachine.CurrentStep].OnNextClicked(_context);
        });
        
        _skipButton.onClick.AddListener(() => {
            _stepHandlers[_stateMachine.CurrentStep].OnSkipClicked(_context);
        });
        
        _backButton.onClick.AddListener(() => {
            if (!_stateMachine.CanGoBack) return;
            _stateMachine.GoBack();
        });
    }

    void Update()
    {
        if (!_calibrationPanel.activeSelf) return;
        
        _context.DeltaTime = Time.deltaTime;
        
        ICalibrationStepHandler handler = _stepHandlers[_stateMachine.CurrentStep];
        handler.OnUpdate(_context);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new CalibrationStateMachine(CalibrationStep.Welcome);
        _stateMachine.StepChanged += OnStepChanged;
    }
    
    private void InitializeStepHandlers()
    {
        _stepHandlers = new Dictionary<CalibrationStep, ICalibrationStepHandler>
        {
            [CalibrationStep.Welcome] = new WelcomeStepHandler(),
            [CalibrationStep.CalibrateSilence] = new SilenceCalibrationStepHandler(),
            [CalibrationStep.CalibrateQuiet] = new QuietCalibrationStepHandler(),
            [CalibrationStep.CalibrateLoud] = new LoudCalibrationStepHandler(),
            [CalibrationStep.Complete] = new CompleteStepHandler()
        };
    }
    
    private void InitializeContext()
    {
        _context = new CalibrationContext(
            _gameController,
            _stateMachine,
            _paddleController,
            _calibrationPanel,
            new CalibrationRecorder(),
            new PlayerPrefsCalibrationPersistence(),
            _volumeMeter,
            _instructionText,
            _nextButton
        );
    }

    private void OnStepChanged(CalibrationStep oldStep, CalibrationStep newStep)
    {
        _backButton.gameObject.SetActive(_stateMachine.CanGoBack);
        _skipButton.gameObject.SetActive(_stepHandlers[newStep].CanSkip);

        ICalibrationStepHandler oldHandler = _stepHandlers[oldStep];
        oldHandler.OnExit(_context);
        ICalibrationStepHandler newHandler = _stepHandlers[newStep];
        newHandler.OnEnter(_context);
    }
}