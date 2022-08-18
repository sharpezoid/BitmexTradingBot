using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoSingleton<UIController>
{
    public Text markPriceText;
    public Text roeText;
    public Text realRoEText;
    public Text availFundsText;
    public Text entryPriceText;
    public Text positionText;
    public Text dollarEstimateText;

    public GameObject UpDirection;
    public GameObject DownDirection;

    public GameObject UpTEMADirection;
    public GameObject DownTEMADirection;

    public GameObject UpWMADirection;
    public GameObject DownWMADirection;

    public void UpdateUI()
    {
        markPriceText.text = BotController.instance.GetMarkPrice().ToString();
        availFundsText.text = (BotController.instance.GetAvailableFunds() / BotController.SATOSHI_TO_BTC_DIVIDER).ToString() + "xbt";
        dollarEstimateText.text = BotController.instance.ConvertBTCtoDollars(BotController.instance.GetAvailableFunds()).ToString();

        if (BotController.instance.CurrentOverallDirection == TimeSlot.Direction.Up)
        {
            UpDirection.SetActive(true);
            DownDirection.SetActive(false);
        }
        else if (BotController.instance.CurrentOverallDirection == TimeSlot.Direction.Down)
        {
            UpDirection.SetActive(false);
            DownDirection.SetActive(true);
        }
        else
        {
            UpDirection.SetActive(true);
            DownDirection.SetActive(true);
        }

        if (BotController.instance.timeSlots[BotController.instance.BucketSize - 1].WMADirection == TimeSlot.Direction.Up)
        {
            UpWMADirection.SetActive(true);
            DownWMADirection.SetActive(false);
        }
        else
        {
            UpWMADirection.SetActive(false);
            DownWMADirection.SetActive(true);
        }

        if (BotController.instance.timeSlots[BotController.instance.BucketSize - 1].TEMADirection == TimeSlot.Direction.Up)
        {
            UpTEMADirection.SetActive(true);
            DownTEMADirection.SetActive(false);
        }
        else
        {
            UpTEMADirection.SetActive(false);
            DownTEMADirection.SetActive(true);
        }

        if (BotController.instance.HasTrade())
        {
            roeText.text = (BotController.instance.CurrentPosition.unrealisedRoePcnt*100.0f).ToString() + "%";
            if (BotController.instance.CurrentPosition.unrealisedRoePcnt >= 0.0f)
            {
                roeText.color = Color.green;
            }
            else
            {
                roeText.color = Color.red;
            }
            //# Contracts * Multiplier * (1/Entry Price - 1/Exit Price)
            // Unrealised Profit = ($1 /$1,000 - $1 /$1,250) *1,000 = 0.20 XBT
            //     Debug.Log("QTY : " + BotController.instance.CurrentPosition.simpleQty + "   close : " + BotController.instance.CurrentCandle().close + "  " + BotController.instance.CurrentPosition.markPrice + "  " + BotController.instance.CurrentPosition.realisedPnl + "  " + BotController.instance.CurrentPosition.rebalancedPnl);
            //      realRoEText.text = (BotController.instance.CurrentPosition.realisedPnl * 100.0f).ToString() + "%";// ((BotController.instance.CurrentPosition.avgEntryPrice - BotController.instance.CurrentPosition.markPrice) * BotController.instance.CurrentPosition.simpleQty).ToString();//.CurrentPosition.simpleQty  (BotController.instance.CurrentCandle().close - BotController.instance.CurrentPosition.openingCost).ToString() + "%";
            float realROE = BotController.instance.CurrentPosition.simpleQty * 1 * ((1.0f / BotController.instance.CurrentPosition.avgEntryPrice) - (1.0f / BotController.instance.CurrentPosition.markPrice));
            realRoEText.text = realROE.ToString();


            entryPriceText.text = BotController.instance.CurrentPosition.avgEntryPrice.ToString();
            if (BotController.instance.CurrentPosition.currentQty > 0)
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
        else
        {
            positionText.text = "NONE";
            positionText.color = Color.white;

            roeText.text = " -- ";
            entryPriceText.text = " -- ";
        }
    }
}
