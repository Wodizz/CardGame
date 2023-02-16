using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Code;
using Protocol.Dto;

public class RegisterWindow : UIBase 
{
    private InputField AccountInput;
    private InputField PasswordInput;
    private InputField RePasswordInput;
    private Button RegisterBtn;
    private Button CloseBtn;

    private AccountDto registDto = new AccountDto();

    public override void AddEvent()
    {
        RegisterBtn.onClick.AddListener(OnRegisterBtnClicked);
        CloseBtn.onClick.AddListener(OnCloseBtnClicked);
    }

    public override void DelEvent()
    {
        RegisterBtn.onClick.RemoveListener(OnRegisterBtnClicked);
        CloseBtn.onClick.RemoveListener(OnCloseBtnClicked);
    }

    public override void FindUI()
    {
        AccountInput = transform.Find("Bg/Content/Account/AccountInput").GetComponent<InputField>();
        PasswordInput = transform.Find("Bg/Content/Password/PasswordInput").GetComponent<InputField>();
        RePasswordInput = transform.Find("Bg/Content/RePassword/RePasswordInput").GetComponent<InputField>();
        RegisterBtn = transform.Find("Bg/RegisterBtn").GetComponent<Button>();
        CloseBtn = transform.Find("Bg/CloseBtn").GetComponent<Button>();
    }

    private void OnRegisterBtnClicked()
    {
        if (string.IsNullOrEmpty(AccountInput.text) || string.IsNullOrEmpty(PasswordInput.text) || string.IsNullOrEmpty(RePasswordInput.text))
        {
            // 提示不能为空
            TipManager.Instance.ShowTip("帐号密码不能为空！", Color.red);
            return;
        }
        if (!string.Equals(PasswordInput.text, RePasswordInput.text))
        {
            // 提示一致
            TipManager.Instance.ShowTip("重复密码输入不一致！", Color.red);
            return;
        }
        registDto.account = AccountInput.text;
        registDto.password = PasswordInput.text;
        // 发送数据
        NetManager.Instance.Execute(OperationCode.ACCOUNT, AccountCode.REGIST_CREQ, registDto);
    }

    private void OnCloseBtnClicked()
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
