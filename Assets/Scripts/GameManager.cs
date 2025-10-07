using GrimTools.Runtime.Core;
using UnityEngine;
using System;

public class GameManager : MonoSingleton<GameManager>
{
    
    private int _cash;
    public bool canAfford;
    public event Action<int> OnCashChanged;
    private void Awake()
    {
        
    }
    
    public void AddCash(int amount)
    {
        _cash += amount;
        OnCashChanged?.Invoke(_cash);
    }
    public int GetCash()
    {
        return _cash;
    }
    public void SetCash(int amount)
    {
        _cash = amount;
        OnCashChanged?.Invoke(_cash);
    }
    public bool CanDeductCash(int amount)
    {
        if (_cash >= amount)
            return true;
        return false;
    }
    
} 
