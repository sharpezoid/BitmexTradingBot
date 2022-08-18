using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Represents a candle on our trading view
/// </summary>
[System.Serializable]
public class Candle
{
    public DateTime timeStamp;

    public float open;

    public float close;

    public float high;

    public float low;

    public float volume;

    public int trades;

    public Candle(float testValue)
    {
        close = testValue;
        high = testValue;
        low = testValue;
        open = testValue;
    }
}
