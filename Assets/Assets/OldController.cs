using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

[Obsolete("Use BotController.cs instead")]
public class OldController : MonoSingleton<OldController>
{
    private string apiKey = "----";
    private string apiSecret = "----";

    BitMEX.BitMEXApi bitmex;

    public List<TimeSlot> timeSlots = new List<TimeSlot>();

    public int BucketSize = 500;

    public RectTransform chartObj;
    public GameObject candlePrefab;

    public float high = float.NegativeInfinity;
    public float low = float.PositiveInfinity;

    public int Range = 20;
    public LineRenderer smaLine;

    public const float SATOSHI_TO_BTC_DIVIDER = 100000000.0f;

    public TimeSlot.Direction CurrentOverallDirection = TimeSlot.Direction.None;

    bool ready = false;

    public Wallet wallet;
    AccountDetailPanel accountDetailPanel;

    public float tickRate = 5.0f;
    float lastTickTime;

    public Text markPriceText;
    public Text roeText;
    public Text availFundsText;
    public Text entryPriceText;
    public Text positionText;
    public Text differenceText;
    public Text dollarEstimateText;

    public GameObject UpDirection;
    public GameObject DownDirection;

    [Tooltip("The minimum amount of time to hold a trade to avoid Up/Down ticks within a candle lifespan (Recommended: == candle time)")]
    public float MinimumTradeHoldTime = 3600f;// ~ 1h
    float lastTradeTime;

    public float Margin = 25f;

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

 

    //public List<Trade> trades = new List<Trade>();
    public Position CurrentPosition = null;

    void Start()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(480, 320, false);
        bitmex = new BitMEX.BitMEXApi(apiKey, apiSecret);
        CurrentOverallDirection = TimeSlot.Direction.None;
        ready = true;
        CurrentPosition = bitmex.GetPosition();
        RedrawGraphWithNewScope();
        lastTickTime = Time.time;
        lastTradeTime = Time.time;
     }



    void Update()
    {
        if (ready)
        {
            if (Time.time > lastTickTime + tickRate)
            {
                lastTickTime = Time.time;
                RedrawGraphWithNewScope();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            lastTradeTime = Time.time - MinimumTradeHoldTime;
            StartCoroutine(SellSignal());
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            lastTradeTime = Time.time - MinimumTradeHoldTime;
            StartCoroutine(BuySignal());
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            CurrentOverallDirection = TimeSlot.Direction.Up;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            CurrentOverallDirection = TimeSlot.Direction.Down;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            bitmex.MakeOrder(BitMEX.OrderType.Buy, 10);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Get Orders : " + bitmex.GetOrders());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CurrentPosition = bitmex.GetPosition();
            if (CurrentPosition != null)
            {
                Debug.Log("Get Position : " + CurrentPosition);
            }
            else
            {
                Debug.Log("Null Current Position");
            }
        }
    }
    
    public void DebugOrders()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        string orders = bitmex.GetOrders();

        Debug.Log("Debug Orders : " + orders);
    }


    public void DebugInstruments()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        Instrument instrument = bitmex.GetInstrument("XBTUSD");
        Debug.Log("Instrument/ : " + instrument.symbol);
    }

    public void SwapSignal(TimeSlot.Direction _direction)
    {
        //Debug.Log("Swap Check - Current : " + CurrentOverallDirection.ToString() + "   TO : " + _direction);

        if (CurrentOverallDirection == TimeSlot.Direction.Up && _direction == TimeSlot.Direction.Down)
        {
            Debug.Log("Direction Switched to Down");
            if (Time.time > lastTradeTime + MinimumTradeHoldTime)
            {
                Debug.Log("SELL SIGNAL!");
                CurrentOverallDirection = _direction;
                StartCoroutine(SellSignal());
            }
        }
        else if (CurrentOverallDirection == TimeSlot.Direction.Down && _direction == TimeSlot.Direction.Up)
        {
            Debug.Log("Direction Switched to Up");
            if (Time.time > lastTradeTime + MinimumTradeHoldTime)
            {
                Debug.Log("BUY SIGNAL!");
                CurrentOverallDirection = _direction;
                StartCoroutine(BuySignal());
            }
        }
        else if (CurrentOverallDirection == TimeSlot.Direction.Up)
        {
            UpDirection.SetActive(true);
            DownDirection.SetActive(false);
        }
        else if (CurrentOverallDirection == TimeSlot.Direction.Down)
        {
            UpDirection.SetActive(false);
            DownDirection.SetActive(true);
        }
        else if (CurrentOverallDirection == TimeSlot.Direction.None)
        {
            //Debug.Log("Swap Check Match, Do nothing");
            CurrentOverallDirection = _direction;
        }

        
    }

    IEnumerator BuySignal()
    {
        positionText.text = "LONG";
        positionText.color = Color.green;
        lastTradeTime = Time.time;

        int tradeValue = 0;

        if (HasTrade())
        {
            tradeValue = Mathf.RoundToInt(Mathf.Abs(CurrentPosition.currentQty));
        }

        float walletDollarValue = timeSlots[timeSlots.Count - 1].candle.close * (wallet.amount / SATOSHI_TO_BTC_DIVIDER);
        tradeValue += Mathf.RoundToInt(walletDollarValue * 0.25f * Margin);

        Debug.Log("Buy Signal Values - wallet : " + walletDollarValue + "    trade value : " + tradeValue);// + "   trade Qty : " + tradeQty);
        bitmex.MakeOrder(BitMEX.OrderType.Buy, tradeValue);

        yield return null;
    }

    IEnumerator SellSignal()
    {
        Debug.Log("Get Sell Signal : " + bitmex.GetOrders());
        positionText.text = "SHORT";
        positionText.color = Color.red;
        lastTradeTime = Time.time;

        int tradeValue = 0;
        if (HasTrade())
        {
            tradeValue = Mathf.RoundToInt(Mathf.Abs(CurrentPosition.currentQty));
        }

        float walletDollarValue = timeSlots[timeSlots.Count - 1].candle.close * (wallet.amount / SATOSHI_TO_BTC_DIVIDER);
        tradeValue += Mathf.RoundToInt(walletDollarValue * 0.25f * Margin);

        Debug.Log("Sell Signal Values - wallet : " + walletDollarValue + "    trade value : " + tradeValue);// + "   trade Qty : " + tradeQty);
        bitmex.MakeOrder(BitMEX.OrderType.Sell, tradeValue);

        yield return null;        
    }

    public void RedrawGraphWithNewScope()
    {
        // -- remove all old timeslots objects
        for (int i = 0; i < chartObj.transform.childCount; i++)
        {
            Destroy(chartObj.transform.GetChild(i).gameObject);
        }

        // -- time slots that we build from
        timeSlots = new List<TimeSlot>();

        // -- full range of candles
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        List<Candle> fullCandleRange = bitmex.GetCandleHistory("XBTUSD", BucketSize, "1m");

        // -- then setup time slots
        for (int i = 0; i < fullCandleRange.Count; i++)
        {
            GameObject newTimeSlot = GameObject.Instantiate(candlePrefab);
            newTimeSlot.transform.SetParent(chartObj.transform);
            Vector3 pos = new Vector3(i * 20, 0, 0);
            newTimeSlot.GetComponent<RectTransform>().position = pos;
            newTimeSlot.GetComponent<TimeSlot>().candle = fullCandleRange[i];
           // newTimeSlot.GetComponent<TimeSlot>().controller = this;
            timeSlots.Add(newTimeSlot.GetComponent<TimeSlot>());
        }

        // -- find chart high and low
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

        for (int i = 0; i < timeSlots.Count; i++)
        {
            timeSlots[i].GetComponent<TimeSlot>().SetupTimeSlot(i);
        }

        if (timeSlots == null || timeSlots.Count == 0)
        {
            return;
        }

        DrawLines();

        // update wallet details etc.
        GetWallet();

        
        if (HasTrade())
        {
            markPriceText.text = timeSlots[timeSlots.Count - 1].candle.close.ToString();
            availFundsText.text = ((wallet.amount / SATOSHI_TO_BTC_DIVIDER)).ToString() + " xbt";
            dollarEstimateText.text = "est $" + (timeSlots[timeSlots.Count - 1].candle.close * (wallet.amount / SATOSHI_TO_BTC_DIVIDER)).ToString();

            float difference = 0.0f;
            if (timeSlots[timeSlots.Count - 1].candle.close > CurrentPosition.avgEntryPrice / SATOSHI_TO_BTC_DIVIDER)
            {
                difference = (timeSlots[timeSlots.Count - 1].candle.close / (CurrentPosition.avgEntryPrice / SATOSHI_TO_BTC_DIVIDER) - 1) * 100.0f;
                differenceText.text = difference + "%";
                if (CurrentOverallDirection == TimeSlot.Direction.Up)
                {
                    differenceText.color = Color.green;
                }
                else if (CurrentOverallDirection == TimeSlot.Direction.Down)
                {
                    differenceText.color = Color.red;
                }
            }
            else if (timeSlots[timeSlots.Count - 1].candle.close < (CurrentPosition.avgEntryPrice / SATOSHI_TO_BTC_DIVIDER))
            {
                difference = (1 - CurrentPosition.avgEntryPrice / timeSlots[timeSlots.Count - 1].candle.close) * 100.0f;
                differenceText.text = difference + "%";
                if (CurrentOverallDirection == TimeSlot.Direction.Down)
                {
                    differenceText.color = Color.green;
                }
                else
                {
                    differenceText.color = Color.red;
                }
            }
            else
            {
                difference = 0.0f;
                differenceText.text = "0%";
                differenceText.color = Color.white;
            }

            if (CurrentPosition.unrealisedRoePcnt > 0.0f)
            {
                roeText.color = Color.green;
            }
            else
            {
                roeText.color = Color.red;
            }
            roeText.text = (CurrentPosition.unrealisedRoePcnt * 100).ToString() + "%";

            entryPriceText.text = CurrentPosition.avgEntryPrice.ToString();
            if (CurrentPosition.openingQty > 0)
            {
                positionText.text = "LONG";
                positionText.color = Color.green;

            }
            else
            {
                positionText.text = "SHORT";
                positionText.color = Color.red;
            }
        }

        ready = true;
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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


    //void DrawIndicators()
    //{
    //    //if (IndicatorFlags.HasFlag(Indicators.SMA))
    //    //{
    //        DrawSMA();
    //    //}
    //}


    void DrawLines()
    {
        smaLine.positionCount = timeSlots.Count;

        for (int aLoop = 0; aLoop < timeSlots.Count; aLoop++)
        {
            Vector3 pos = Vector3.zero;

            pos.x = aLoop * 20;
            pos.y = GetChartYPosition(timeSlots[aLoop].f_TEMA);
            
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
            smaLine.SetPosition(aLoop, pos);
        }
    }

    float GetChartYPosition(float _in)
    {
        float range = high - low;
        // -- normalised value of our in value position on chart
        float normY = (_in - low) / range;

        //bodyNormY = (candle.close - controller.low) / maxRange;

        // -- get position to draw at
        //       float y = timeSlots[0].candlesObj.GetComponent<RectTransform>().position.y + timeSlots[0].candlesObj.GetComponent<RectTransform>().sizeDelta.y * normY;// chartRange;

        return 0;// y;
    }

    /// <summary>
    /// THE UI BUTTON HANDLER FOR GETTING WALLET
    /// </summary>
    public void GetWallet()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        wallet = bitmex.GetWallet();
        CurrentPosition = bitmex.GetPosition();
    }
}
