using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom information about a trade in progress
/// </summary>
public class Trade
{
    public int TradeID;
    public float EntryMarkPrice;
    public enum TradeType
    {
        Short,
        Long
    }
    public TradeType Type;
    public float EntryTime;
    public float TradeValue;
    public float TradeAmount;

    public Trade(float _entry, float _value, float _amount, TradeType _type)
    {
        EntryTime = Time.time;
        EntryMarkPrice = _entry;
        TradeValue = _value;
        TradeAmount = _amount;
        Type = _type;
    }
}
