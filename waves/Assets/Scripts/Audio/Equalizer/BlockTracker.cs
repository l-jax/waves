using UnityEngine;

public class BlockTracker : MonoBehaviour
{
    [SerializeField] private Transform _blocksContainer;
    private readonly int[] _maxRevealedHeight = new int[8];
    
    public void Initialize()
    {
        for (int i = 0; i < 8; i++)
        {
            _maxRevealedHeight[i] = 0;
        }
    }

    public void Update()
    {
        foreach (Transform column in _blocksContainer)
        {
            UpdateColumnHeight(column);
        }
    }

    private void UpdateColumnHeight(Transform column)
    {
        int maxRevealedHeight = 0;
        
        for (int j = column.childCount - 1; j >= 0; j--)
        {
            if (column.GetChild(j).gameObject.activeInHierarchy)
            {
                break;
            }
            maxRevealedHeight++;
        }
        
        _maxRevealedHeight[column.GetSiblingIndex()] = maxRevealedHeight;
    }

    public int GetMaxRevealedHeight(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= _maxRevealedHeight.Length)
        {
            return 0;
        }
        return _maxRevealedHeight[trackIndex];
    }

    public int[] GetAllMaxRevealedHeights()
    {
        return _maxRevealedHeight;
    }
}