using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveformVisualizer : MonoBehaviour
{

    [SerializeField] private float _waveformScale = 5f;
    [SerializeField] private float _displayHeightOffset = -2f;
    [SerializeField] private float _displayDepth = -1f;
    [SerializeField] private Color _lineColor = new(0.1f, 0.8f, 1.0f, 0.7f);
    [SerializeField] private float _startWidth = 0.1f;
    [SerializeField] private float _endWidth = 0.1f;
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _waveformStartX = -7f;
    [SerializeField] private float _waveformWidth = 14f;

    private MicrophoneAdaptor _microphoneAdaptor;
    private LineRenderer _lineRenderer;

    void Awake()
    {
        _microphoneAdaptor = FindFirstObjectByType<MicrophoneAdaptor>();
        _lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    public void Update()
    {
        float[] samples = _microphoneAdaptor.SampleBuffer;
        _lineRenderer.positionCount = samples.Length;
        for (int i = 0; i < samples.Length; i++)
        {
            float xPos = _waveformStartX + i / (float)(samples.Length - 1) * _waveformWidth;
            float yPos = samples[i] * _waveformScale + _displayHeightOffset;
            _lineRenderer.SetPosition(i, new Vector3(xPos, yPos, _displayDepth));
        }
    }

    private void SetupLineRenderer()
    {
        _lineRenderer.useWorldSpace = true;

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
            Debug.LogWarning("No Line Material assigned. Using default transparent material. Consider creating one (e.g., URP/Unlit Transparent or Standard with Alpha Blending).");
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = _lineColor
            };
        }
    }
}
