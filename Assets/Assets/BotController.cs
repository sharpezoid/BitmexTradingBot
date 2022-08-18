using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class BotController : MonoSingleton<BotController>
{
    // keys
    private string apiKey = "----";
    private string apiSecret = "----";

    BitMEX.BitMEXApi bitmex;

    public LineRenderer WMALine;
    public LineRenderer TEMALine;

    public GameObject TimeslotPrefab;
    public List<TimeSlot> timeSlots = new List<TimeSlot>();

    public int BucketSize = 500;

    public float high = float.NegativeInfinity;
    public float low = float.PositiveInfinity;

    public int Range = 20;

    public float MaxRange = 0.0f;

    public const float SATOSHI_TO_BTC_DIVIDER = 100000000.0f;

    public TimeSlot.Direction CurrentOverallDirection = TimeSlot.Direction.None;

    public Wallet wallet;
    public Instrument CurrentInstrument;

    public float TickRate = 5.0f;
    float lastTickTime;

    public float MinimumTradeHoldTime = 3600f;
    float lastTradeTime;
    public float PercentOfFundsToUse = 0.1f;
    public float PercentStopLoss = 0.1f;
    public float MaximumMarkPriceOffset = 0.15f;
    public string Window = "4h";
    bool ready = false;

    public RectTransform ChartRect;
    public Position CurrentPosition = null;

    public bool WaitingForTrade = true;

    public bool HasTrade()
    {
        bool hasTrade = false;
        if (CurrentPosition != null)
        {
            if (CurrentPosition.isOpen)
            {
                hasTrade = true;
            }
        }
        return hasTrade;
    }
    public float GetMarkPrice()
    {
        return timeSlots[BucketSize - 1].candle.close;
    }

    public float ConvertBTCtoDollars(float btc)
    {
        return GetMarkPrice() * (btc / SATOSHI_TO_BTC_DIVIDER);
    }

    public float GetAvailableFunds()
    {
        float retVal = 0;
        if (wallet != null)
        {
            retVal += wallet.amount;
        }
        if (CurrentPosition != null)
        {
            retVal += CurrentPosition.realisedPnl;
        }
        return retVal;
 //       return wallet.amount + CurrentPosition.realisedPnl;
    }

    public Candle CurrentCandle()
    {
        return timeSlots[BucketSize - 1].candle;
    }
    

    private void Start()
    {
        bitmex = new BitMEX.BitMEXApi(apiKey, apiSecret);
        CurrentOverallDirection = TimeSlot.Direction.None;
        CurrentPosition = bitmex.GetPosition();
        lastTickTime = Time.time;
        lastTradeTime = Time.time;
        for (int i = 0; i < BucketSize; i++)
        {
            GameObject slot = GameObject.Instantiate(TimeslotPrefab, ChartRect);
            timeSlots.Add(slot.GetComponent<TimeSlot>());
        }
        TEMALine.positionCount = BucketSize;
        WMALine.positionCount = BucketSize;
        UpdateBucketData();
        WaitingForTrade = true;
        ready = true;
    }

    void Update()
    {
        if (ready)
        {
            if (Time.time > lastTickTime + TickRate)
            {
                lastTickTime = Time.time;
                UpdateBucketData();
                UIController.instance.UpdateUI();

                if (BucketValuesAreGood())
                {
                    if (HasTrade())
                    {
                        CheckForTradeClose();
                    }
                    else if (WaitingForTrade)
                    {
                        CheckForTradeEntry();
                    }
                }
            }
        }
    }

    /// <summary>
    /// possible that our buckets have crap data or we failed to retreive them...
    /// </summary>
    /// <returns></returns>
    bool BucketValuesAreGood()
    {
        if (timeSlots[BucketSize-1].candle.close == 0)
        {
            return false;
        }

        return true;
    }

    // Check for whether we want close a trade.
    void CheckForTradeClose()
    {
        float lastTEMAValue = timeSlots[BucketSize - 2].f_TEMA;
        float thisTEMAValue = timeSlots[BucketSize - 1].f_TEMA;

        Vector2 lhs = new Vector2(0,1);
        Vector2 rhs = new Vector2(1, thisTEMAValue - lastTEMAValue);
        float dot = Vector3.Dot(lhs, rhs);

        if ( (CurrentPosition.currentQty > 0 && dot < 0.0f) || CurrentPosition.currentQty < 0 && dot > 0.0f)
        {
            Debug.Log("Closing Trade");
            StartCoroutine(CloseTrade());
        }

        // check we don't stop loss.
        CheckStopLoss();
    }

    // Check for a chance to open a trade.
    void CheckForTradeEntry()
    {
        TimeSlot.Direction TEMADirection = timeSlots[BucketSize - 1].TEMADirection;
        TimeSlot.Direction WMADirection = timeSlots[BucketSize - 1].WMADirection;
        //if (TEMADirection == WMADirection && (TEMADirection != CurrentOverallDirection || WMADirection != CurrentOverallDirection))
        //{
        //    if (Time.time > lastTradeTime + MinimumTradeHoldTime)
        //    {
        //        EntrySignal(TEMADirection);
        //        lastTradeTime = Time.time;
        //    }
        //    CurrentOverallDirection = TEMADirection;
        //}

        // just TEMA
        if (TEMADirection != CurrentOverallDirection && Time.time > lastTradeTime + MinimumTradeHoldTime)
        {
            EntrySignal(TEMADirection);
            lastTradeTime = Time.time;

            CurrentOverallDirection = TEMADirection;
        }
    }

    void UpdateBucketData()
    {
        CurrentInstrument = bitmex.GetInstrument("XBTUSD");

        // -- full range of candles
        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        List<Candle> candles = bitmex.GetCandleHistory("XBTUSD", BucketSize, Window);

        if (candles.Count == BucketSize)
        {
            for (int i = 0; i < candles.Count; i++)
            {
                timeSlots[i].candle = candles[i];
            }

            // -- find chart high and low
            high = float.NegativeInfinity;
            low = float.PositiveInfinity;
            for (int i = 0; i < timeSlots.Count; i++)
            {
                if (timeSlots[i].candle.high > high)
                {
                    high = timeSlots[i].candle.high;
                }
                if (timeSlots[i].candle.low < low)
                {
                    low = timeSlots[i].candle.low;
                }
            }

            MaxRange = high - low;

            for (int i = 0; i < timeSlots.Count; i++)
            {
                timeSlots[i].SetupTimeSlot(i);
            }
        }
        else
        {
            Debug.Log("Failed : Candles (" + candles.Count + ") not same as Bucketsize (" + BucketSize + ")");
        }

        DrawLines();

        GetWallet();

        if (HasTrade())
        {
            CheckStopLoss();
        }
    }


    void CheckStopLoss()
    {
        float unrealisedROEPcnt = BotController.instance.CurrentPosition.unrealisedRoePcnt;
        if (unrealisedROEPcnt < -PercentStopLoss)
        {
            Debug.Log("<color=red>STOP LOSS CLOSE TRADE! unrealised ROE : " + unrealisedROEPcnt + "    stop loss : " + PercentStopLoss + "</color>");
            StartCoroutine(CloseTrade());
            WaitingForTrade = true;
        }
    }

    void EntrySignal(TimeSlot.Direction _direction)
    {
        if (CurrentOverallDirection == TimeSlot.Direction.Up && _direction == TimeSlot.Direction.Down)
        {
            WaitingForTrade = false;
            StartCoroutine(SellSignal());
        }
        else if (CurrentOverallDirection == TimeSlot.Direction.Down && _direction == TimeSlot.Direction.Up)
        {
            WaitingForTrade = false;
            StartCoroutine(BuySignal());
        }

        CurrentOverallDirection = _direction;
    }

    public bool CheckChartSanity()
    {
        float offset = 0;
        float markprice = CurrentPosition.markPrice;

        if (CurrentInstrument.markPrice > CurrentCandle().close)
        {
            offset = CurrentCandle().close / CurrentInstrument.markPrice;
        }
        else
        {
            offset = CurrentInstrument.markPrice / CurrentCandle().close;
        }
        
        if ( (1-offset) > MaximumMarkPriceOffset)
        {
            return false;
        }
        
        return true;
    }


    void DrawLines()
    {
        for (int aLoop = 0; aLoop < timeSlots.Count; aLoop++)
        {
            Vector3 pos = Vector3.zero;
            pos.y = GetChartYPosition(timeSlots[aLoop].f_TEMA);
            TEMALine.SetPosition(aLoop, pos);
            WMALine.SetPosition(aLoop, new Vector3(0, GetChartYPosition(timeSlots[aLoop].f_WMA), 0));
        }
    }

    float GetChartYPosition(float _in)
    {
        float range = high - low;

        // -- normalised value of our in value position on chart
        float normY = (_in - low) / range;

        // -- get position to draw at
        return timeSlots[0].candleBody.GetComponent<RectTransform>().position.y + timeSlots[0].candleBody.GetComponent<RectTransform>().sizeDelta.y * normY;
    }

    IEnumerator CloseTrade()
    {
        Debug.Log("CLOSING TRADE!!");

        lastTradeTime = Time.time;
        int tradeValue = Mathf.RoundToInt(CurrentPosition.currentQty);
        if (tradeValue > 0)
        {
            bitmex.MakeOrder(BitMEX.OrderType.Sell, tradeValue);
        }
        else
        {
            bitmex.MakeOrder(BitMEX.OrderType.Buy, -tradeValue);
        }

        yield return null;
    }

    IEnumerator BuySignal()
    {
        lastTradeTime = Time.time;

        int tradeValue = 0;

        if (HasTrade())
        {
            yield return StartCoroutine(CloseTrade());
        }

        if (CheckChartSanity())
        {
            float walletDollarValue = timeSlots[timeSlots.Count - 1].candle.close * (GetAvailableFunds() / SATOSHI_TO_BTC_DIVIDER);
            
            tradeValue = Mathf.RoundToInt(walletDollarValue * PercentOfFundsToUse * CurrentPosition.leverage);

            bitmex.MakeOrder(BitMEX.OrderType.Buy, tradeValue);

            WaitingForTrade = true;
        }
        yield return null;
    }

    IEnumerator SellSignal()
    {
        lastTradeTime = Time.time;

        int tradeValue = 0;

        if (HasTrade()) //this shouldn't be possible but just incase
        {
            yield return StartCoroutine(CloseTrade());
        }

        if (CheckChartSanity())
        {
            float walletDollarValue = timeSlots[timeSlots.Count - 1].candle.close * (GetAvailableFunds() / SATOSHI_TO_BTC_DIVIDER);
            tradeValue = Mathf.RoundToInt(walletDollarValue * PercentOfFundsToUse * CurrentPosition.leverage);

            bitmex.MakeOrder(BitMEX.OrderType.Sell, -tradeValue);
        }

        WaitingForTrade = true;

        yield return null;
    }

    public void GetWallet()
    {
        wallet = new Wallet();
        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        wallet = bitmex.GetWallet();
        CurrentPosition = bitmex.GetPosition();
    }

    public bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}
