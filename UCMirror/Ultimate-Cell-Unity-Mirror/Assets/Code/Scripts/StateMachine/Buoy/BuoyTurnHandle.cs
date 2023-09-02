using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;

public class BuoyTurnHandle : MonoBehaviour
{
    float zoomSpeed = 0.5f;
    float minValue = 0f;
    float maxValue = 3f;

    private float currentValue = 0.1f; // 初始值
    public float CurrentValue
    {
        get { return currentValue; }
        set {
                if (value != currentValue)
                {
                    currentValue = value;
                    TurnHandleTurning(currentValue);
                }

            }
    }
    float shaderTurnHandle;
    List<Material> progressBarMats = new();
   
    void Start()
    {
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();
        foreach (var renderer in renderers)
        {
            progressBarMats.Add(renderer.material);
        }
        CurrentValue = currentValue;
        TurnHandleTurning(CurrentValue);
    }
    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        CurrentValue += scrollInput * zoomSpeed;

        // 限制值在最小值和最大值之间
        CurrentValue = Mathf.Clamp(currentValue, minValue, maxValue);

    }
    void TurnHandleTurning(float value)
    {
        shaderTurnHandle = Remap(value,minValue,maxValue,0,1);
        foreach (var progressBarMat in progressBarMats)
        {
            progressBarMat.SetFloat("_Progress", shaderTurnHandle);
        }   
        
    }
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
    
}
