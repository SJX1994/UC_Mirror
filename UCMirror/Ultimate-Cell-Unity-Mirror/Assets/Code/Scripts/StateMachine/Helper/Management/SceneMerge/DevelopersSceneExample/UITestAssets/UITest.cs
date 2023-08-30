using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    private int m_data;
    public TMP_Text textMesh;
    public Button AddOneButton;
    public Button Destory3DButton;
    private SceneLoader sceneLoader;
    public void SetData()
    {
        SceneLoader sceneLoader = GameObject.Find("LanNetWorkManager").GetComponent<SceneLoader>();
        m_data = sceneLoader.data;
        sceneLoader.data += 1;
        sceneLoader.fireDataEvent();
        textMesh.SetText($"AddOne:{m_data.ToString()}");
    }

    public void Destory3D()
    {
        AddOneButton.interactable = false;
        Destory3DButton.interactable = false;
        SceneLoader sceneLoader = GameObject.Find("LanNetWorkManager").GetComponent<SceneLoader>();
        sceneLoader.state3D = SceneLoader.Test3DState.Destory;
        sceneLoader.fireDestory3DEvent();
    }
}
