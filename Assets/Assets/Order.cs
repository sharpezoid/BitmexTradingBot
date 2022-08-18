using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    [SerializeField]
    public string orderID;
    [SerializeField]
    public string clOrdID;
    [SerializeField]
    public string clOrdLinkID;
    [SerializeField]
    public int account;
    [SerializeField]
    public string symbol;
    [SerializeField]
    public string side;
    [SerializeField]
    public int simpleOrderQuantity;
    [SerializeField]
    public int price;
    [SerializeField]
    public int displayQuantity;
    [SerializeField]
    public int stopPx;
    [SerializeField]
    public int pegOffsetValue;
    [SerializeField]
    public string pegPriceType;
    [SerializeField]
    public string currency;
    [SerializeField]
    public string settleCurrency;
    [SerializeField]
    public string orderType;
    [SerializeField]
    public string timeInForce;
    [SerializeField]
    public string executeInstance;
    [SerializeField]
    public string contingencyType;
    [SerializeField]
    public string exDestination;
    [SerializeField]
    public string orderStatus;
    [SerializeField]
    public string triggered;
    [SerializeField]
    public bool workingIndicator;
    [SerializeField]
    public string orderRejectionReason;
    [SerializeField]
    public int simpleLeavesQuantity;
    [SerializeField]
    public int leavesQuantity;
    [SerializeField]
    public int simpleCumQuantity;
    [SerializeField]
    public int cumQuantity;
    [SerializeField]
    public int averagePx;
    [SerializeField]
    public string multipleReportingType;
    [SerializeField]
    public string str_string;
    [SerializeField]
    public string transactionTime;
    [SerializeField]
    public string timestamp;
}
