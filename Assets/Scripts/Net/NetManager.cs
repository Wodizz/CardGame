using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WodiServer;

/// <summary>
/// 网络管理类
/// </summary>
public class NetManager : SingleThreadBase<NetManager>
{
    private ClientPeer client;

    public void Init()
    {
        client = new ClientPeer("127.0.0.1", 9999);
        client.ConnectServer();
        // 添加网络更新事件
        Main.OnGameUpdate += NetUpdate;
        // 添加退出断开连接事件
        Main.OnGameQuit += client.ServerDisconnect;
    }

    /// <summary>
    /// 客户端消息发送
    /// </summary>
    public void Execute(int opCode, int subCode, object obj)
    {
        client.SendMessageToServer(opCode, subCode, obj);
    }

    /// <summary>
    /// 得到一条SocketMessage
    /// </summary>
    /// <returns></returns>
    public SocketMessage GetSocketMessage()
    {
        return client.ReciveMessageQuene.Dequeue();
    }

    /// <summary>
    /// 网络更新
    /// </summary>
    private void NetUpdate()
    {
        if (client.ReciveMessageQuene.Count > 0)
        {
            // 消息中心派发消息
            MessageCenter.Instance.MessageDistribute(client.ReciveMessageQuene.Dequeue());
        }
    }
}
