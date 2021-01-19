using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和lambda表达式，lambda表达式在使用中模块时，避免重复创建函数（异步加载要有一个委托函数，需要做的事的函数）
/// 3.协程
/// 4.泛型
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {

        T res = Resources.Load<T>(name);
        //如果对象是一个Gameobject类型的  实例化后  再返回  外部可以直接使用
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);//实例化返回
        }
        else
            return res;
    }

    //异步加载资源
    public void LoadAsync<T>(string name,UnityAction<T> callback) where T : Object
    {
        Resources.LoadAsync(name);
        //开启异步加载的协程
        MonoMgr.Getinstate().StartCoroutine(ReallyLoadAsync(name,callback));
    }
 //真正的协同函数 用于 开启异步加载对应的资源
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T:Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
        {

            callback(GameObject.Instantiate(r.asset) as T);

        }
        else
            callback(r.asset as T);
    }
}
