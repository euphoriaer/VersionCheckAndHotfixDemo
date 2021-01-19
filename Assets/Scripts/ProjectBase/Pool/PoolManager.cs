using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据，池子中的一列数据
/// </summary>
public class PoolDate
{
    public GameObject fatherObj;//抽屉中对象挂在的父节点
    public List<GameObject> poolList;//对象的容器

    public PoolDate(GameObject obj, GameObject poolObj)
    {//给抽屉创造一个父对象，并且将其作为pool（衣柜）的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;


        poolList = new List<GameObject>() { };
        PushObj(obj);//创建衣柜时将第一个物体压进去
    }
    /// <summary>
    /// 压东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);//失活，隐藏
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 从抽屉里取东西
    /// </summary>
    /// <param name="name"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public GameObject GetObj()
    {



        GameObject obj = null;

        obj = poolList[0];
        poolList.RemoveAt(0);


        obj.SetActive(true);
        //激活显示
        obj.transform.parent = null;//断开父子关系
        return obj;

    }

}
public class PoolMgr : BaseManager<PoolMgr>
{
    //缓存池容器，减少GC次数，用内存换不卡（减少GC回收，大量物体使GC频繁回收，会显得卡顿）
    //需要知识字典，列表

    public GameObject poolObj;



    public Dictionary<string, PoolDate> poolDic = new Dictionary<string, PoolDate>();
    /// <summary>
    /// 往外拿东西,东西名字(路径)，拿完之后要执行的函数 重载1创造的第一个物体啥也不干（默认创造在世界中心）
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callback)
    {



      //有抽屉，抽屉里有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)//有抽屉并且有东西
        {

            callback(poolDic[name].GetObj());


        }
        else
        {//通过异步加载资源 创建对象给外部用
            ResMgr.Getinstate().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
             

            });
           

            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name), transform);//从文件实例化对象
            ////把对象名字改成池子的名字
            //obj.name = name;

        }

      

    }
    /// <summary>
    /// 往外拿东西 1东西名字（路径），2每次拿完之后要执行的函数，3创造的第一个物体要执行的函数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <param name="first"></param>
    public void GetObj(string name, UnityAction<GameObject> callback, UnityAction<GameObject> first)
    {



        //有抽屉，抽屉里有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)//有抽屉并且有东西
        {

            callback(poolDic[name].GetObj());


        }
        else
        {//通过异步加载资源 创建对象给外部用
            ResMgr.Getinstate().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                first(o);

            });


            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name), transform);//从文件实例化对象
            ////把对象名字改成池子的名字
            //obj.name = name;

        }



    }
    /// <summary>
    /// 换暂时不用的东西给我，往里存东西
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void PushObj(string name, GameObject obj)
    {//里面有抽屉

        if (poolObj == null)
        {
            poolObj = new GameObject("pool");


        }
        obj.transform.parent = poolObj.transform;//设置父对象
        obj.SetActive(false);//失活，隐藏
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        //里面没有抽屉，加入一个
        else
        {
            poolDic.Add(name, new PoolDate(obj,poolObj));
        }
    }
    /// <summary>
    /// 清空缓存池
    /// </summary>
    public void clear()
    {
        poolDic.Clear();
        poolObj = null;
    }

}
