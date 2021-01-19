using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



/// <summary>
/// ResUI管理器
/// 管理所有显示的面板
/// 提供给外部 显示和隐藏等等的接口
/// </summary>
public class ResUIManager : BaseManager<ResUIManager>
{
    //里式转换原则，用基类装子类
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform canvas;

    private Transform bot;
    private Transform Mid;
    private Transform top;
    private Transform system;

    public ResUIManager()
    {
        //创建 Canvas 让其过场景不被移除
       GameObject obj= ResMgr.Getinstate().Load<GameObject>("UI/Canvas");
        canvas = obj.transform;
        
        GameObject.DontDestroyOnLoad(obj);
        //找到各层
        bot = canvas.Find("Bot");
        Mid = canvas.Find("Mid");
        top = canvas.Find("top");
        system = canvas.Find("system");


        //创建EventSystem 让过场景不溢出
        obj = ResMgr.Getinstate().Load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);


    }
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callback">当面板预设体创建成功后，你想做的事</param>
    public void ShowPanel<T>(string panelName,E_UI_Layer layer,UnityAction<T>callback) where T:BasePanel
    {
        ResMgr.Getinstate().LoadAsync<GameObject>("UI/" + panelName, (obj) =>
           {
               panelDic[panelName].ShowMe();
               //把他作为 Canvas 的子对象
               //并且 要设置它的相对位置
               //找到父对象 显示到哪一层
               Transform father = bot;

               switch (layer)
               {
               
                   case E_UI_Layer.Mid:
                       father = Mid;
                       break;
                   case E_UI_Layer.Top:
                       father = top;
                       break;
                   case E_UI_Layer.System:
                       father = system;
                       break;
                   default:
                       break;
               }
               //设置父对象 设置相对位置和大小
               obj.transform.SetParent(father);
               obj.transform.localScale = Vector3.one;

               (obj.transform as RectTransform).offsetMax = Vector2.zero;
               (obj.transform as RectTransform).offsetMin = Vector2.zero;
               //得到预设体身上的面板脚本
               T panel = obj.GetComponent<T>();
               //处理面板完成后的逻辑
               if (callback!=null)
               {
                   callback(panel);
               }
               panel.ShowMe();
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
            panelDic[panelName].HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

}
