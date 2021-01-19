using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// 主下载器
/// </summary>
public class ABDownLoad : SingletnoAutoMono<ABDownLoad>
{
    private string m_VersionUrl;//由外部通过InitServerVersion 赋值
    private Action<List<DownLoadDataEnety>> m_OnInitVersion;// 由外部通过InitServerVersion 赋值
    /// <summary>
    /// 下载完成的委托
    /// </summary>
    private UnityAction finishCheck;

    private AssetBundleDownloadRoutine[] m_Routine = new AssetBundleDownloadRoutine[DownLoadMgr.DownLoadRoutineNum];
    private int m_RoutineIndex;

    private int totalSize;

    /// <summary>
    /// 总大小
    /// </summary>
    public int TotalSize { get => totalSize; private set => totalSize = value; }

    private int totalCount;

    private bool m_IsDownLoadOver = false;

    private float m_Time = 1;//采样时间
    private float m_AlreadyTime = 0;
    private float m_NeedTime = 0;
    private float m_Speed = 0f;

    /// <summary>
    /// 当前已经下载的总大小
    /// </summary>
    /// <returns></returns>
    public int CurrCompleteTotalSize()
    {
        int conpleteTotalSize = 0;
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] != null)
            {
                conpleteTotalSize += m_Routine[i].DownloadSize;
            }
        }
        return conpleteTotalSize;
    }

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalCount { get => totalCount; private set => totalCount = value; }

    /// <summary>
    /// 当前已经下载的总数量
    /// </summary>
    /// <returns></returns>
    public int CurrCompleteTotalCount()
    {
        int conpleteTotalSizeCount = 0;
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] != null)
            {
                conpleteTotalSizeCount += m_Routine[i].CompleteCount;
            }
        }
        return conpleteTotalSizeCount;
    }

    private void Start()
    {
        Debug.Log("读取版本文件ABDownLoad");
        StartCoroutine(DownLoadVersion(m_VersionUrl));
    }

    /// <summary>
    /// 下载版本文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator DownLoadVersion(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest;//finish 等待资源下载
        webRequest.SendWebRequest();
        if (webRequest.isNetworkError || webRequest.isHttpError)                                                             //如果出错
        {
            Debug.Log(webRequest.error); //输出 错误信息
        }
        else
        {
            while (!webRequest.isDone) //只要下载没有完成，一直执行此循环
            {
                Debug.Log("请求版本信息" + webRequest.downloadProgress);
                yield return 0;
            }

            if (webRequest.isDone) //如果下载完成了
            {
                print("版本文件下载完成：");
            }
        }

        if (webRequest != null && webRequest.error == null)
        {
            string content = webRequest.downloadHandler.text;
            Debug.Log("读取版本文件内容：" + content);

            if (m_OnInitVersion != null)
            {
                m_OnInitVersion(DownLoadMgr.Getinstate().PackDownloadData(content));//调用委托 将下载列表内容解析 然后传给委托执行，执行下载
            }
        }
        else
        {
            Debug.LogError("下载失败" + webRequest.error);
        }
    }

    private void Update()
    {
        
        if (TotalCount==0&& m_IsDownLoadOver == false&&DownLoadMgr.Getinstate().isLocalVersion)//存在版本文件 且下载数量=0
        {
            Debug.Log("没有更新资源需要下载");
            m_IsDownLoadOver = true;
            finishCheck();
        }

        if (TotalCount > 0 && m_IsDownLoadOver == false)
        {

            int totalCompleteCount = CurrCompleteTotalCount();
            totalCompleteCount = totalCompleteCount == 0 ? 1 : totalCompleteCount;//total的值判断是否是0，是0则归1，否则就是本身的值  判断是true或者false ？true：false

            int totalCompleteSize = CurrCompleteTotalSize();
           
            string str = string.Format("正在下载：{0}/{1}", totalCompleteCount, TotalCount);//finish 下载数量
            string strProgress = string.Format("下载进度:{0}", totalCompleteSize / (float)TotalSize);//finish 下载进度

            //Debug.Log(totalCompleteSize);
            //Debug.Log((float)TotalSize);
            //todo 剩余时间 采样时间内下载的大小/采样时间=下载速度，剩余时间=剩余大小/下载速度  totalMD/(MB/S)
            
            Debug.Log(str);
            Debug.Log(strProgress);
            if (totalCompleteCount == TotalCount)
            {
                m_IsDownLoadOver = true;
                Debug.Log("下载完成");
                //finish 判断下载完成 finishCheck ，进度条完成即下载完成
                finishCheck();
            }
        }
    }

    /// <summary>
    /// 初始化资源服务器信息，将传进来的网址和委托添加
    /// </summary>
    /// <param name="url"></param>
    /// <param name="onInitVersion"></param>
    public void InitServerVersion(string url, Action<List<DownLoadDataEnety>> onInitVersion, UnityAction finishCheck)
    {
        m_VersionUrl = url;
        m_OnInitVersion = onInitVersion;
        this.finishCheck = finishCheck;
    }

    /// <summary>
    /// 进行下载
    /// </summary>
    /// <param name="downLoadDataEneties"></param>
    public void DownLoadFiles(List<DownLoadDataEnety> downloadList)
    {
        Debug.Log("开始下载");
        TotalSize = 0;
        totalCount = 0;
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            {
                Debug.Log("创建下载器");
                m_Routine[i] = gameObject.AddComponent<AssetBundleDownloadRoutine>();
            }
        }
        //循环给下载器分配任务，循环结束则任务分配完毕
        for (int i = 0; i < downloadList.Count; i++)
        {
            Debug.Log("为下载器分配下载任务");
            m_RoutineIndex = m_RoutineIndex % m_Routine.Length;//0-4 取余给每个下载器分别赋值下载任务
            //分配文件
            m_Routine[m_RoutineIndex].AddDownload(downloadList[i]);
            m_RoutineIndex++;

            TotalSize += downloadList[i].Size;
            
        }
        TotalCount = downloadList.Count;
        Debug.LogError(string.Format("需要下载的总数量：{0} \n 需要下载的总大小：{1}", TotalCount, TotalSize));
        //开始下载
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            {
                continue;
            }

            m_Routine[i].StarDownload();
        }
        
        
    }
}