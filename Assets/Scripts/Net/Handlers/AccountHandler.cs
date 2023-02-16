using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Protocol.Code;
using UnityEngine;

/// <summary>
/// 帐号处理模块 用于网络层与游戏层的交互
/// </summary>
public class AccountHandler : HandlerBase
{
    public override void OnRecive(int subCode, object obj)
    {
        switch (subCode)
        {
            case AccountCode.LOGIN_SRES:
                AccountManager.Instance.LoginResponse((int)obj);
                break;
            case AccountCode.REGIST_SRES:
                AccountManager.Instance.RegistResponse((int)obj);
                break;
            default:
                break;
        }
    }

    
}
