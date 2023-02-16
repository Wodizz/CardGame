using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;


/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesManager
{

    public static void Init()
    {
        // 添加加载完成事件
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    /// <summary>
    /// 加载完成统一执行
    /// </summary>
    private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Main.CurScene = (E_Scene)Enum.Parse(typeof(E_Scene), scene.name);
        Main.SceneInit();
    }

    /// <summary>
    /// 同步切换场景方法
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="callback">回调</param>
    public static void LoadScene(string name, Action callback = null)
    {
        // 场景同步加载
        SceneManager.LoadScene(name);
        if (callback != null)
            callback();
    }

    /// <summary>
    /// 异步切换场景方法
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="callback">回调</param>
    public static void LoadSceneAsyn(string name, OnValueChanged callback = null)
    {
        Main.MainMono.StartCoroutine(LoadSceneAsynByCoroutine(name, callback));
    }


    private static IEnumerator LoadSceneAsynByCoroutine(string name, OnValueChanged callback = null)
    {
        // AsyncOperation异步操作类
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            // 触发进度条事件
            
            // 如果没完成 返回进度值
            yield return ao.progress;
        }
        yield return null;
        if (callback != null)
            callback();
    }

    
}
