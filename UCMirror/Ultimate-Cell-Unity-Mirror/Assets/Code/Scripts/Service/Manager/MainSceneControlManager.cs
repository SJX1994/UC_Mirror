using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneControlManager : MonoBehaviour
{
    [Header("启动模式")]
    public ServerSwitch ServerSwitchCheck;

    [Header("固定加载场景")]
    [SerializeField] public SceneReference[] sceneToLoads;

    // Header("选择加载场景")]
    // [SerializeField] public SceneReference[] selectivelyLoadScene;

    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager comm;

    private NetWorkBroadClass NetBroadClass;

    private GameObject sceneLoader;

    public enum ServerSwitch 
    {
        OnLocalStrat,
        OnClientStart,
        OnServerStart
    }

    /// <summary>
    /// 所有场景
    /// </summary>
    private Dictionary<string, SceneReference> sceneReferenceDic = new Dictionary<string, SceneReference>();

    private void Awake()
    {
    }

    private void Start()
    {
        // 获取通讯管理器
        comm = this.GetComponent<CommunicationInteractionManager>();

        // 加载固定加载场景
        LoadSceneAdditive(sceneToLoads);

        // 判断启动模式为客户端连接模式
        if (ServerSwitchCheck == ServerSwitch.OnClientStart)
        {
            comm = GameObject.Find("LanNetWorkManager").GetComponent<CommunicationInteractionManager>();

            comm.ServerState = 0;

            Debug.Log("当前启动模式为客户端启动模式");
        }

        // // 判断启动模式为服务器启动模式
        // if (ServerSwitchCheck == ServerSwitch.OnServerStart)
        // {
        //     LoadSceneAdditive(selectivelyLoadScene);

        //     comm.ServerState = 1;

        //     Debug.Log("当前启动模式为服务器启动模式");
        // }

        // // 判断启动模式为本地启动模式
        // if (ServerSwitchCheck == ServerSwitch.OnLocalStrat)
        // {
        //     LoadSceneAdditive(selectivelyLoadScene);

        //     comm.ServerState = 2;

        //    // Debug.Log("当前启动模式为本地静态启动模式");
        // }

        // 获取通讯管理器
        sceneLoader = GameObject.Find("LanNetWorkManager");

        if (sceneLoader != null)
        {
            broadcastClass = sceneLoader.gameObject.GetComponent<BroadcastClass>();

            broadcastClass.StartGamePVP += LoadMainFightScene;
        }
    }

    /// <summary>
    /// 加载PVE场景
    /// </summary>
    public void LoadMainFightScene()
    {
        broadcastClass.Reload();

        ABManager.Instance.UnLoadAll();

        SceneManager.LoadScene("MainFightScene");

        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// 加载主界面
    /// </summary>
    public void LoadMainBasicScene()
    {
        broadcastClass.Reload();

        ABManager.Instance.UnLoadAll();

        SceneManager.LoadScene("MainBasicScene");

        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// 加载玩家对战场景
    /// </summary>
    public void LoadMainFightScene(int info)
    {
        broadcastClass.Reload();

        ABManager.Instance.UnLoadAll();

        SceneManager.LoadScene("MainFightScene");

        Time.timeScale = 1.0f;
    }


    /// <summary>
    /// 根据名称全局加载场景接口
    /// </summary>
    /// <param name="SceneNames"></param>
    /// <param name="Loadtype"></param>
    public void LoadSceneAsync(string[] SceneNames, LoadSceneMode Loadtype)
    {
        Dictionary<string, SceneReference> keyValuePairs = new();

        foreach (string scneneName in SceneNames)
        {
            if (sceneReferenceDic.TryGetValue(scneneName, out SceneReference targetScene))
            {
                keyValuePairs.Add(scneneName, targetScene);
            }
            else
            {
                Debug.Log("场景：" + scneneName + "不存在，请检查名称拼写或预存位置");
            }
        }

        switch (Loadtype)
        {
            case LoadSceneMode.Additive:

                //this.LoadSceneAdditive(keyValuePairs);

                break;

            case LoadSceneMode.Single:

                this.LoadSceneSingle(keyValuePairs);

                break;
        }
    }

    /// <summary>
    ///  加载指定的几个场景，在Mian场景下
    /// </summary>
    /// <param name="sceneToLoads"></param>
    public void LoadSceneAdditive(SceneReference[] sceneToLoads)
    {
        foreach (var sceneName in sceneToLoads)
        {
            try
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            catch (Exception e)
            {
                Debug.Log("场景：" + sceneName + "不存在， 请检查 File -> BuildSettings -> Scenes in Build 中是否存在该场景" + e);
            }
        }
    }

    /// <summary>
    /// 关闭其他所有场景，并加载指定场景
    /// </summary>
    /// <param name="sceneToLoad"></param>
    public void LoadSceneSingle(Dictionary<string, SceneReference> sceneToLoads)
    {
        foreach (var sceneName in sceneToLoads.Keys)
        {
            var sceneToLoad = sceneToLoads[sceneName];
            try
            {
                SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            }
            catch (Exception e)
            {
                Debug.Log("场景：" + sceneName + "不存在， 请检查 File -> BuildSettings -> Scenes in Build 中是否存在该场景" + e);
            }
        }
    }
}
