using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instrument
{
    [SerializeField]
    public string symbol;
    [SerializeField]
    public double tickSize;
    [SerializeField]
    public double volume24H;
    [SerializeField]
    public float markPrice;

    public string TextOutput()
    { 
        string s = symbol + " " + tickSize + " " + volume24H;
        return s;
    }
}
