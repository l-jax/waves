using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calibration : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _calibrationPanel;
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private UnityEngine.UI.Slider _volumeMeter;
    [SerializeField] private UnityEngine.UI.Button _nextButton;
    [SerializeField] private UnityEngine.UI.Button _skipButton;
    [SerializeField] private UnityEngine.UI.Button _backButton;
    [SerializeField] private Tracker _tracker;
    [SerializeField] private float _calibrationDuration = 2f;
    
    private CalibrationStateMachine _stateMachine;
    private CalibrationContext _context;
    private Dictionary<CalibrationStep, ICalibrationStepHandler> _stepHandlers;
    
    private TextMeshProUGUI _nextButtonText;
    private Coroutine _autoAdvanceCoroutine;
    
    void Awake()
    {
        _nextButtonText = _nextButton.GetComponentInChildren<TextMeshProUGUI>();
        InitializeStateMachine();
        InitializeContext();
        InitializeStepHandlers();
    }
    
    void Start()
    {
        SetupButtons();
        LoadPreviousCalibration();
    }
    
    void Update()
    {
        if (!_calibrationPanel.activeSelf) return;
        
        _context.DeltaTime = Time.deltaTime;
        
        ICalibrationStepHandler currentHandler = _stepHandlers[_stateMachine.CurrentStep];
        currentHandler.OnUpdate(_context);
        
        _context.Recorder?.UpdateRecording(Time.deltaTime);
    }
    
    private void InitializeStateMachine()
    {
        _stateMachine = new CalibrationStateMachine();
        _stateMachine.StepChanged += OnStepChanged;
    }
    
    private void InitializeContext()
    {
        _context = new CalibrationContext
        {
            Data = new CalibrationData(),
            Recorder = new CalibrationRecorder(),
            Persistence = new PlayerPrefsCalibrationPersistence(),
            CalibrationDuration = _calibrationDuration,
            
            GetCurrentVolume = () => _tracker.GetCurrentVolume(),
            SetInstructionText = text => _instructionText.text = text,
            SetButtonText = text => _nextButtonText.text = text,
            SetButtonEnabled = enabled => _nextButton.interactable = enabled,
            ShowVolumeMeter = show => _volumeMeter.gameObject.SetActive(show),
            UpdateVolumeMeter = volume => {
                _volumeMeter.value = Mathf.Lerp(_volumeMeter.value, volume * 1000f, 0.3f);
            },
            ShowErrorMessage = message => {
                Debug.LogError(message);
                // Could show a popup here
            },
            TransitionToStep = step => TransitionToStep(step),
            CloseCalibration = () => CloseCalibration()
        };
    }
    
    private void InitializeStepHandlers()
    {
        _stepHandlers = new Dictionary<CalibrationStep, ICalibrationStepHandler>
        {
            [CalibrationStep.Welcome] = new WelcomeStepHandler(),
            [CalibrationStep.CalibrateSilence] = new RecordingCalibrationStepHandler(CalibrationStep.CalibrateSilence),
            [CalibrationStep.CalibrateQuiet] = new RecordingCalibrationStepHandler(CalibrationStep.CalibrateQuiet),
            [CalibrationStep.CalibrateLoud] = new RecordingCalibrationStepHandler(CalibrationStep.CalibrateLoud),
            [CalibrationStep.Complete] = new CompleteStepHandler()
        };
    }
    
    private void SetupButtons()
    {
        _nextButton.onClick.AddListener(() => {
            _stepHandlers[_stateMachine.CurrentStep].OnNextClicked(_context);
        });
        
        _skipButton.onClick.AddListener(() => {
            _stepHandlers[_stateMachine.CurrentStep].OnSkipClicked(_context);
        });
        
        _backButton.onClick.AddListener(() => {
            if (_stateMachine.CanGoBack)
            {
                TransitionToStep(_stateMachine.GoBack());
            }
        });
    }
    
    private void TransitionToStep(CalibrationStep newStep)
    {
        if (_autoAdvanceCoroutine != null)
        {
            StopCoroutine(_autoAdvanceCoroutine);
            _autoAdvanceCoroutine = null;
        }
        
        _stepHandlers[_stateMachine.CurrentStep].OnExit(_context);
        _stateMachine.TransitionTo(newStep);
    }
    
    private void OnStepChanged(CalibrationStep oldStep, CalibrationStep newStep)
    {
        _backButton.gameObject.SetActive(_stateMachine.CanGoBack);
        _skipButton.gameObject.SetActive(_stepHandlers[newStep].CanSkip);
        _stepHandlers[newStep].OnEnter(_context);
        
        if (newStep is CalibrationStep.CalibrateSilence 
                    or CalibrationStep.CalibrateQuiet 
                    or CalibrationStep.CalibrateLoud)
        {
            _autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterRecording());
        }
    }
    
    private IEnumerator AutoAdvanceAfterRecording()
    {
        yield return new WaitUntil(() => _context.Recorder.IsRecording);
        yield return new WaitUntil(() => _context.Recorder.IsComplete);
        
        yield return new WaitForSeconds(1.5f);
        
        TransitionToStep(_stateMachine.GetNextStep());
        
        _autoAdvanceCoroutine = null;
    }
    
    private void LoadPreviousCalibration()
    {
        if (_context.Persistence.HasSavedData())
        {
            _context.Data = _context.Persistence.Load();
            _tracker?.ApplyCalibration(_context.Data);
        }
    }
    
    private void CloseCalibration()
    {
        _calibrationPanel.SetActive(false);
        _tracker?.ApplyCalibration(_context.Data);
    }
    
    public void OpenCalibration()
    {
        _calibrationPanel.SetActive(true);
        TransitionToStep(CalibrationStep.Welcome);
    }
}