using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    // 闪光精灵
    public SpriteRenderer myLight;        // Your light
    Color color,A,B;

    void Start()
    {
        if(!myLight)
        {
            myLight = GetComponent<SpriteRenderer>();
        }
        color = myLight.sharedMaterial.GetColor("_BaseColor");
        A = new Color(color.r,color.g,color.b,0.4f);
        B = new Color(color.r,color.g,color.b,0.2f);     
    }
    void FixedUpdate()
    {
        
        color = Color.Lerp(A, B, Mathf.PingPong(0.3f*(Time.time+Random.Range(-0.1f,0.1f)), 1));
        myLight.sharedMaterial.SetColor("_BaseColor",color);

    }
    void OnDisable()
    {
       
        if(!myLight){myLight = GetComponent<SpriteRenderer>();}
        myLight.sharedMaterial.SetColor("_BaseColor",new Color(1,1,1,0.3f));
        
    }
    
  
   
    
}
