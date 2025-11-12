public interface ICalibrationStepHandler
{
    void OnEnter(CalibrationContext context);
    void OnUpdate(CalibrationContext context) { }
    void OnExit(CalibrationContext context) { }
    void OnNextClicked(CalibrationContext context);
    void OnSkipClicked(CalibrationContext context) { }
    bool CanSkip => true;
}
