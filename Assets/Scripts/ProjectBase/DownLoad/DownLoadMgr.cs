using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


//todo 应该检测本地 资源生成一个文件，然后根据本地资源和服务器对比更新版本（可能出现本地资源缺失，但是版本文件没有更改的情况，所以做资源检查，或者当玩家无法进入游戏，进行资源检查button 逆水寒例子）
//todo 应该检测本地，同时删除过时的文件
public class DownLoadMgr : BaseManager<DownLoadMgr>
{
    public const int DownloadTimeOut = 5;//超时时间
    public const int DownLoadRoutineNum = 5;//下载器数量
    public string m_LocalVersionPath;//本地版本文件位置（可能本地没有版本文件，则下载全部资源）
    public string DownLoadUrl = DownLoadBaseUrl + "StandaloneWindows";//默认平台
    public static bool CheckVersionFin = false;
    public const string DownLoadBaseUrl = "http://localhost:8081/AssetBundles/";//版本文件基地址
    public string resourcesURL = "http://localhost:8081/";//资源服务器网址，配合版本文件中的路径使用，下载资源文件

    public string LocalFilePath = Application.persistentDataPath + "/";//本地资源路径

    private List<DownLoadDataEnety> m_NeedDownLoadDataList = new List<DownLoadDataEnety>();
    private List<DownLoadDataEnety> m_LocalDadaList = new List<DownLoadDataEnety>();

    public bool isLocalVersion { get; private set; }

    /// <summary>
    /// 输入版本文件所在服务器地址（地址复制到网页可直接下载）
    /// </summary>
    /// <param name="versionPathURL">输入版本文件所在服务器地址（地址复制到网页可直接下载）</param>
    public void InitCheckVersion(string versionPathURL,UnityAction finishCheck)
    {
//条件编译，根据不同平台下载不同的版本文件
#if UNITY_EDITOR || UNITY_STANDLONE_WIN
     DownLoadUrl = DownLoadBaseUrl + "StandaloneWindows";
#elif UNITY_ANDROID
    DownLoadUrl = DownLoadBaseUrl + "Android";
#elif UNITY_IPHONE
    DownLoadUrl = DownLoadBaseUrl + "IOS";
#endif
        Debug.Log("运行平台为：" + Application.platform);
        string strVersionPath = DownLoadUrl + "/" + "VersionFile.txt";//版本文件路径
        Debug.Log("版本文件路径" + strVersionPath);
        if (strVersionPath != versionPathURL)
        {
            Debug.Log("版本文件路径配置错误，可能无法根据版本文件下载资源，请修改");
            ABDownLoad.Instance.InitServerVersion(versionPathURL, OnInitVersionCallBack, finishCheck);
        }
        else
        {
       
            ABDownLoad.Instance.InitServerVersion(strVersionPath, OnInitVersionCallBack, finishCheck);//将委托传进去
        }
    }

   

    /// <summary>
    /// 初始化版本回调
    /// </summary>
    /// <param name="serverDownLoadData"></param>
    private void OnInitVersionCallBack(List<DownLoadDataEnety> serverDownLoadData)
    {
        m_LocalVersionPath = LocalFilePath + "VersionFile.txt";
        Debug.Log("版本文件地址: " + m_LocalVersionPath);
        //todo 检测本地文件，生成版本文件，对比删除过时
        if (File.Exists(m_LocalVersionPath))
        {
            Debug.Log("本地存在版本文件，对比服务器版本文件信息");
            //如果本地文件存在版本文件 则和服务器进行对比
            Dictionary<string, string> serveDic = PackDownLoadDataDic(serverDownLoadData);//服务器的版本文件字典

            //本地数据
            string clientContent = File.ReadAllText(m_LocalVersionPath);//读本地版本文件内容;
            Dictionary<string, string> clientDic = PackDownLoadDataDic(clientContent);//传入内容进行分割，赋值字典
            m_LocalDadaList = PackDownloadData(clientContent);

            isLocalVersion = true;

            //遍历服务器文件
            foreach (var serverData in serverDownLoadData)
            {
                //如果服务器有文件本地不存在，直接下载
                if (!clientDic.ContainsKey(serverData.FullName))
                {
                    Debug.Log("服务器有本地文件没有的文件"+ serverData.FullName);
                    m_NeedDownLoadDataList.Add(serverData);
                }

                if (clientDic.ContainsKey(serverData.FullName))
                {
                    
                    //同名文件比较MD5 ，不同则更新文件
                    if (serverData.MD5 != clientDic[serverData.FullName])//文件名一致，查找本地版本文件字典的MD5 是否与服务器版本文件的MD5一致
                    {
                        Debug.Log("同名文件MD5不同");
                        m_NeedDownLoadDataList.Add(serverData);
                        m_NeedDownLoadDataList.Add(serverData);
                    }
                    
                }
                
            }
        }
        else
        {
            Debug.Log("本地不存在版本文件，全部下载");
            
            for (int i = 0; i < serverDownLoadData.Count; i++)
            {
                m_NeedDownLoadDataList.Add(serverDownLoadData[i]);

            }
        }
        //进行下载
        ABDownLoad.Instance.DownLoadFiles(m_NeedDownLoadDataList);
    }

    private DownLoadDataEnety GetDownLoadData(string fullName, List<DownLoadDataEnety> lst)
    {
        foreach (var item in lst)
        {
            if (item.FullName == fullName)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 解析版本文件，返回一个文件列表
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public List<DownLoadDataEnety> PackDownloadData(string content)
    {
        List<DownLoadDataEnety> list = new List<DownLoadDataEnety>();
        string[] arrLines = content.Split('\n');//用回车 字符 \n 分割每一行
        foreach (string item in arrLines)
        {
            string[] arrData = item.Split(' ');//用空格分割每行数据的三个类型
            if (arrData.Length == 3)
            {
                DownLoadDataEnety entity = new DownLoadDataEnety();
                entity.FullName = arrData[0];//名称即路径
                entity.MD5 = arrData[1];
                entity.Size = int.Parse(arrData[2]);
                //Debug.Log(string.Format("解析的路径：{0}\n解析的MD5：{1}\n解析的文件大小KB：{2}", entity.FullName, entity.MD5, entity.Size)); 
                //todo 设置一个debug模式，利用条件编译只在 勾选时 编译（可在Android利用插件查看） ，不编译Editor 防止真机无法查看debug
                list.Add(entity);
            }
        }

        return list;
    }

    /// <summary>
    /// 传入解析后的版本文件list，list中是含有文件路径，MD5 ，大小的实体
    /// </summary>
    /// <param name="lst"></param>
    /// <returns></returns>
    public Dictionary<string, string> PackDownLoadDataDic(List<DownLoadDataEnety> lst)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        for (int i = 0; i < lst.Count; i++)
        {
            dic.Add(lst[i].FullName, lst[i].MD5);

            // todo dic[lst[i].FullName] = lst[i].MD5; 测试=替代add
        }
        return dic;
    }

    /// <summary>
    /// 传入版本文件的string内容，进行分割，且按照key为路径，value为MD5返回一个字典
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public Dictionary<string, string> PackDownLoadDataDic(string content)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        string[] arrLines = content.Split('\n');//用回车 字符 \n 分割每一行
        foreach (string item in arrLines)
        {
            string[] arrData = item.Split(' ');//用空格分割每行数据的三个类型
            if (arrData.Length == 3)
            {
                dic.Add(arrData[0], arrData[1]);//将文件名和 MD5 加入到字典中
                //Debug.Log("分割内容传入字典Key：" + arrData[0] + "  Value: " + arrData[1]);
            }
        }
        return dic;
    }

    /// <summary>
    /// 修改版本文件信息
    /// </summary>
    /// <param name="entity"></param>

    public void ModifyLocalData(DownLoadDataEnety entity)
    {
        if (m_LocalDadaList == null) return;

        bool isExists = false;
        for (int i = 0; i < m_LocalDadaList.Count; i++)
        {
            if (m_LocalDadaList[i].FullName.Equals(entity.FullName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                m_LocalDadaList[i].MD5 = entity.MD5;
                m_LocalDadaList[i].Size = entity.Size;
                isExists = true;//finish 发现版本文件中有一样名字的line，修改
            }
        }

        if (!isExists)
        {
            m_LocalDadaList.Add(entity);//finish 版本文件不存在一样的line ，加入
        }

        SaveLocalVersion();//版本文件创建
    }

    /// <summary>
    /// 保存本地版本文件
    /// </summary>
    private void SaveLocalVersion()
    {
        //Debug.Log("修改版本文件");
        StringBuilder sbContent = new StringBuilder();
        for (int i = 0; i < m_LocalDadaList.Count; i++)
        {
            sbContent.AppendLine(string.Format("{0} {1} {2}", m_LocalDadaList[i].FullName, m_LocalDadaList[i].MD5, m_LocalDadaList[i].Size));
        }

        IOUtil.CreatTextFile(m_LocalVersionPath, sbContent.ToString());
    }
}