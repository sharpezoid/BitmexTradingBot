using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Position
{
    [SerializeField]
    public int account = 0;
    [SerializeField]
    public string symbol = "ERROR";
    [SerializeField]
    public string currency = "ERROR";
    [SerializeField]
    public string underlying = "ERROR";
    [SerializeField]
    public string quoteCurrency = "ERROR";
    [SerializeField]
    public float commission = 0;
    [SerializeField]
    public float initMarginReq = 0;
    [SerializeField]
    public float mafloatMarginReq = 0;
    [SerializeField]
    public float riskLimit = 0;
    [SerializeField]
    public float leverage = 0;
    [SerializeField]
    public bool crossMargin = true;
    [SerializeField]
    public float deleveragePercentile = 0;
    [SerializeField]
    public float rebalancedPnl = 0;
    [SerializeField]
    public float prevRealisedPnl = 0;
    [SerializeField]
    public float prevUnrealisedPnl = 0;
    [SerializeField]
    public float prevClosePrice = 0;
    [SerializeField]
    public string openingTimestamp = "ERROR";
    [SerializeField]
    public float openingQty = 0;
    [SerializeField]
    public float openingCost = 0;
    [SerializeField]
    public float openingComm = 0;
    [SerializeField]
    public float openOrderBuyQty = 0;
    [SerializeField]
    public float openOrderBuyCost = 0;
    [SerializeField]
    public float openOrderBuyPremium = 0;
    [SerializeField]
    public float openOrderSellQty = 0;
    [SerializeField]
    public float openOrderSellCost = 0;
    [SerializeField]
    public float openOrderSellPremium = 0;
    [SerializeField]
    public float execBuyQty = 0;
    [SerializeField]
    public float execBuyCost = 0;
    [SerializeField]
    public float execSellQty = 0;
    [SerializeField]
    public float execSellCost = 0;
    [SerializeField]
    public float execQty = 0;
    [SerializeField]
    public float execCost = 0;
    [SerializeField]
    public float execComm = 0;
    [SerializeField]
    public string currentTimestamp = "ERROR";
    [SerializeField]
    public float currentQty = 0;
    [SerializeField]
    public float currentCost = 0;
    [SerializeField]
    public float currentComm = 0;
    [SerializeField]
    public float realisedCost = 0;
    [SerializeField]
    public float unrealisedCost = 0;
    [SerializeField]
    public float grossOpenCost = 0;
    [SerializeField]
    public float grossOpenPremium = 0;
    [SerializeField]
    public float grossExecCost = 0;
    [SerializeField]
    public bool isOpen = true;
    [SerializeField]
    public float markPrice = 0;
    [SerializeField]
    public float markValue = 0;
    [SerializeField]
    public float riskValue = 0;
    [SerializeField]
    public float homeNotional = 0;
    [SerializeField]
    public float foreignNotional = 0;
    [SerializeField]
    public string posState = "ERROR";
    [SerializeField]
    public float posCost = 0;
    [SerializeField]
    public float posCost2 = 0;
    [SerializeField]
    public float posCross = 0;
    [SerializeField]
    public float posInit = 0;
    [SerializeField]
    public float posComm = 0;
    [SerializeField]
    public float posLoss = 0;
    [SerializeField]
    public float posMargin = 0;
    [SerializeField]
    public float posMafloat = 0;
    [SerializeField]
    public float posAllowance = 0;
    [SerializeField]
    public float taxableMargin = 0;
    [SerializeField]
    public float initMargin = 0;
    [SerializeField]
    public float mafloatMargin = 0;
    [SerializeField]
    public float sessionMargin = 0;
    [SerializeField]
    public float targetExcessMargin = 0;
    [SerializeField]
    public float varMargin = 0;
    [SerializeField]
    public float realisedGrossPnl = 0;
    [SerializeField]
    public float realisedTax = 0;
    [SerializeField]
    public float realisedPnl = 0;
    [SerializeField]
    public float unrealisedGrossPnl = 0;
    [SerializeField]
    public float longBankrupt = 0;
    [SerializeField]
    public float shortBankrupt = 0;
    [SerializeField]
    public float taxBase = 0;
    [SerializeField]
    public float indicativeTaxRate = 0;
    [SerializeField]
    public float indicativeTax = 0;
    [SerializeField]
    public float unrealisedTax = 0;
    [SerializeField]
    public float unrealisedPnl = 0;
    [SerializeField]
    public float unrealisedPnlPcnt = 0;
    [SerializeField]
    public float unrealisedRoePcnt = 0;
    [SerializeField]
    public float simpleQty = 0;
    [SerializeField]
    public float simpleCost = 0;
    [SerializeField]
    public float simpleValue = 0;
    [SerializeField]
    public float simplePnl = 0;
    [SerializeField]
    public float simplePnlPcnt = 0;
    [SerializeField]
    public float avgCostPrice = 0;
    [SerializeField]
    public float avgEntryPrice = 0;
    [SerializeField]
    public float breakEvenPrice = 0;
    [SerializeField]
    public float marginCallPrice = 0;
    [SerializeField]
    public float liquidationPrice = 0;
    [SerializeField]
    public float bankruptPrice = 0;
    [SerializeField]
    public string timestamp = "ERROR";
    [SerializeField]
    public float lastPrice = 0;
    [SerializeField]
    public float lastValue = 0;
}


//[{"account":55706,
//    "symbol":"XBTUSD",
//    "currency":"XBt",
//    "underlying":"XBT",
//    "quoteCurrency":"USD",
//    "commission":0.00075,
//    "initMarginReq":0.1,
//    "maintMarginReq":0.005,
//    "riskLimit":20000000000,
//    "leverage":10,
//    "crossMargin":false,
//    "deleveragePercentile":1,
//    "rebalancedPnl":-1568,
//    "prevRealisedPnl":1568,
//    "prevUnrealisedPnl":0,
//    "prevClosePrice":11028.77,
//    "openingTimestamp":"2019-07-20T23:00:00.000Z",
//    "openingQty":-10,
//    "openingCost":89498,
//    "openingComm":342,
//    "openOrderBuyQty":0,
//    "openOrderBuyCost":0,
//    "openOrderBuyPremium":0,
//    "openOrderSellQty":0,
//    "openOrderSellCost":0,
//    "openOrderSellPremium":0,
//    "execBuyQty":0,
//    "execBuyCost":0,
//    "execSellQty":0,
//    "execSellCost":0,
//    "execQty":0,
//    "execCost":0,
//    "execComm":0,
//    "currentTimestamp":"2019-07-20T23:30:35.059Z",
//    "currentQty":-10,
//    "currentCost":89498,
//    "currentComm":342,
//    "realisedCost":-1842,
//    "unrealisedCost":91340,
//    "grossOpenCost":0,
//    "grossOpenPremium":0,
//    "grossExecCost":0,
//    "isOpen":true,
//    "markPrice":10995.09,
//    "markValue":90950,
//    "riskValue":90950,
//    "homeNotional":-0.0009095,
//    "foreignNotional":10,
//    "posState":"",
//    "posCost":91340,
//    "posCost2":91340,
//    "posCross":0,
//    "posInit":9134,
//    "posComm":76,
//    "posLoss":0,
//    "posMargin":9210,
//    "posMaint":533,
//    "posAllowance":0,
//    "taxableMargin":0,
//    "initMargin":0,
//    "maintMargin":8820,
//    "sessionMargin":0,
//    "targetExcessMargin":0,
//    "varMargin":0,
//    "realisedGrossPnl":1842,
//    "realisedTax":0,
//    "realisedPnl":1500,
//    "unrealisedGrossPnl":-390,
//    "longBankrupt":0,
//    "shortBankrupt":0,
//    "taxBase":1452,
//    "indicativeTaxRate":0,
//    "indicativeTax":0,
//    "unrealisedTax":0,
//    "unrealisedPnl":-390,
//    "unrealisedPnlPcnt":-0.0043,
//    "unrealisedRoePcnt":-0.0427,
//    "simpleQty":null,
//    "simpleCost":null,
//    "simpleValue":null,
//    "simplePnl":null,
//    "simplePnlPcnt":null,
//    "avgCostPrice":10948,
//    "avgEntryPrice":10948,
//    "breakEvenPrice":10939.5,
//    "marginCallPrice":12096,
//    "liquidationPrice":12096,
//    "bankruptPrice":12163.5,
//    "timestamp":"2019-07-20T23:30:35.059Z",
//    "lastPrice":10995.09,
//    "lastValue":90950}]
