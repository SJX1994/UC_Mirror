using Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GlobalControlManager : MonoBehaviour
{
    // 通讯对象
    private GameObject sceneLoader;
    private CommunicationInteractionManager CommunicationManager;
    private BroadcastClass broadcastClass;
    private MainSceneControlManager sceneManager;

    // 系统管理
    private StoryPlayerManager StoryPlayer;
    private HeroManager heroManager;
    private BattlefieldControlManager battlefieldManager;
    private List<GameObject> HideGameObject = new();

    private void Start()
    {
        // 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // 全局通信方法管理
        CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();

        // 场景管理类
        sceneManager = sceneLoader.GetComponent<MainSceneControlManager>();

        broadcastClass.OnStoryLoadingComplete += StoryBegin;

        broadcastClass.OnStoryPlay += StoryFire;

        broadcastClass.OnStoryEnd += StoryEnd;

        broadcastClass.StoryHide += StoryHide;

        broadcastClass.StoryHide += StoryHide1;

        StoryPlayer = this.gameObject.AddComponent<StoryPlayerManager>();

        AudioSystemManager.Instance.PlayMusic("FearsRightHandMan", 99);

        ExcelLoadManager.Instance.Load();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;

                Debug.Log("游戏暂停");
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;

                Debug.Log("游戏继续");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (Time.timeScale != 1 && Time.timeScale != 0)
            {
                Time.timeScale -= 1;

                Debug.Log("游戏现在的速度为：" + Time.timeScale);
            }
            else
            {
                Debug.Log("已到达最低速度不能继续减速");

            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale += 1;

                Debug.Log("游戏现在的速度为：" + Time.timeScale);
            }
            else
            {
                Debug.Log("暂停中无法加速");

            }
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            ReplayGame();
        }
    }

    /// <summary>
    /// 全局游戏暂停
    /// </summary>
    public void OnPause()
    {
        Time.timeScale = 0;

        Debug.Log("游戏暂停");
    }

    /// <summary>
    /// 全局游戏继续
    /// </summary>
    public void OnContinues()
    {
        Time.timeScale = 1;

        Debug.Log("游戏继续");
    }

    /// <summary>
    ///  全局游戏加速
    /// </summary>
    public void OnAccelerate()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale += 1;

            Debug.Log("游戏现在的速度为：" + Time.timeScale);
        }
        else
        {
            Debug.Log("暂停中无法加速");

        }
    }

    /// <summary>
    /// 全局游戏减速
    /// </summary>
    public void OnModerate()
    {
        if (Time.timeScale != 1 && Time.timeScale != 0)
        {
            Time.timeScale -= 1;

            Debug.Log("游戏现在的速度为：" + Time.timeScale);
        }
        else
        {
            Debug.Log("已到达最低速度不能继续减速");

        }
    }

    /// <summary>
    /// 重新加载游戏
    /// </summary>
    public void ReplayGame()
    {
        // sceneManager.LoadMainFightScene();
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    private void StoryBegin(int info) 
    {
        // StoryPlayer.PLayStory(0);
    }

    private void StoryFire(int info)
    {
        foreach (GameObject infoObj in HideGameObject)
        {
            infoObj.SetActive(false);
        }
    }

    private void StoryEnd(int info)
    {
        foreach (GameObject infoObj in HideGameObject)
        {
            infoObj.SetActive(true);
        }

    }

    private void StoryHide(GameObject info)
    {
        HideGameObject.Add(info);
    }
    private void StoryHide1(GameObject info)
    {
        HideGameObject.Add(info);
    }
}
