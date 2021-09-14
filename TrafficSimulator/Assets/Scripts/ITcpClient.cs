public interface ITcpClient
{
    void TxMsg(string msg);
    string RxMsg();
    bool IsLastMsg();
}
