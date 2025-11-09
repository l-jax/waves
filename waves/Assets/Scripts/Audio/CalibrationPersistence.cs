using UnityEngine;

public interface ICalibrationPersistence
{
    void Save(CalibrationData data);
    CalibrationData Load();
    bool HasSavedData();
}

public class PlayerPrefsCalibrationPersistence : ICalibrationPersistence
{
    private const string CalibrationKey = "CalibrationData";

    public void Save(CalibrationData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(CalibrationKey, json);
        PlayerPrefs.Save();
    }

    public CalibrationData Load()
    {
        if (!HasSavedData())
            return null;

        string json = PlayerPrefs.GetString(CalibrationKey);
        return JsonUtility.FromJson<CalibrationData>(json);
    }

    public bool HasSavedData()
    {
        return PlayerPrefs.HasKey(CalibrationKey);
    }
}