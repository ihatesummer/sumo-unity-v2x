using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SumoUnityTcpHandler : MonoBehaviour, ITcpClient
{
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private const int PORT = 4042;
    private const string IP = "127.0.0.1";
    private int mTimeout = 3;
    //private int mTimeout = 15;
    private string mSendMsg;
    private string mReceivedMsg;
    private string cleardIncoming;
    private string cleardIncoming2;
    private string Rx;
    public int mNumConnection = 1;
    public Queue<string> TCP_recv_queue = new Queue<string>();
    public Queue<string> Rx_queue = new Queue<string>();
    public Queue<string> TCP_send_queue = new Queue<string>();
    public Queue<string> Tx_queue = new Queue<string>();
    public bool lastMsg = false;

    // Start is called before the first frame update
    void Start()
    {
        ConnectServerInternal();
    }

    // Update is called once per frame
    void Update()
    {
        if (TCP_recv_queue.Count > 0)
        {
            string msg = TCP_recv_queue.Dequeue();
            if (msg.Contains("finish"))
            {
                clientReceiveThread.Abort();
                lastMsg = true;
            }
            else if (msg.Contains("O1G") && msg.Contains("&"))
            {
//                cleardIncoming2 = cleardIncoming.Substring(0, cleardIncoming.IndexOf("&") - 1);
                try
                {
                    cleardIncoming = msg.Substring(3);
                    cleardIncoming2 = cleardIncoming.Substring(0, cleardIncoming.IndexOf("&") - 1);
                    Rx = cleardIncoming2;
                    Rx_queue.Enqueue(Rx);
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    Debug.Log(e);
                    Rx_queue.Enqueue("null");
                }
                //Rx = cleardIncoming2;
                //Rx_queue.Enqueue(Rx);
            }

        }
        if (TCP_send_queue.Count > 0)
        {
            mSendMsg = TCP_send_queue.Dequeue();
            SendMessageInternal(mSendMsg);
        }
    }

    private void ConnectServerInternal()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ConnInternal))
            { 
                IsBackground = true
            };
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);   
        }
    }
    private void ConnInternal()
    {
        bool connect = true;
        TCP_send_queue.Enqueue("start");
        while (connect)
        {
            mNumConnection += 1;
            if (mNumConnection < mTimeout)
            {
                int i = 1;
                try
                {
                    socketConnection = new TcpClient(IP, PORT);
                    NetworkStream stream = socketConnection.GetStream();
                    mSendMsg = "start";
                    SendMessageInternal(mSendMsg);
                    ListenDataInternal();
                    connect = false;
                    i++;
                }
                catch (Exception e)
                {
                    Debug.Log("On client connect generation " + e);   

                }
            }
            else
            {
                clientReceiveThread.Abort();
                Debug.Log("TimeOut : Exit the game");
                Application.Quit();
            }
        }
    }

    private void ListenDataInternal()
    {
        mNumConnection = 1;
        Byte[] bytes = new byte[8192];
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanRead)
            {
                try { 
                    Debug.Log("connected");
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            mReceivedMsg = Encoding.UTF8.GetString(incomingData);
                            TCP_recv_queue.Enqueue(mReceivedMsg);
                        }
                    }
                }
                catch
                {
                    Debug.Log("lost connection");
                }
            }
        }
        catch
        {
            Debug.Log("lost connection");
        }
    }

    private void SendMessageInternal(string msg)
    {
        if (socketConnection == null)
        {
            Debug.Log("NO SERVER AVAILABLE");
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                byte[] clientMessageArr = Encoding.UTF8.GetBytes(msg);
                stream.Write(clientMessageArr, 0, clientMessageArr.Length);
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Write Error " + e);
        }
    }
    public void TxMsg(string msg)
    {
        //Debug.Log(msg);
        TCP_send_queue.Enqueue(msg);
    }
    public string RxMsg()
    {
        if (Rx_queue.Count > 0)
        {
            return Rx_queue.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public bool IsLastMsg()
    {
        return lastMsg && Rx_queue.Count == 0;
    }
}
