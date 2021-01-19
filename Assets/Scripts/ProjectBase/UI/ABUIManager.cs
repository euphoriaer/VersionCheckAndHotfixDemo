using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// ABUI管理器
/// 管理所有显示的面板
/// 提供给外部 显示和隐藏等等的接口
/// </summary>
public class ABUIManager : SingletnoAutoMono<ABUIManager>
{
    //里式转换原则，用基类装子类
    public Dictionary<string, GameObject> panelDic = new Dictionary<string, GameObject>();

    private Transform canvas;

    private Transform Bot;
    private Transform Mid;
    private Transform Top;
    private Transform System;
    private void Start()
    {
       
    }
    public void init()
    {
        //创建 Canvas 让其过场景不被移除
        GameObject obj = ResMgr.Getinstate().Load<GameObject>("UI/AB_BagCanvas");
        canvas = obj.transform;

        GameObject.DontDestroyOnLoad(obj);
        //找到各层
        Bot = canvas.Find("Bot");
        Mid = canvas.Find("Mid");
        Top = canvas.Find("Top");
        System = canvas.Find("System");



        //创建EventSystem 让过场景不溢出
        obj = ResMgr.Getinstate().Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
    }
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="ABname">AB包名</param>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪层</param>
    /// <param name="callback">回调函数对实例化的物体操作</param>
    public void ShowPanel<T>(string ABname,string panelName, E_UI_Layer layer, UnityAction<GameObject> callback) where T : BasePanel
    {
        init();
        ABManager.Instance.LoadResAsync<GameObject>(ABname, panelName,(obj) =>
        {
            if (panelDic.ContainsKey(panelName))
            {
                if (panelDic[panelName].GetComponent<T>()==null)
                {
                    panelDic[panelName].AddComponent<T>();
                }
                panelDic[panelName].GetComponent<UIBag>().ShowMe();//有面板信息，直接打开

            }
            //把他作为 Canvas 的子对象
            //并且 要设置它的相对位置
            //找到父对象 显示到哪一层
            Transform father = Bot;

            switch (layer)
            {

                case E_UI_Layer.Mid:
                    father = Mid;
                    break;
                case E_UI_Layer.Top:
                    father = Top;
                    break;
                case E_UI_Layer.System:
                    father = System;
                    break;
                default:
                    break;
            }
            //设置父对象 设置相对位置和大小
            obj.transform.SetParent(father, false);
            obj.transform.localScale = Vector3.one;


            //得到预设体身上的面板脚本
            GameObject panel = obj;
            panel.AddComponent<T>();
            //处理面板完成后的逻辑
            if (callback != null)
            {
                callback(panel);
            }
            panel.GetComponent<UIBag>().ShowMe();
            //把面板存起来
            panelDic.Add(panelName, panel);

        });

    }


    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].GetComponent<UIBag>().HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

}

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System,
}
