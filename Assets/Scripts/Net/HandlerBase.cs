using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 客户端处理基类
/// </summary>
public abstract class HandlerBase
{
    /// <summary>
    /// 消息接收
    /// </summary>
    public abstract void OnRecive(int subCode, object obj);

    /// <summary>
    /// 消息派发
    /// </summary>
    protected virtual void Dispatch(int opCode, int subCode, object obj)
    {
        NetManager.Instance.Execute(opCode, subCode, obj);
    }
}
