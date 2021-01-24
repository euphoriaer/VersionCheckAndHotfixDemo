using UnityEngine;
using UnityEngine.UI;

internal class UISolt : BasePanel
{
    public Image Image;
    public int Number;

    /// <summary>
    /// 显示格子并初始化数据
    /// </summary>
    /// <param name="array"></param>
    public override void ShowMe(params object[] array)
    {
        base.ShowMe();

        SetNumber((int)array[0]);
        Debug.Log("显示格子");
    }

    private void SetNumber(int number)
    {
        Text Number = GetControl<Text>("Number");
        Number.text = number.ToString();
    }

    //todo 双击检测
}