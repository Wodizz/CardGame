using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : UIBase 
{
    private Button StartBtn;
    private Button RegiestBtn;

    public override void FindUI()
    {
        StartBtn = transform.Find("StartBtn").GetComponent<Button>();
        RegiestBtn = transform.Find("RegiestBtn").GetComponent<Button>();
    }

    public override void AddEvent()
    {
        StartBtn.onClick.AddListener(OnStartBtnClicked);
        RegiestBtn.onClick.AddListener(OnRegiestBtnClicked);
        EventManager.LoginSuccessEvent.AddListener(OnLoginSuccessEvent);
    }

    public override void DelEvent()
    {
        StartBtn.onClick.RemoveListener(OnStartBtnClicked);
        RegiestBtn.onClick.RemoveListener(OnRegiestBtnClicked);
        EventManager.LoginSuccessEvent.DelListener(OnLoginSuccessEvent);
    }

    private void OnStartBtnClicked()
    {
        UIManager.CreateWindow<LoginWindow>(E_WindowType.Pop);
    }

    private void OnRegiestBtnClicked()
    {
        UIManager.CreateWindow<RegisterWindow>(E_WindowType.Pop);
    }

    private void OnLoginSuccessEvent()
    {
        UIManager.HideUI(this);
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {

    }

}
