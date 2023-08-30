using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneToMainScene : MonoBehaviour
{
    //private CommunicationInteractionManager CommunicationManager;
    public List<GameObject>  cloneObjects = new();
    // Start is called before the first frame update
    void Start()
    {
        // TODO 暂时获取方式
        // if(GameObject.Find("LanNetWorkManager") == null)
        // {
        //     return;
        // }
        // var sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;

        // // 全局通信方法管理
        // CommunicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();

        foreach (var obj in cloneObjects)
        {
            GameObject o = Instantiate(obj);
            o.SetActive(true);

            // CommunicationManager.AddHideGameObjectList(o);
        }
    }

    // Update is called once per frame
   
}
