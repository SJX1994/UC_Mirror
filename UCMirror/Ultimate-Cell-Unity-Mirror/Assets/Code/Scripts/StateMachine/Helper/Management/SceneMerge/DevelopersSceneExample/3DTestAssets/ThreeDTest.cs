using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDTest : MonoBehaviour
{
    SceneLoader sceneLoader;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = GameObject.Find("LanNetWorkManager").GetComponent<SceneLoader>();
        sceneLoader.OnDataPass+= SetData;
        sceneLoader.OnDestoryPass += SetDestory;
    }

    void SetData(int data)
    {
        transform.localScale = new Vector3(transform.localScale.x + data, transform.localScale.y, transform.localScale.z);
    }
    void SetDestory(SceneLoader.Test3DState state)
    {
        switch(state)
        {
            case SceneLoader.Test3DState.Active:
            break;
            case SceneLoader.Test3DState.Destory:
                sceneLoader.OnDestoryPass -= SetDestory;
                Destroy(this.transform.gameObject,0.5f);
            break;
        }
    }
}
