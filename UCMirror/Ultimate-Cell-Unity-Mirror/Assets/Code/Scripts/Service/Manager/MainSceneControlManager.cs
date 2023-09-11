using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneControlManager : MonoBehaviour
{
    [Header("固定加载场景")]
    [SerializeField] public SceneReference[] sceneToLoads;

    private void Start()
    {
        // 加载固定加载场景
        LoadSceneAdditive(sceneToLoads);
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
}
