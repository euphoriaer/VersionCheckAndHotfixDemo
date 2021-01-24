//如果涉及到Assembly-CSharp.dll之外的其它dll，如下代码需要放到Editor目录
using XLua;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class HotfixCfg
{
    [Hotfix]
    public static List<Type> by_field = new List<Type>()
    {
        typeof (MonoMain),//背包热更
         typeof (MyActivityLua),//活动热更
    };

    // [Hotfix]
    // public static List<Type> by_property
    // {
    //     get
    //     {
    //         return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
    //                 where type.Namespace == "XXXX"
    //                 select type).ToList();
    //     }
    // }
}