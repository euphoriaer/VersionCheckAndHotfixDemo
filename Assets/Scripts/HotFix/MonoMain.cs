using System;
using UnityEngine;

public class MonoMain : SingletnoAutoMono<MonoMain>
{
    private string buildTarget = "StandaloneWindows";

    // Start is called before the first frame update
    private void Start()
    {
#if UNITY_EDITOR || UNITY_STANDLONE_WIN
        buildTarget = "StandaloneWindows";
#elif UNITY_ANDROID
    buildTarget = "Android";
#elif UNITY_IPHONE
    buildTarget = "IOS";
#endif
        //版本检查
        DownLoadMgr.Getinstate().InitCheckVersion("http://localhost:8081/AssetBundles/StandaloneWindows/VersionFile.txt",SucessCheck);

       

    }

    private void SucessCheck()
    {
        Debug.Log("完成版本检查");
        //加载AB包
        string ABmainPath = Application.persistentDataPath + "/AssetBundles/" + buildTarget + "/" + buildTarget + ".assetbundle";


        Debug.Log("AB主包加载路径：" + ABmainPath);
        ABManager.Instance.LoadMainAB(ABmainPath);
        ABManager.Instance.LoadResAsync<GameObject>("cube.assetbundle", "Cube", successTwo);//Cube测试
        ABManager.Instance.pathUrl = Application.persistentDataPath + "/AssetBundles/" + buildTarget + "/";//主包所在目录


        ////加载背包界面
        ABUIManager.Instance.ShowPanel<UIBag>("ui/bag.assetbundle", "Bag", E_UI_Layer.Top, SuccessPanel);
    }

    private void successTwo(GameObject arg0)
    {
        
    }

    private void SuccessPanel(GameObject arg0)
    {
        Debug.Log("成功加载背包");
        arg0.GetComponent<UIBag>().AddSlotTitle("BagTitle", "ui/bag.assetbundle", (obj) => {
            obj.transform.SetParent(arg0.transform.parent, false);
        });//加载背包标题
        arg0.GetComponent<UIBag>().AddSlot("BloodSlot", 1,"ui/solt.assetbundle",(obj)=> {
            obj.transform.SetParent(arg0.transform,false);
        });//todo 添加格子
       //设置父物体


    }

}