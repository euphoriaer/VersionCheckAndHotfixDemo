using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

/// <summary>
/// 1.给外部添加帧更新事件的方法
/// 2.给外部添加 协程的方法
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    public MonoControner controller;
    public MonoMgr()
    {
        //保证了Monocontroller对象的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoControner>();

    }
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }
    /// <summary>
    /// 给外部添加，用于移除帧更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);

    }
    /// <summary>
    /// 协程的封装
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);

    }
    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }
    public Coroutine StartCoroutine_Auto(IEnumerator routine)
    {
        return StartCoroutine_Auto(routine);
    }
}
