using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using BitMEX;

/// <summary>
/// Usable wallet data format for information about account gained from API
/// </summary>
[System.Serializable]
public class Wallet
{
    [SerializeField]
    public int account = 0;
    [SerializeField]
    public string currency = "NONE";
    [SerializeField]
    public float prevDeposited = 0;
    [SerializeField]
    public float prevWithdrawn = 0;
    [SerializeField]
    public float prevTransferIn = 0;
    [SerializeField]
    public float prevTransferOut = 0;
    [SerializeField]
    public float prevAmount = 0;
    [SerializeField]
    public string prevTimestamp = "ERROR";
    [SerializeField]
    public string timestamp = "ERROR";
    [SerializeField]
    public float deltaDeposited = 0;
    [SerializeField]
    public float deltaWithdrawn = 0;
    [SerializeField]
    public float deltaTransferIn = 0;
    [SerializeField]
    public float deltaTransferOut = 0;
    [SerializeField]
    public float deltaAmount = 0;
    [SerializeField]
    public float deposited = 0;
    [SerializeField]
    public float withdrawn = 0;
    [SerializeField]
    public float transferIn = 0;
    [SerializeField]
    public float transferOut = 0;
    [SerializeField]
    public float amount = 0;
    [SerializeField]
    public float pendingCredit = 0;
    [SerializeField]
    public float pendingDebit = 0;
    [SerializeField]
    public float confirmedDebit = 0;
    [SerializeField]
    public string address = "ERROR";
    [SerializeField]
    public string script = "ERROR";
    [SerializeField]
    public string[] withdrawalLock;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("-----------ACCOUNT DETAILS ----------");
        sb.AppendLine("Account : " + account);
        sb.AppendLine("Currency : " + currency);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Time : " + timestamp + "   Previous Time : " + prevTimestamp);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Prev Deposit : " + prevDeposited);
        sb.AppendLine("Delta Deposit : " + deltaDeposited);
        sb.AppendLine("Deposited : " + deposited);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Prev Withdrawal : " + prevWithdrawn);
        sb.AppendLine("Delta Withdrawal : " + deltaWithdrawn);
        sb.AppendLine("Withdrawn :" + withdrawn);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Prev Transfer In : " + prevTransferIn);
        sb.AppendLine("Delta Transfer In : " + deltaTransferIn);
        sb.AppendLine("Transfer In : " + transferIn);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Prev Transfer Out : " + prevTransferOut);
        sb.AppendLine("Delta Transfer Out : " + deltaTransferOut);
        sb.AppendLine("Transfer Out : " + transferOut);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Prev Amount : " + prevAmount);
        sb.AppendLine("Delta Amount : " + deltaAmount);
        sb.AppendLine("Amount : " + amount);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Pending Credit : " + pendingCredit);
        sb.AppendLine("Pending Debit : " + pendingDebit);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Confirmed Debit : " + confirmedDebit);
        sb.AppendLine("-------------------------------------");
        sb.AppendLine("Address : " + address);
        sb.AppendLine("Script : " + script);
        sb.AppendLine("-----------DETAILS COMPLETED---------");

        return sb.ToString();
    }
}
