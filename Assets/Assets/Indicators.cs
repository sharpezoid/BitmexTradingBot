using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicators : MonoSingleton<Indicators>
{
    /// <summary>
    /// Returns Simple Moving Average of a candle using given previous candles
    /// </summary>
    /// <param name="val"></param>
    /// <param name="previousCandles"></param>
    /// <returns></returns>
    public float SMA(float val, List<TimeSlot> previousSlots)
    {
        float total = 0;
        for(int cLoop = 0; cLoop < previousSlots.Count; cLoop++)
        {
            float addedVal = previousSlots[cLoop].candle.close;
            total += previousSlots[cLoop].candle.close;
        }
        float SMA = total / previousSlots.Count;

        return SMA;
    }


    /// <summary>
    /// Calculates the Exponential Moving Average of a timeslot's candle
    /// </summary>
    /// <param name="val"></param>
    /// <param name="previousSlots"></param>
    /// <returns></returns>
    public float EMA(float closeValue, List<TimeSlot> previousSlots)
    {
        float retVal = -1;

        // EMAtoday = EMAyesterday + alpha * (PRICEtoday - EMAyesterday)

        ////float ema = closeValue * k + previousSlots[previousSlots.Count - 1].f_EMA * (1 - k);// previousSlots[previousSlots.Count - 1].f_EMA;
        //float prevEma = previousSlots[previousSlots.Count - 1].f_EMA;
        //Debug.Log("PREV EMA : " + prevEma);
        //float ema = k * closeValue + (1 - k) * prevEma;
        //retVal = ema;

        // [Close - previous EMA] * (2 / n+1) + previous EMA,
        //where n = range
        float prevEMA = previousSlots[previousSlots.Count-2].f_EMA;
       // Debug.Log("Prev EMA : " + prevEMA);
        //float EMA = (closeValue - prevEMA) * (2 / (previousSlots.Count + 1)) + prevEMA;

        //EMA = Price(t)×k + EMA(y)×(1−k)
        float k = 2.0f / (previousSlots.Count + 1);
        //Debug.Log("ALPHA : " + k);
        float EMA = (closeValue * k) + prevEMA * (1 - k);

        //float alpha = 2.0f / (previousSlots.Count + 1.0f);
        //float sum = 0.0f;
        //float prevSum = previousSlots[previousSlots.Count - 1].f_EMA;
        //sum = alpha * closeValue + (1 - alpha) * prevSum;

        retVal = EMA;
        return retVal;
    }

    /// <summary>
    /// Calculates the Triple Exponential Moving Average of a timeslot's candle... uuuggh..
    /// </summary>
    /// <param name="closeValue"></param>
    /// <param name="previousSlots"></param>
    /// <returns></returns>
    public float TEMA(float closeValue, List<TimeSlot> previousSlots)
    {
        float retVal = -1;

        float ema1 = EMA(closeValue, previousSlots);
        float ema2 = EMA(ema1, previousSlots);
        float ema3 = EMA(ema2, previousSlots);

        float tema = (3 * ema1) - (3 * ema2) + ema3; // alternatively:  3 * (ema1 - ema2) + ema3;

        retVal = tema;

        return retVal;
    }
}
