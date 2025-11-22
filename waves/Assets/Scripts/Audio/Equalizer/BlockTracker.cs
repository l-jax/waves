using UnityEngine;
using System;

public class BlockTracker : MonoBehaviour
{
    [SerializeField] private Transform _blocksContainer;
    private readonly int[] _maxRevealedHeight = new int[8];
    private bool _allBlocksBroken = false;
    
    public event Action OnAllBlocksBroken;
    
    public void Initialize()
    {
        for (int i = 0; i < 8; i++)
        {
            _maxRevealedHeight[i] = 0;
        }
        
        _allBlocksBroken = false;
        
        foreach (Transform column in _blocksContainer)
        {
            for (int i = 0; i < column.childCount; i++)
            {
                column.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void Update()
    {
        foreach (Transform column in _blocksContainer)
        {
            UpdateColumnHeight(column);
        }
        
        CheckAllBlocksBroken();
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

    private void CheckAllBlocksBroken()
    {
        if (_allBlocksBroken) return;

        foreach (Transform column in _blocksContainer)
        {
            if (column.childCount == 0) continue;
            
            bool hasActiveBlock = false;
            for (int i = 0; i < column.childCount; i++)
            {
                if (column.GetChild(i).gameObject.activeInHierarchy)
                {
                    hasActiveBlock = true;
                    break;
                }
            }

            if (hasActiveBlock) return;
        }
        
        _allBlocksBroken = true;
        OnAllBlocksBroken?.Invoke();
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