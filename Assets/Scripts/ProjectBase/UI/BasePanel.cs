using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 面板基类 适用于UGUI
/// 帮助我们快速找到子控件
/// 方便我们在子类中处理逻辑
/// 节约找控件的工作量
/// </summary>
public class BasePanel : MonoBehaviour
{
    //通过里氏转换原则 来存储所有的控件 button image 等等的基类是 UIBehaviour
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    // Start is called before the first frame update
    void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
    }
    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe(params object[] array)//参数数组，方便打开面板时，选择性传递参数
    {

    }
    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe(params object[] array)
    {

    }



    /// <summary>
    /// 得到对应名字的控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                {
                    return controlDic[controlName][i] as T;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        string objName;

        for (int i = 0; i < controls.Length; i++)
        {
            objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);

            }
            else
            {
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            }



        }
    }
}
