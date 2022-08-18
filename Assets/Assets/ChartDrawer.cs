using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// FIGURE TREND USING TEMA TO GET UP OR DOWN MARKET DIRECTION
/// USE WAVE PATTERN RECOGNITION TO PLOT POSSIBLE ABCD WAVES
/// BUY OR SELL DEPENDING ON TEMA DIRECTION AND WAVE EXPECTANCY
/// </summary>
/// 
public class ChartDrawer : MonoBehaviour
{
    public int EMALookbackPeriod = 20;

    public int resolution = 4;
    public Color green = Color.green;
    public Color red = Color.red;

    //SMA: 10 period sum / 10 
    //Multiplier: (2 / (Time periods + 1) ) = (2 / (10 + 1) ) = 0.1818 (18.18%)
    //EMA: {Close - EMA(previous day)} x multiplier + EMA(previous day). 
    public float EMA(float value, float period = 20)
    {
        float retVal = -1;

        float k = 2 / (period + 1);

        return retVal;
    }
}