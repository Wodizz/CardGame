using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

namespace WodiServer
{
    /// <summary>
    /// 客户端对象封装
    /// </summary>
    public class ClientPeer
    {
        private Socket clientSocket;

        // 远程主机地址
        private IPEndPoint remoteIpEndPoint;

        // 单个数据的字节缓冲区
        private byte[] reciveBuffer = new byte[1024];

        // 所有数据的缓存区
        private List<byte> dataCache = new List<byte>();

        // 接收消息队列(解析完成后的SocketMessage)
        private Queue<SocketMessage> reciveMessageQuene = new Queue<SocketMessage>();

        // 发送消息队列(解析完成后的标准数据包)
        private Queue<byte[]> sendMessageQuene = new Queue<byte[]>();

        // 用于递归判断是否正在处理接收数据
        private bool isProcessRecive = false;

        // 用于递归判断是否正在处理发送数据
        private bool isProcessSend = false;

        // 客户端发送操作
        private SocketAsyncEventArgs sendArgs;

        // 客户端接收操作
        private SocketAsyncEventArgs reciveArgs;

        // 消息对象池
        private ObejctPool<SocketMessage> socketMessagePool = new ObejctPool<SocketMessage>(100);

        #region 外部属性

        /// <summary>
        /// 发送操作
        /// </summary>
        public SocketAsyncEventArgs SendArgs { get { return sendArgs; } }


        /// <summary>
        /// 接收操作
        /// </summary>
        public SocketAsyncEventArgs ReciveArgs { get { return reciveArgs; } }


        /// <summary>
        /// 接受消息队列
        /// </summary>
        public Queue<SocketMessage> ReciveMessageQuene
        {
            get { return reciveMessageQuene; }
            set { reciveMessageQuene = value; }
        }

        /// <summary>
        /// 与服务器连接断开回调事件
        /// </summary>
        public Action ConnectFailureEvent { get; set; }


       
        #endregion

        /// <summary>
        /// 构造连接对象
        /// </summary>
        public ClientPeer(string ip, int port)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sendArgs = new SocketAsyncEventArgs();
                reciveArgs = new SocketAsyncEventArgs();
                // 设置接收缓存区
                reciveArgs.SetBuffer(reciveBuffer, 0, reciveBuffer.Length);
                // 添加完成事件
                sendArgs.Completed += OnSendArgsCompleted;
                reciveArgs.Completed += OnReciveArgsCompleted;
                // 设置ip
                remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServer()
        {
            try
            {
                clientSocket.Connect(remoteIpEndPoint);
                if (clientSocket.Connected)
                {
                    Debug.Log("连接服务器成功！");
                    // 开始异步接收数据
                    StartRecive();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }

        #region 接收数据

        /// <summary>
        /// 等待接收服务器数据
        /// </summary>
        public void StartRecive()
        {
            try
            {
                // 可以直接使用封装好的SocketAsyncEventArgs异步套接字操作
                // 学习使用BeginReceive 最后一个object参数不起作用 可以传任何对象 类似SocketAsyncEventArgs中的UserToken
                //clientSocket.BeginReceive(reciveBuffer, 0, 1024, SocketFlags.None, ReciveCallBack, clientSocket);

                // Connected只会判断上一次状态
                bool isAsync = clientSocket.ReceiveAsync(reciveArgs);
                if (!isAsync)
                    ProcessRecive();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// 接收完成监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEvent"></param>
        private void OnReciveArgsCompleted(object sender, SocketAsyncEventArgs socketAsyncEvent)
        {
            ProcessRecive();
        }

        /// <summary>
        /// 接收数据后的回调函数
        /// 在一次reciveBuffer字节数组长度(1024)被填满时 就会执行一次
        /// </summary>
        /// <param name="asyncResult">异步操作结果</param>
        private void ProcessRecive()
        {
            try
            {
                if (reciveArgs.SocketError == SocketError.Success)
                {
                    // 接收结果成功 并且存在数据
                    if (reciveArgs.BytesTransferred > 0)
                    {
                        byte[] tempByte = new byte[reciveArgs.BytesTransferred];
                        // 拷贝至字节缓冲区
                        Buffer.BlockCopy(reciveBuffer, reciveArgs.Offset, tempByte, 0, reciveArgs.BytesTransferred);
                        // 拷贝至数据缓存区
                        dataCache.AddRange(tempByte);
                        // 进行数据解析
                        if (!isProcessRecive)
                            SloveRecive();
                        // 解析完成继续接收
                        StartRecive();
                    }
                    // 服务端主动返回0字节 主动断开连接
                    else if (reciveArgs.BytesTransferred == 0)
                        ServerDisconnect();
                    return;
                }
                Debug.Log("断开连接" + reciveArgs.SocketError.ToString());
                if (ConnectFailureEvent != null)
                    ConnectFailureEvent.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 数据解析
        /// </summary>
        private void SloveRecive()
        {
            isProcessRecive = true;
            // 解析成单个消息数据
            byte[] data = EncodeTool.DecodeMessage(ref dataCache);
            if (data == null)
            {
                isProcessRecive = false;
                return;
            }
            // 解析成socketmessage
            SocketMessage message = EncodeTool.DecodeSocketMessage(data);
            // 存储SocketMessage进入等待处理的队列
            reciveMessageQuene.Enqueue(message);
            //// 通知消息派发 
            // 不能这样写！！！ 不能在异步对象中 去调用unity主线程的事件！！
            //EventManager.MessageDistributeEvent.InvokeEvent();
            // 尾递归
            SloveRecive();
        }

        #endregion

        #region 发送数据

        /// <summary>
        /// 给服务端发送一条数据
        /// </summary>
        /// <param name="OpCode">主操作码</param>
        /// <param name="SubCode">子操作码</param>
        /// <param name="value">数据对象</param>
        public void SendMessageToServer(int OpCode, int SubCode, object value)
        {
            // 尝试从对象池获取SocketMessage
            SocketMessage message = socketMessagePool.TryGet();
            if (message == null)
                message = new SocketMessage(OpCode, SubCode, value);
            else
            {
                message.OperationCode = OpCode;
                message.SubOperationCode = SubCode;
                message.Value = value;
            }
            // 第一步 转换成字节数组
            byte[] messageByte = EncodeTool.EncodeSocketMessage(message);
            // 第二步 转换成标准数据包
            byte[] messageData = EncodeTool.EncodeMessage(messageByte);
            // 存入发送队列
            sendMessageQuene.Enqueue(messageData);
            // 回收消息对象
            socketMessagePool.Recyle(message);
            if (!isProcessSend)
                ProcessSend();
        }

        /// <summary>
        /// 处理发送消息操作
        /// </summary>
        private void ProcessSend()
        {
            isProcessSend = true;
            // 发送队列为空
            if (sendMessageQuene.Count == 0)
            {
                isProcessSend = false;
                return;
            }
            // 取出一条数据 存入发送操作
            byte[] sendData = sendMessageQuene.Dequeue();
            sendArgs.SetBuffer(sendData, 0, sendData.Length);
            bool isAsync = clientSocket.SendAsync(sendArgs);
            if (!isAsync)
                SendArgsCompleted();
        }

        /// <summary>
        /// 用于回调完成操作的封装
        /// </summary>
        private void OnSendArgsCompleted(object sender, SocketAsyncEventArgs args)
        {
            SendArgsCompleted();
        }


        /// <summary>
        /// 发送完成方法
        /// </summary>
        private void SendArgsCompleted()
        {
            if (sendArgs.SocketError != SocketError.Success)
            {
                // 打印发送失败原因
                Debug.LogError(sendArgs.SocketError.ToString());
                return;
            }
            // 否则继续发送
            ProcessSend();
        }

        #endregion

        #region 断开连接

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        public void ServerDisconnect()
        {
            try
            {
                // 清空缓存
                dataCache.Clear();
                sendMessageQuene.Clear();
                // 关闭
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
        }

        #endregion
    }
}

