using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Stationary
}

public class MicrophoneAdaptor
{
    public float GetCurrentVolume() => _currentVolume;
    private readonly MicrophoneInput _microphoneInput;
    private readonly float _backgroundVolume;
    private readonly float _midVolume;
    private float _currentVolume;

    private readonly float _waveformScale = 5f;
    private readonly float _displayHeightOffset = -2f;
    private readonly float _displayDepth = -1f;
    private Color _lineColor = new(0.1f, 0.8f, 1.0f, 0.7f);
    private readonly float _startWidth = 0.1f;
    private readonly float _endWidth = 0.1f;
    private readonly Material _lineMaterial;
    private readonly LineRenderer _lineRenderer;
    private readonly float _waveformStartX = -7f;
    private readonly float _waveformWidth = 14f;

    public MicrophoneAdaptor(
        LineRenderer lineRenderer,
        AudioSource audioSource,
        CalibrationData calibrationData)
    {
        _lineRenderer = lineRenderer;
        _backgroundVolume = calibrationData.GetBackgroundVolume();
        _midVolume = calibrationData.GetMidVolume();
        _microphoneInput = new MicrophoneInput(audioSource);
        _microphoneInput.Initialize();
        SetupLineRenderer();
    }



    public void Update()
    {
        _microphoneInput.Update();
        _currentVolume = _microphoneInput.GetVolume();

        // Update LineRenderer positions
        for (int i = 0; i < 256; i++)
        {
            // Map sample index to X position across the screen
            float xPos = _waveformStartX + i / 255f * _waveformWidth;
            
            // Map sample amplitude to Y position, scaled and offset
            float yPos = _microphoneInput.SampleBuffer[i] * _waveformScale + _displayHeightOffset;

            // Set the position, ensuring it's at the desired depth (Z)
            _lineRenderer.SetPosition(i, new Vector3(xPos, yPos, _displayDepth));
        }
    }

    public Direction GetDirection()
    {
        if (_currentVolume < _backgroundVolume)
        {
            return Direction.Stationary;
        }

        if (_currentVolume < _midVolume)
        {
            return Direction.Left;
        }
        return Direction.Right;
    }

    void SetupLineRenderer()
    {
        _lineRenderer.positionCount = 256;
        _lineRenderer.useWorldSpace = true; // Draw in world space

        _lineRenderer.startWidth = _startWidth;
        _lineRenderer.endWidth = _endWidth;
        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;

        if (_lineMaterial != null)
        {
            _lineRenderer.material = _lineMaterial;
        }
        else
        {
            // Fallback if no material is assigned
            Debug.LogWarning("No Line Material assigned. Using default transparent material. Consider creating one (e.g., URP/Unlit Transparent or Standard with Alpha Blending).");
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // A common transparent shader
            _lineRenderer.material.color = _lineColor;
        }
    }

}