using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
public interface IEventInfo
{

}

public class EventInfo<T>: IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        action += action;

    }

}

public class EventInfo: IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        action += action;

    }

}/// <summary>
/// 事件中心， 单例模式对象
/// 1.字典 Dictionary
/// 2.委托
/// 3.观察者设计模式
/// 4.泛型
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //Key 对应事件的名字
    //value 对应监听事件的函数
    private Dictionary<string,IEventInfo> eventDic = new Dictionary<string, IEventInfo>();
    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {//有对应的监听事件
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else//没有对应的监听事件
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }

    }
    /// <summary>
    /// 添加不需要参数的监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(string name, UnityAction action)
    {//有对应的监听事件
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else//没有对应的监听事件
        {
            eventDic.Add(name, new EventInfo(action));
        }

    }
    /// <summary>
    /// 移除事件的监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">对应之前添加的委托函数</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;

        }
    }
    /// <summary>
    /// 移除不需要参数的监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;

        }
    }
    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">哪一个名字的事件触发了</param>
    public void EventTrigger<T>(string name,T info)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();  同下，调用委托函数
            if ((eventDic[name] as EventInfo<T>).actions!=null)
            (eventDic[name] as EventInfo<T>).actions.Invoke(info);

            //eventDic[name].Invoke(info);//依次执行，先+=进来的先执行
        }
    }
    /// <summary>
    /// 触发不需要参数的事件
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(string name )
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();  同下，调用委托函数
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();

            //eventDic[name].Invoke(info);//依次执行，先+=进来的先执行
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}
