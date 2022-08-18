using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The definition of a bar of time as represented on our graph
/// </summary>
public class TimeSlot : MonoBehaviour
{
    public Candle candle;

    //public Image SMADot;
    //public Image EMADot;
    public Image TEMADot;
    public Image WMADot;

    public float f_SMA;
    public float f_EMA;
    public float f_EMAEMA;
    public float f_EMAEMAEMA;
    public float f_TEMA;
    public float f_WMA;
    public float f_VOLUME;

    //public Image TEMA_DIRECTION;
    //public Image WMA_DIRECTION;
    //public Image OVERALL_DIRECTION;

    public enum Direction
    {
        Up,
        Down,
        None
    }
    public Direction WMADirection;
    public Direction TEMADirection;

    //public DebugController controller;
    public int index;

    public GameObject candlesObj;

    public void SetupTimeSlot(int _index)
    {
        index = _index;

        SetupCandle();

        PlotSMA();

        PlotEMA();

        PlotTEMA();

        PlotWMA();
    }

    void PlotSMA()
    {
        if (candle != null)
        {
            int range = BotController.instance.Range;
            if ((index) < range)
            {
                f_SMA = candle.close;
                return;
            }

            f_SMA = Indicators.instance.SMA(candle.close, BotController.instance.timeSlots.GetRange((index+1) - range, range));

            float yNorm = (f_SMA - BotController.instance.low) / (BotController.instance.high - BotController.instance.low);
            float yPos = yNorm * BotController.instance.ChartRect.rect.height;
        }
    }


    void PlotEMA()
    {
        if (candle != null)
        { 
            int range = BotController.instance.Range;
            if ((index) < range)
            {
                f_EMA = candle.close;
                f_EMAEMA = candle.close;
                f_EMAEMAEMA = candle.close;
                return;
            }

            f_EMA = Indicators.instance.EMA(candle.close, BotController.instance.timeSlots.GetRange( index + 1 - range, range));
            
            float yNorm = (f_EMA - BotController.instance.low) / (BotController.instance.high - BotController.instance.low);
            float yPos = yNorm * BotController.instance.ChartRect.rect.height;
            //EMADot.transform.localPosition = new Vector3(0, yPos, 0);
        }
    }

    void PlotTEMA()
    {
        if (candle != null)
        {
            int range = BotController.instance.Range;

            if (index < range)
            {
                f_TEMA = candle.close;
                return;
            }

            float ema1 = 0;
            float ema2 = 0;
            float ema3 = 0;
            List<TimeSlot> slots = BotController.instance.timeSlots.GetRange(index - range + 1, range);
            TimeSlot prevSlot = slots[slots.Count - 2];

            for (int i = 0; i < slots.Count; i++)
            {
                float prevThisSlotEMA = -1;
                prevThisSlotEMA = slots[i].f_EMA;
            }

            float k = 2.0f / (range + 1.0f);
            ema1 = f_EMA;
            ema2 = (prevSlot.f_EMAEMA) + k * (f_EMA - prevSlot.f_EMAEMA);
            f_EMAEMA = ema2;
            ema3 = (prevSlot.f_EMAEMAEMA) + k * (f_EMAEMA - prevSlot.f_EMAEMAEMA);
            f_EMAEMAEMA = ema3;
            
            f_TEMA = (3.0f * ema1) - (3.0f * ema2) + ema3;
            
            float yNorm = (f_TEMA - BotController.instance.low) / (BotController.instance.high - BotController.instance.low);
            float yPos = yNorm * BotController.instance.ChartRect.rect.height;

            TEMADot.transform.localPosition = new Vector3(0, yPos, 0);

            float smoothedPrevious = slots[slots.Count - 2].f_TEMA;
            smoothedPrevious += slots[slots.Count - 3].f_TEMA;
            smoothedPrevious += slots[slots.Count - 4].f_TEMA;
            smoothedPrevious += slots[slots.Count - 5].f_TEMA;
            smoothedPrevious += slots[slots.Count - 6].f_TEMA;
            smoothedPrevious /= 5;
            if (f_TEMA > smoothedPrevious)
            {
              //  TEMA_DIRECTION.color = Color.green;
                TEMADirection = Direction.Up;
            }
            else
            {
             //   TEMA_DIRECTION.color = Color.red;
                TEMADirection = Direction.Down;
            }
        }
        //DrawIndicators();
    }

    void PlotWMA()
    {
        //WMA = (Price * n + Price(1) * n - 1 + Price(n - 1) * 1) / (n * (n + 1) / 2)
        if (candle != null)
        {
            int range = BotController.instance.Range;

            if (index < range)
            {
                f_TEMA = candle.close;
                return;
            }
            List<TimeSlot> slots = BotController.instance.timeSlots.GetRange(index - range + 1, range);
            TimeSlot prevSlot = slots[slots.Count - 2];

            float top = 0;// slots[slots.Count - 1].candle.close * slots.Count;
            for (int i = slots.Count-1; i >= 0; i--)
            {
                top += slots[i].candle.close * (i-1);
            }
            float bottom = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                bottom += slots[i].candle.close;
            }

            f_WMA = top / bottom;

            float yNorm = (f_WMA - BotController.instance.low) / (BotController.instance.high - BotController.instance.low);
            float yPos = yNorm * BotController.instance.ChartRect.rect.height;

            WMADot.transform.localPosition = new Vector3(0, yPos, 0);

            float smoothedPrevious = slots[slots.Count - 2].f_WMA;
            smoothedPrevious += slots[slots.Count - 3].f_WMA;
            smoothedPrevious += slots[slots.Count - 4].f_WMA;
            smoothedPrevious += slots[slots.Count - 5].f_WMA;
            smoothedPrevious += slots[slots.Count - 6].f_WMA;
            smoothedPrevious /= 5;
            if (f_WMA > smoothedPrevious)
            {
                WMADirection = Direction.Up;
            }
            else
            {
                WMADirection = Direction.Down;
            }
        }
    }

 
    public RectTransform candleBody;
    public RectTransform candleWick;
    public RectTransform volumeBar;

    public void SetupCandle()
    {
        //-- get parent chart height
        float chartHeight = BotController.instance.ChartRect.rect.height;// candlesObj.GetComponent<RectTransform>().rect.height;

        // -- body
        float bodyY = 0;
        float wickY = 0;
        float bodyHeight = 0;
        float wickHeight = 0;
        float bodyNormY = 0;
        float bodyNormHeight = 0;

        float wickNormY = (candle.low - BotController.instance.low) / BotController.instance.MaxRange;
        float wickNormHeight = (candle.high - candle.low) / BotController.instance.MaxRange;

        if (candle.close >= candle.open) // GREEN CANDLE
        {
            bodyNormY = (candle.open - BotController.instance.low) / BotController.instance.MaxRange;
            bodyNormHeight = (candle.close - candle.open) / BotController.instance.MaxRange;

            candleBody.GetComponent<Image>().color = Color.green;
            candleWick.GetComponent<Image>().color = new Color(0, 1.0f, 0, 0.25f);
        }
        else // RED CANDLE
        {
            bodyNormY = ((candle.close - BotController.instance.low)) / BotController.instance.MaxRange;
            bodyNormHeight = (candle.open - candle.close) / BotController.instance.MaxRange;

            candleBody.GetComponent<Image>().color = Color.red;
            candleWick.GetComponent<Image>().color = new Color(1.0f, 0, 0, 0.25f);
        }

        bodyY = bodyNormY * chartHeight;
        bodyHeight = bodyNormHeight * chartHeight;
        if (bodyHeight < 1)
        {
            bodyHeight = 1;
        }

        wickY = wickNormY * chartHeight;
        wickHeight = wickNormHeight * chartHeight;
        
        candleBody.localPosition = new Vector3(0, bodyY, 0);
        candleBody.sizeDelta = new Vector2(20, bodyHeight);

        candleWick.localPosition = new Vector3(0, wickY, 0);
        candleWick.sizeDelta = new Vector2(6, wickHeight);
    }

    //public void SetupVolume()
    //{
    //    f_VOLUME = candle.volume;
    //    float maxRange = BotController.instance.high - BotController.instance.low;

    //    //-- get parent chart height
    //    float chartHeight = volumeObj.GetComponent<RectTransform>().rect.height;

    //    float wickNormHeight = candle.volume / 50000000.0f;
    //    volumeBar.sizeDelta = new Vector2(20, wickNormHeight * chartHeight);
    //    if (index > 0)
    //    {
    //        if (DebugController.instance.timeSlots[index - 1].f_VOLUME > f_VOLUME)
    //        {
    //            volumeBar.GetComponent<Image>().color = Color.red;
    //        }
    //        else
    //        {
    //            volumeBar.GetComponent<Image>().color = Color.green;
    //        }
    //    }
    //}

  //  public void SetupIntention()
  //  {
  //      if (TEMADirection == Direction.Up && WMADirection == Direction.Up)
  //      {
  //    //      OVERALL_DIRECTION.color = Color.green;
  //          if (index == DebugController.instance.BucketSize - 1)
  //          {
  //              DebugController.instance.SwapSignal(Direction.Up);
  //          }
  //      }
  //      else if (TEMADirection == Direction.Down && WMADirection == Direction.Down)
  //      {
  //    //      OVERALL_DIRECTION.color = Color.red;
  //          if (index == DebugController.instance.BucketSize - 1)
  //          { 
  //              DebugController.instance.SwapSignal(Direction.Down);
  //          }
  //      }
  //      else
  //      {
  ////          OVERALL_DIRECTION.color = Color.yellow;
  //      }
  //  }

    //void SetupMarkers()
    //{
    //    if (index == DebugController.instance.BucketSize - 1)
    //    {
    //        //currentLabel.gameObject.SetActive(true);
    //        //if (DebugController.instance.HasTrade())
    //        //{
    //        //    entryLabel.gameObject.SetActive(true);
    //        //}
    //        //else
    //        //{
    //        //    return;
    //        //}

    //        float maxRange = controller.high - controller.low;

    //        //-- get parent chart height
    //        float chartHeight = candlesObj.GetComponent<RectTransform>().rect.height;

    //        // -- body
    //        // float targetY = 0;
    //        float currentY = 0;
    //        float openY = 0;
    //        //float bodyHeight = 0;
    //        //float wickHeight = 0;
    //        //float bodyNormY = 0;
    //        //float bodyNormHeight = 0;
    //        //(value-min)/(max-min)
    //        float currentNormY = (candle.close - controller.low) / maxRange;
    //        float openNormY = (DebugController.instance.CurrentPosition.avgEntryPrice - controller.low) / maxRange;

    //        //if (candle.close >= candle.open) // GREEN CANDLE
    //        //{
    //        //    //bodyNormY = (candle.open - controller.low) / maxRange;
    //        //    //bodyNormHeight = (candle.close - candle.open) / maxRange;

    //        //    candleBody.GetComponent<Image>().color = Color.green;
    //        //    candleWick.GetComponent<Image>().color = new Color(0, 1.0f, 0, 0.25f);
    //        //}
    //        //else // RED CANDLE
    //        //{
    //        //    //bodyNormY = ((candle.close - controller.low)) / maxRange;
    //        //    //bodyNormHeight = (candle.open - candle.close) / maxRange;

    //        //    candleBody.GetComponent<Image>().color = Color.red;
    //        //    candleWick.GetComponent<Image>().color = new Color(1.0f, 0, 0, 0.25f);
    //        //}

    //        //bodyY = bodyNormY * chartHeight;
    //        //bodyHeight = bodyNormHeight * chartHeight;
    //        //if (bodyHeight < 1)
    //        //{
    //        //    bodyHeight = 1;
    //        //}

    //        currentY = currentNormY * chartHeight;
    //        openY = openNormY * chartHeight;
    //        // wickHeight = wickNormHeight * chartHeight;

    //        //currentLabel.transform.localPosition = new Vector3(0, currentY, 0);
    //        //currentLabel.GetComponentInChildren<Text>().text = candle.close.ToString();

    //        //entryLabel.transform.localPosition = new Vector3(0, openY, 0);
    //        //entryLabel.GetComponentInChildren<Text>().text = DebugController.instance.CurrentPosition.avgEntryPrice.ToString();
    //    }
    //}
}
