using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using Mirror;
public class CloneToMainScene : MonoBehaviour
{
    public List<GameObject>  cloneObjects = new();
    void Start()
    {
        if(!CheckLocal())return;
        foreach (var obj in cloneObjects)
        {
            GameObject o = Instantiate(obj);
            o.SetActive(true);
        }
        foreach (var obj in cloneObjects)
        {
            obj.SetActive(false);
            DestroyImmediate(obj);
        }
    }
    bool CheckLocal()
    {
        if(RunModeData.CurrentRunMode == RunMode.Host)
        {
            
            for(int i = 0;i<cloneObjects.Count;i++)
            {
                if(!cloneObjects[i].transform.TryGetComponent(out NetworkIdentity networkIdentity))continue;
                networkIdentity.enabled = false;
                cloneObjects[i].SetActive(false);
                DestroyImmediate(cloneObjects[i]);
            }
            return false;
        }else if(RunModeData.CurrentRunMode == RunMode.Local)
        {
            foreach (var obj in cloneObjects)
            {
                if(obj.transform.TryGetComponent(out NetworkIdentity networkIdentity))DestroyImmediate(networkIdentity);
                if(obj.transform.TryGetComponent(out NetworkTransformBase networkTransformBase))networkTransformBase.enabled = false;
            }
            return true;
        }
        return false;
    }
   
}
