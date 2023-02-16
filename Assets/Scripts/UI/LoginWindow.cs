using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Code;
using Protocol.Dto;

public class LoginWindow : UIBase 
{
    private InputField AccountInput;
    private InputField PasswordInput;
    private Button LoginBtn;
    private Button CloseBtn;
    // 当一个消息频繁发送时 可以提取一下 节省性能
    private AccountDto loginDto = new AccountDto();

    public override void AddEvent()
    {
        LoginBtn.onClick.AddListener(OnLoginBtnClicked);
        CloseBtn.onClick.AddListener(OnCloseBtnClicked);
        EventManager.LoginSuccessEvent.AddListener(OnLoginSuccessEvent);
    }

    public override void DelEvent()
    {
        LoginBtn.onClick.RemoveListener(OnLoginBtnClicked);
        CloseBtn.onClick.RemoveListener(OnCloseBtnClicked);
        EventManager.LoginSuccessEvent.DelListener(OnLoginSuccessEvent);
    }

    public override void FindUI()
    {
        AccountInput = transform.Find("Bg/Content/Account/AccountInput").GetComponent<InputField>();
        PasswordInput = transform.Find("Bg/Content/Password/PasswordInput").GetComponent<InputField>();
        LoginBtn = transform.Find("Bg/LoginBtn").GetComponent<Button>();
        CloseBtn = transform.Find("Bg/CloseBtn").GetComponent<Button>();
    }

    private void OnLoginBtnClicked()
    {
        if (string.IsNullOrEmpty(AccountInput.text) || string.IsNullOrEmpty(PasswordInput.text))
        {
            TipManager.Instance.ShowTip("帐号密码不能为空！", Color.red);
            return;
        }
        // 生成一个帐号数据传输对象
        loginDto.account = AccountInput.text;
        loginDto.password = PasswordInput.text;
        // 发送对应操作数据
        NetManager.Instance.Execute(OperationCode.ACCOUNT, AccountCode.LOGIN_CREQ, loginDto);
    }

    private void OnCloseBtnClicked()
    {
        UIManager.HideUI(this);
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
