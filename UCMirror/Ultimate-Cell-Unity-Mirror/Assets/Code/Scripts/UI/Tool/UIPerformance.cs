using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI操作导致的效果 静态类
/// </summary>
public class UIPerformance : MonoBehaviour
{
    public static void ButtonEnterEffect(List<SpriteRenderer> spriteRenderers)
    {
        if(spriteRenderers.Count >= 0)
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                spriteRenderers[i].material.SetFloat("_OutlineWidth", 0.015f);
                spriteRenderers[i].material.SetFloat("_Intensity", 1.25f);
                spriteRenderers[i].material.SetFloat("_Fresnel", 1f);
            }
        }
    }

    public static void ButtonExitEffect(List<SpriteRenderer> spriteRenderers)
    {
        if(spriteRenderers.Count >= 0)
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                spriteRenderers[i].material.SetFloat("_OutlineWidth", 0f);
                spriteRenderers[i].material.SetFloat("_Intensity", 1f);
                spriteRenderers[i].material.SetFloat("_Fresnel", 0f);
            }
        }
    }

    public static List<SpriteRenderer> GetSpriteRenderer(GameObject obj)
    {
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        Transform[] heroPrefabs = obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < heroPrefabs.Length; i++)
        {
            if (heroPrefabs[i].GetComponent<SpriteRenderer>() && heroPrefabs[i].name != "TouYing")
            {
                SpriteRenderer spriteRender = heroPrefabs[i].GetComponent<SpriteRenderer>();
                spriteRenderers.Add(spriteRender);
            }
        }
        return(spriteRenderers);
    }
}
