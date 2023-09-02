using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalMouseClickDetection : MonoBehaviour, IPointerClickHandler,IPointerDownHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("鼠标左键点击");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("鼠标右键点击");
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
         if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("鼠标左键点击");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("鼠标右键点击");
        }
        
        
    }
}