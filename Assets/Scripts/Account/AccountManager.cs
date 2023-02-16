using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AccountManager : SingleBase<AccountManager>
{
    /// <summary>
    /// 响应登录
    /// </summary>
    /// <param name="value">错误码</param>
    public void LoginResponse(int value)
    {
        switch (value)
        {
            case -1:
                TipManager.Instance.ShowTip("不存在该帐号！", Color.red);
                break;
            case -2:
                TipManager.Instance.ShowTip("帐号密码不匹配！", Color.red);
                break;
            case -3:
                TipManager.Instance.ShowTip("该帐号已登录！", Color.red);
                break;
            case 0:
                TipManager.Instance.ShowTip("登录成功！", Color.green);
                // 通知登录成功
                EventManager.LoginSuccessEvent.InvokeEvent();
                // 跳转场景
                ScenesManager.LoadScene(E_Scene.Game.ToString());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 响应注册
    /// </summary>
    /// <param name="value">错误码</param>
    public void RegistResponse(int value)
    {
        switch (value)
        {
            case -1:
                TipManager.Instance.ShowTip("已存在该帐号！", Color.red);
                break;
            case -2:
                TipManager.Instance.ShowTip("帐号密码不合法！", Color.red);
                break;
            case 0:
                TipManager.Instance.ShowTip("注册成功！", Color.green);
                break;
            default:
                break;
        }
    }
}
