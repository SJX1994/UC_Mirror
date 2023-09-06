using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneToMainScene : MonoBehaviour
{
    public List<GameObject>  cloneObjects = new();
    void Start()
    {
        foreach (var obj in cloneObjects)
        {
            GameObject o = Instantiate(obj);
            o.SetActive(true);
        }
        foreach (var obj in cloneObjects)
        {
            obj.SetActive(false);
        }
    }
   
}
