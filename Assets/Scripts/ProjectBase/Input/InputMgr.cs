using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1.Input 类
/// 2.事件中心模块
/// 3.公共Mono模块
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    private bool isStar = false;
    /// <summary>
    /// 构造函数中 添加 Update 监听
    /// </summary>
    public InputMgr()
    {
        MonoMgr.Getinstate().AddUpdateListener(MyUpdate);
    }
    public void StarOrEndCheck(bool isOpen)
    {
        isStar = isOpen;
    }

    /// <summary>
    /// 检测按键 分发事件
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            //事件中心模块 分发按下事件
            EventCenter.Getinstate().EventTrigger("某键被按下", key);
        }
        if (Input.GetKeyUp(key))
        {
            //事件中心模块 分发
            EventCenter.Getinstate().EventTrigger("某键被按下", key);
        }
    }
    private void MyUpdate()
    {//没有开启就直接 return 不检测
        if (isStar)
        {
            return;
        }
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);


    }
}
