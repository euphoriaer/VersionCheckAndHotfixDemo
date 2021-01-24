using UnityEngine;
using UnityEngine.Events;

public class MyBagLua : SingletnoAutoMono<MyBagLua>
{
    private GameObject Bag = null;

    public void InitBagData()
    {
        Debug.Log("初始化背包数据");
        //初始化面板
        ABUIManager.Instance.ShowPanel<UIBag>("ui/bag.assetbundle", "Bag", E_UI_Layer.Top, (bag) =>
        {
            Debug.Log("成功初始化背包，按B打开");
            GameObject bagFather = new GameObject();
            bagFather.transform.SetParent(bag.transform.parent, false);
            bag.transform.SetParent(bagFather.transform);
            
            Bag = bag;

            bag.GetComponent<UIBag>().AddSlotTitle("BagTitle", "ui/bag.assetbundle", (title) =>
            {
                title.transform.SetParent(bag.transform.parent,false);
            });//加载背包标题


            Bag.GetComponent<UIBag>().AddSlot("BloodSlot", 1, "ui/solt.assetbundle", (obj) =>
            {
                obj.transform.SetParent(Bag.transform,false);
            });//todo 初始化格子 应该来自服务器信息读取，还应该有一个物品池（总的物品配置文件）作为玩家可能获得的道具列表,这里直接添加格子

            bagFather.SetActive(false);


        });

       

    }

    /// <summary>
    /// 主函数调用的接口,同时提供给lua修改的接口（添加物品）
    /// </summary>
    public void LuaOpen()
    {
        if (Bag != null)
        {
            Bag.transform.parent.gameObject.SetActive(true);
            Debug.Log("打开背包");
        }
      

        ////加载背包界面
       
    }

    /// <summary>
    /// 提供给主函数以及lua
    /// </summary>
    public void LuaClose()
    {
        if (Bag != null)
        {
            Bag.transform.parent.gameObject.SetActive(false);
            Debug.Log("关闭背包");
        }
    }
    /// <summary>
    /// todo 提供给lua 添加格子对象（直接展示） ，
    /// </summary>
    public void LuaAddSolt()
    {
        // AddSlot("BloodSlot", 1, "ui/solt.assetbundle");
    }

    /// <summary>
    /// 封装添加格子的方法
    /// </summary>
    /// <param name="BloodSlot"></param>
    /// <param name="BloodSlotNum"></param>
    /// <param name="path"></param>
    private void AddSlot(string BloodSlot, int BloodSlotNum, string path)
    {
        Bag.GetComponent<UIBag>().AddSlot(BloodSlot, BloodSlotNum, path, (obj) =>
        {
            obj.transform.SetParent(Bag.transform, false);
        });//添加格子
    }

    /// <summary>
    /// 私有不允许lua修改，封装打开的方法
    /// </summary>
    /// <param name="assetbundle"></param>
    /// <param name="objectName"></param>
    /// <param name="e_UI_Layer"></param>
    /// <param name="SuccessOpen"></param>
    private void Open(string assetbundle, string objectName, E_UI_Layer e_UI_Layer, UnityAction<GameObject> SuccessOpen)
    {
        ABUIManager.Instance.ShowPanel<UIBag>(assetbundle, objectName, e_UI_Layer, SuccessOpen);
    }

    /// <summary>
    /// 背包加载完成的回调
    /// </summary>
    /// <param name="arg0"></param>
    private void SuccessPanel(GameObject arg0)
    {
        if (Bag==null)
        {
        Bag = arg0;//传出加载完成的背包对象

        }
      
        
        //arg0.GetComponent<UIBag>().AddSlot("BloodSlot", 1, "ui/solt.assetbundle", (obj) =>
        //{
        //    obj.transform.SetParent(arg0.transform, false);
        //});//todo 添加格子
        //   //设置父物体

        
    }
}