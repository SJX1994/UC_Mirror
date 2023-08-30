using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ref: https://www.mooict.com/unity-3d-tutorial-how-to-drag-and-drop-objects-using-touch-controls-with-c/
public class TouchToCollect : MonoBehaviour
{
    private float dist;
    // private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;
    private UnitSoul soul;
   
   
    
    void Update()
    {
        
        Vector3 pos;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount != 1)
            {
                // dragging = false;
                return;
            }
            Touch touch = Input.touches[0];
            pos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                RayLogic(pos); 
                
                
            }
            // if (dragging && touch.phase == TouchPhase.Moved)
            // {
            //     v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            //     v3 = Camera.main.ScreenToWorldPoint(v3);
            //     toDrag.position = v3 + offset;
            // }
            // if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            // {
            //     dragging = false;
            // }
        }
        
        
        
        
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                pos = Input.mousePosition;
                RayLogic(pos);  
            }
        }
        
    }
    bool RayLogic(Vector3 pos)
    {
        Vector3 v3;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Soul_Cell")
            {
                
                toDrag = hit.transform;
                UnitSoul soul = toDrag.GetComponent<UnitSoul>();
                if(soul.clicked == false)
                {
                    soul.clicked = true;
                    dist = hit.transform.position.z - Camera.main.transform.position.z;
                    v3 = new Vector3(pos.x, pos.y, dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    offset = toDrag.position - v3;
                    // dragging = true;
                    Vector2 temp_from = new Vector2(toDrag.position.x,toDrag.position.z) ;
                    Vector2 temp_to = new Vector2(8.5f,9.5f);
                    StartCoroutine(soul.CollectingDispaly(temp_from,temp_to,1.5f,toDrag.gameObject));
                }
                
                return true;
            }else
            {
                 return false;
            }
           
        }
        return false;
    }
}