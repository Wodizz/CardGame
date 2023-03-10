using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_WindowType
{
	Base,
	Menu,
	Pop,
	System
}


/// <summary>
/// UI窗口管理类
/// </summary>
public class UIManager
{
	// UI字典
	private static Dictionary<string, ObejctPool<UIBase>> uiPool = new Dictionary<string, ObejctPool<UIBase>>();
	// 同时只有一个画布
	public static GameObject Canvas;
	public static GameObject EventSystem;
	public static Camera UICamera;
	private static Transform Base;
	private static Transform Menu;
	private static Transform Pop;
	private static Transform System;

	public static void Init() 
    {
		// 生成UI画布
		Canvas = ResourceManager.Instance.Load<GameObject>("UI/Canvas");
		EventSystem = ResourceManager.Instance.Load<GameObject>("UI/EventSystem");

		// 找到各层级
		Base = Canvas.transform.Find("Base");
		Menu = Canvas.transform.Find("Menu");
		Pop = Canvas.transform.Find("Pop");
		System = Canvas.transform.Find("System");

		// 找到UI摄像机
		UICamera = Canvas.transform.Find("UICamera").GetComponent<Camera>();
	}

	/// <summary>
	/// 创建窗口
	/// </summary>
	public static T CreateWindow<T>(E_WindowType windowType) where T : UIBase
    {
		string uiName = typeof(T).Name;
		T window;
		// 存在key 从对应池子中拿ui
		if (uiPool.ContainsKey(uiName))
			window = uiPool[uiName].TryGet() as T;
		// 不存在则加载
        else
        {
			window = ResourceManager.Instance.Load<GameObject>("UI/" + uiName).GetComponent<T>();
			// 只有第一次加载才会去找ui
			window.FindUI();
		}
		// 设置父对象
		window.transform.SetParent(GetWindowParent(windowType));
		// 获取窗口UI盒子
		RectTransform _windowRect = window.GetComponent<RectTransform>();
		// 设置窗口的默认大小
		_windowRect.sizeDelta = Vector2.zero;
		_windowRect.localScale = Vector3.one;
		// 调用窗口生命周期函数
		window.OnShow();
		window.AddEvent();
		return window;
	}

	/// <summary>
	/// 创建ui道具
	/// </summary>
	public static T CreateItem<T>(Transform uiParent) where T : UIBase
    {
		string uiName = typeof(T).Name;
		T item;
		// 存在key 从对应池子中拿ui
		if (uiPool.ContainsKey(uiName))
			item = uiPool[uiName].TryGet() as T;
		// 不存在则加载
		else
		{
			item = ResourceManager.Instance.Load<GameObject>("UI/" + uiName).GetComponent<T>();
			// 只有第一次加载才会去找ui
			item.FindUI();
		}
		// 设置父对象
		item.transform.SetParent(uiParent);
		// 获取UI盒子
		RectTransform _itemRect = item.GetComponent<RectTransform>();
		// 设置默认大小
		_itemRect.sizeDelta = Vector2.zero;
		_itemRect.localScale = Vector3.one;
		// 调用生命周期函数
		item.OnShow();
		item.AddEvent();
		return item;
	}


	/// <summary>
	/// 回收UI
	/// </summary>
	public static void HideUI<T>(UIBase uibase) where T :UIBase
    {
		string uiName = (uibase as T).GetType().Name;
		// 如果不存在对应窗口节点 就创建一个新池子
		if (!uiPool.ContainsKey(uiName))
			uiPool.Add(uiName, new ObejctPool<UIBase>(uiName));
		// 调用生命周期函数
		uibase.OnHide();
		uibase.DelEvent();
		// 入池
		uiPool[uiName].Recyle(uibase);
    }


	/// <summary>
	/// 回收UI
	/// </summary>
	public static void HideUI(UIBase uibase)
	{
		string uiName = uibase.GetType().Name;
		// 如果不存在对应窗口节点 就创建一个新池子
		if (!uiPool.ContainsKey(uiName))
			uiPool.Add(uiName, new ObejctPool<UIBase>(uiName));
		// 调用生命周期函数
		uibase.OnHide();
		uibase.DelEvent();
		// 入池
		uiPool[uiName].Recyle(uibase);
	}


	/// <summary>
	/// 清空ui池
	/// </summary>
	public static void Clear()
    {
        foreach (var item in uiPool)
        {
			item.Value.Clear();
		}
		uiPool.Clear();
    }

	/// <summary>
	/// 得到ui节点
	/// </summary>
	public static Transform GetWindowParent(E_WindowType windowType)
    {
		Transform parent = null;
        switch (windowType)
        {
            case E_WindowType.Base:
				parent = Base;
				break;
            case E_WindowType.Menu:
				parent = Menu;
				break;
            case E_WindowType.Pop:
				parent = Pop;
				break;
            case E_WindowType.System:
				parent = System;
				break;
            default:
                break;
        }
		return parent;
    }
}
