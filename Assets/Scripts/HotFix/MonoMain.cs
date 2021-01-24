using System.IO;
using UnityEngine;
using XLua;

public class MonoMain : SingletnoAutoMono<MonoMain>
{
    private string buildTarget = "StandaloneWindows";
    public static LuaEnv LuaEnv = new LuaEnv();

    [Header("打开/关闭背包")]
    public KeyCode keyBag=KeyCode.B;
    private bool isOpenBag;
    private bool isOverCheck = false;

    private void Awake()
    { 
        LuaEnv.AddLoader(MyLoder); //添加loader
     
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOverCheck)
        {
            //打开关闭背包的操作
            BagOperation();
            MyActivityLua.Getinstate().LuaActivityDemo();//执行lua的活动脚本
        }
        
    }

    private void BagOperation()
    {


        if (Input.GetKeyDown(keyBag))
        {
            isOpenBag = !isOpenBag;
        }


        if (isOpenBag)
        {
            MyBagLua.Instance.LuaOpen();
        }
        else
        {
            MyBagLua.Instance.LuaClose();
        }


    }

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
        DownLoadMgr.Getinstate().InitCheckVersion("http://localhost:8081/AssetBundles/StandaloneWindows/VersionFile.txt", SucessCheck);
    }

    /// <summary>
    /// 完成版本检查的回调
    /// </summary>
    private void SucessCheck()
    {
        Debug.Log("完成版本检查");
        //AB主包的路径
        string ABmainPath = Application.persistentDataPath + "/AssetBundles/" + buildTarget + "/" + buildTarget + ".assetbundle";

        Debug.Log("AB主包加载路径：" + ABmainPath);
        ABManager.Instance.LoadMainAB(ABmainPath);//加载AB主包
        //ABManager.Instance.LoadResAsync<GameObject>("cube.assetbundle", "Cube", successTwo);//Cube测试
        ABManager.Instance.pathUrl = Application.persistentDataPath + "/AssetBundles/" + buildTarget + "/";//主包所在目录,提供给其余包加载
        //执行lua脚本（如果有热更新，替换脚本）
        LuaEnv.DoString("require 'main'");//所有lua脚本通过main执行，不进行混合开发
        MyBagLua.Instance.InitBagData();//初始化背包数据
        isOverCheck = true;
    }



    private byte[] MyLoder(ref string filePath)
    {
        string path = Application.persistentDataPath + "/AssetBundles/" + buildTarget  + "/Xlua/" + filePath + ".lua.txt";
        
        //string path = Application.dataPath + "/MyLoader/" + filePath + ".lua.txt";

        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(path));
    }
}