using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Margin
{
    [SerializeField]
    public int account;
    [SerializeField]
    public string currency;
    [SerializeField]
    public int riskLimit;
    [SerializeField]
    public string prevState;
    [SerializeField]
    public string state;
    [SerializeField]
    public string action;
    [SerializeField]
    public int amount;
    [SerializeField]
    public int pendingCredit;
    [SerializeField]
    public int pendingDebit;
    [SerializeField]
    public int confirmedDebit;
    [SerializeField]
    public int prevRealisedPNL;
    [SerializeField]
    public int prevUnrealisedPNL;
    [SerializeField]
    public int grossCommision;
    [SerializeField]
    public int grossOpenCost;
    [SerializeField]
    public int grossOpenPremium;
    [SerializeField]
    public int grossExecCost;
    [SerializeField]
    public int grossMarkValue;
    [SerializeField]
    public int riskValue;
    [SerializeField]
    public int taxableMargin;
    [SerializeField]
    public int initMargin;
    [SerializeField]
    public int maintenanceMargin;
    [SerializeField]
    public int sessionMargin;
    [SerializeField]
    public int targetExcessMargin;
    [SerializeField]
    public int variableMargin;
    [SerializeField]
    public int realisedPNL;
    [SerializeField]
    public int unrealisedPNL;
    [SerializeField]
    public int indicativeTax;
    [SerializeField]
    public int unrealisedProfit;
    [SerializeField]
    public int syntheticMargin;
    [SerializeField]
    public int walletBalance;
    [SerializeField]
    public int marginBalance;
    [SerializeField]
    public int marginBalancePercent;
    [SerializeField]
    public int marginLeverage;
    [SerializeField]
    public int marginUsedPercent;
    [SerializeField]
    public int excessMargin;
    [SerializeField]
    public int excessMarginPercent;
    [SerializeField]
    public int availableMargin;
    [SerializeField]
    public int withdrawableMargin;
    [SerializeField]
    public string timeStamp;
    [SerializeField]
    public int grossLastValue;
    [SerializeField]
    public int commission;
}
