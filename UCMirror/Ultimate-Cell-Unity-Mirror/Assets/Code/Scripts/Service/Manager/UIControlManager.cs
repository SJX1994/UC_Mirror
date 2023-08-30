using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIControlManager : MonoBehaviour
{

    private void Start()
    {
        AudioSystemManager.Instance.PlayMusic("FearsLeftHandMan", 99);
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("MainFightScene");

        ABManager.Instance.UnLoadAll();
    }
}
