using DG.Tweening;
using Mirror;
using System;
using System.Net;
using UnityEngine;

public class StartCheckManager : MonoBehaviour
{
    // 通讯组件
    private BroadcastClass broadcastClass;

    private NetWorkBroadClass NetBroadClass;

    private CommunicationInteractionManager CommunicationManager;

    public GameObject lanNetWork;

    void Start()
    {
        if (!lanNetWork.activeSelf)
        {
            lanNetWork.SetActive(true);
        }

        // 通信获取
        // 暂时获取方式
        CommunicationManager = GameObject.Find("LanNetWorkManager").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.StratCheckInfo += StartCheck;

    }

    void StartCheck(int info)
    {
        /*string ipAddress = GetLocalIP();
        Debug.Log("本地IP地址：" + ipAddress);

        string publicIP = GetPublicIP();
        Debug.Log("公网IP地址：" + publicIP);

        IPGeolocationData location = GetIPGeolocation(publicIP);
        Debug.Log("IP地址所属地理位置：" + location);*/

        StartCheckClass startcheck = new StartCheckClass()
        {
            PackageVersion = "v1.5.1",
            /*country = location.country,
            regionName = location.regionName,
            city = location.city*/
        };

        CommunicationManager.C2G_StartCheck(startcheck);
    }

    string GetLocalIP()
    {
        string localIP = "";
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
    string GetPublicIP()
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString("https://api.ipify.org");
                return response;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("获取公网IP地址出错：" + e.Message);
        }

        return null;
    }

    IPGeolocationData GetIPGeolocation(string ipAddress)
    {
        string requestUrl = "http://ip-api.com/json/" + ipAddress;

        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(requestUrl);
                IPGeolocationData data = JsonUtility.FromJson<IPGeolocationData>(response);

                if (data != null)
                {
                    return data;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("获取IP地址地理位置出错：" + e.Message);
        }

        return null;
    }

    // IP地理位置数据的结构体
    [Serializable]
    public class IPGeolocationData
    {
        public string country;
        public string regionName;
        public string city;
    }
}