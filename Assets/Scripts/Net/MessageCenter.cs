using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Protocol.Code;

namespace WodiServer
{
    /// <summary>
    /// 消息中心 根据对应消息 去分发操作
    /// </summary>
    public class MessageCenter : SingleBase<MessageCenter>
    {

        /// <summary>
        /// 消息派发
        /// </summary>
        public void MessageDistribute(SocketMessage message)
        {
            switch (message.OperationCode)
            {
                case OperationCode.ACCOUNT:
                    accountHandler.OnRecive(message.SubOperationCode, message.Value);
                    break;
                default:
                    break;
            }
        }

        #region 模块声明

        AccountHandler accountHandler = new AccountHandler();

        #endregion

    }
}
