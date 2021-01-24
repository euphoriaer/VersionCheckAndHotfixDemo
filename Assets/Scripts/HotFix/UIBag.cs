using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class UIBag:BasePanel
    {

    public Dictionary<string ,GameObject > dicSolt = new System.Collections.Generic.Dictionary<string, GameObject>();//管理背包全部的格子，不考虑出现相同格子dic增加效率，考虑相同格子用list维护
   
    public override void ShowMe(params object[] array)
    {
        base.ShowMe();

       
    }

    /// <summary>
    ///   加载背包的标题
    /// </summary>
    /// <param name="bagtitle">背包名</param>
    /// <param name="bagtitleAB">AB包名：默认ui/bag</param>
    public void AddSlotTitle(string bagtitle, string bagtitleAB ,UnityAction<GameObject> sucessTitle)
    {
        //"ui/solt.assetbundle"
        ABManager.Instance.LoadResAsync<GameObject>(bagtitleAB, bagtitle, sucessTitle);//加载背包的标题


    }

   

    /// <summary>
    ///   添加格子
    /// </summary>
    /// <param name="soltName">格子名</param>
    /// <param name="abName">ab包名：默认ui/solt.assetbundle</param>
    public void AddSlot(string soltName, int number,string abName,UnityAction<GameObject> sucessSlot)
    {
        //"ui/solt.assetbundle"
        ABManager.Instance.LoadResAsync<GameObject>(abName, soltName, (arg0)=> {
            Debug.Log("成功加载格子");//此处会创造一个实例格子
            dicSolt.Add(arg0.name, arg0);//加入到格子列表
            arg0.GetComponent<UISolt>().ShowMe(number);                             //设置数据（数量）
            sucessSlot(arg0);//把格子传给外边调用
        });//加载AB包中的格子


        }

    /// <summary>
    /// 删除格子
    /// </summary>
    /// <param name="soltName">格子名</param>
    public void DeleteSlot(string soltName)
    {
        if (dicSolt.ContainsKey(soltName))
        {

            Destroy(dicSolt[soltName]);
        }
        else
        {
            Debug.Log("不存此在格子");
        }
    }

}

