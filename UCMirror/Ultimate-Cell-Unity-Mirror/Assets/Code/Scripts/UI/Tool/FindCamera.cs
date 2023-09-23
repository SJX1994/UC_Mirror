using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCamera : MonoBehaviour
{
    Canvas canvas;
    private void Start()
    {
        //这里需要把UI设定为3DUI
        if(!GameObject.FindGameObjectWithTag("UICamera"))return;
        Camera uiCam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCam;
        canvas.sortingOrder = 90;
        canvas.planeDistance = 5;
    }
}
