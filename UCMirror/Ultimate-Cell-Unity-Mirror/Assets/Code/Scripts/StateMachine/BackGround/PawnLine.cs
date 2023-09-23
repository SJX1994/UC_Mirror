using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PawnLine : MonoBehaviour
{
    /// <summary>
    /// 兵线预制体
    /// </summary>
    public GameObject pawnLineInfo;
    private GameObject pawnLine;
    private float value = 0f;
    private float valueY = 0f;
    Material material;
    bool finishedMove = false;
    bool finishedMoveY = false;
    private GameObject sceneLoader;
    private CommunicationInteractionManager communicationManager;
    private BroadcastClass broadcastClass;
    private int counter = 0;

    void Start()
    {
        pawnLine = GameObject.Instantiate(pawnLineInfo, this.transform);
    
        material = transform.GetComponent<SpriteRenderer>().sharedMaterial;
        StartCoroutine(SmoothChange(0, 0.5f, 2f));
        StartCoroutine(SmoothChange(pawnLine.transform, 0, 0.5f, 2f));
        if (GameObject.Find("LanNetWorkManager") == null) return;
        // 暂时获取方式
        sceneLoader = GameObject.Find("LanNetWorkManager").gameObject;
        // 全局通信方法管理
        communicationManager = sceneLoader.GetComponent<CommunicationInteractionManager>();
        // 全局通信事件注册类
        broadcastClass = sceneLoader.GetComponent<BroadcastClass>();
        // 兵线移动事件监听
        // broadcastClass.OnFireLineChange += OnFireLineChange;

        // 兵线移动表现层更新
        broadcastClass.OnFireLineChange += SetCounter;
        broadcastClass.OnFireLineChange += ChangeValueX;
        broadcastClass.OnFireLineChange += ChangeValueY;
    }

    void Update()
    {   
        // TODO 删除兵线左右摇摆状态
        /*if(Time.timeScale == 0) return;
        float speed = 20f + (10 -counter)*2;
        float amount =  (10 - counter)*0.003f;
        pawnLine.transform.position = new Vector3(
            pawnLine.transform.position.x + Mathf.Sin(Time.time * speed) * amount,
            pawnLine.transform.position.y,
            pawnLine.transform.position.z) ;*/
    }
    
    void OnDisable()
    {
        broadcastClass.OnFireLineChange -= SetCounter;
        broadcastClass.OnFireLineChange -= ChangeValueX;
        broadcastClass.OnFireLineChange -= ChangeValueY;

        if (!material) { material = transform.GetComponent<SpriteRenderer>().sharedMaterial; }
        material.SetFloat("_Lerp", 0.5f);
        if (!pawnLine) { return; }
        pawnLine.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Lerp", 0.5f);

    }

    /// <summary>
    /// 计时器显示
    /// </summary>
    /// <param name="counter"></param>
    void SetCounter(FireLineInfoClass info)
    {
        var aa = pawnLine.transform.Find("counter");

        if (pawnLine.transform.Find("counter").TryGetComponent<TextMeshPro>(out TextMeshPro tmp))
        {
            tmp.text = info.FireLineCount.ToString();
            counter = info.FireLineCount;
        }
    }

    /// <summary>
    /// 兵线值映射显示
    /// </summary>
    /// <param name="posX"></param>
     void ChangeValueY(FireLineInfoClass info)
    {

        if (finishedMoveY)
        {

            float oldValue = valueY;
            // 映射
            valueY = info.FireLinePosY;
            // value = Remap( posX, -15.55f, 15.55f, 0f, 1f );
            // Set值范围为0-1

            StartCoroutine(SmoothChange(pawnLine.transform, oldValue, valueY, 0.2f));
        }

    }

    /// <summary>
    /// 兵线表现层左右移动
    /// </summary>
    /// <param name="posX"></param>
     void ChangeValueX(FireLineInfoClass info)
    {
        if (finishedMove)
        {
            pawnLine.transform.position = new Vector3(info.FireLinePosX.x, pawnLine.transform.position.y, pawnLine.transform.position.z);
        }
    }

     void ChangeValue(float posX)
    {

        if (finishedMove)
        {
            // 假数据
            pawnLine.transform.position = new Vector3(pawnLine.transform.position.x + posX, pawnLine.transform.position.y, pawnLine.transform.position.z);
            float oldValue = value;
            // 映射
            value = Remap(-pawnLine.transform.position.x, -30f, 30f, 0f, 1f);
            // Set值范围为0-1
            StartCoroutine(SmoothChange(oldValue, value, 2f));
        }

    }
     void ChangeValue(float posX, float min, float max)
    {

        if (finishedMove)
        {
            // 假数据
            float oldValue = value;
            // 映射
            value = Remap(posX, min, max, 0f, 1f);
            // Set值范围为0-1
            StartCoroutine(SmoothChange(oldValue, value, 1f));
        }

    }


    IEnumerator SmoothChange(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        finishedMove = false;
        while (elapsed < duration)
        {
            value = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            material.SetFloat("_Lerp", value);
            yield return null;
        }
        finishedMove = true;
        value = v_end;
    }
    IEnumerator SmoothChange(Transform gameObject, float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        finishedMoveY = false;
        Material mat = gameObject.GetComponent<SpriteRenderer>().sharedMaterial;
        while (elapsed < duration)
        {
            valueY = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            mat.SetFloat("_Lerp", valueY);
            yield return null;
        }
        finishedMoveY = true;
        valueY = v_end;
    }
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
}
