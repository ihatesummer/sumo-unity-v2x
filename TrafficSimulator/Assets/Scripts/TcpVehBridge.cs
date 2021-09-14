using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpVehBridge : ITcpClient
{
    [Header("TCP Interface")]
    private ITcpClient _tcpClient;

    public TcpVehBridge(ITcpClient tcp)
    {
        this._tcpClient = tcp;
    }
    public void TxMsg(string msg)
    {
        this._tcpClient.TxMsg(msg);
    }
    public string RxMsg()
    {
        return this._tcpClient.RxMsg();
    }

    public bool IsLastMsg()
    {
        return this._tcpClient.IsLastMsg();
    }

}
