using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : SingleBase<TipManager>
{
    
    private Queue<TipInfo> tipQuene = new Queue<TipInfo>();

    private bool isShowTip = false;

    /// <summary>
    /// 提示消息队列
    /// </summary>
    public Queue<TipInfo> TipQuene
    {
        get
        {
            return tipQuene;
        }

        set
        {
            tipQuene = value;
        }
    }

    /// <summary>
    /// 是否展示提示
    /// </summary>
    public bool IsShowTip
    {
        get
        {
            return isShowTip;
        }

        set
        {
            isShowTip = value;
        }
    }


    /// <summary>
    /// 展示一条提示信息
    /// </summary>
    /// <param name="tip">内容</param>
    /// <param name="color">颜色</param>
    public void ShowTip(string tip, Color color)
    {
        TipInfo tipInfo = new TipInfo(tip, color);
        tipQuene.Enqueue(tipInfo);
        if (!isShowTip)
            UIManager.CreateWindow<TipWindow>(E_WindowType.System);
    }
}

public class TipInfo
{
    string tip;
    Color color;

    public TipInfo(string tip, Color color)
    {
        this.Tip = tip;
        this.Color = color;
    }

    #region 属性

    public string Tip
    {
        get
        {
            return tip;
        }

        set
        {
            tip = value;
        }
    }

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
        }
    }

    #endregion


}
