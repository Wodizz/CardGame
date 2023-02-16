using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipWindow : UIBase 
{
    private Text tipText;
    private TipInfo tipInfo;
    private Animator tipWindowAnimator;

    public override void AddEvent()
    {
        tipWindowAnimator.AddAnimEvent("TipAnimation", OnTipAnimationEnd);
    }

    public override void DelEvent()
    {
        tipWindowAnimator.CleanAllEvent();
    }

    private void OnTipAnimationEnd()
    {
        tipWindowAnimator.Play("TipAnimation", 0, 0);
        OnShow();
    }

    public override void FindUI()
    {
        tipText = transform.Find("TipBg/TipTxt").GetComponent<Text>();

        tipWindowAnimator = GetComponent<Animator>();
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        TipManager.Instance.IsShowTip = true;
        // 没有要展示的了
        if (TipManager.Instance.TipQuene.Count == 0)
        {
            TipManager.Instance.IsShowTip = false;
            UIManager.HideUI(this);
            return;
        }
        // 取出一条提示信息
        tipInfo = TipManager.Instance.TipQuene.Dequeue();
        tipText.text = tipInfo.Tip;
        tipText.color = tipInfo.Color;
    }

}
