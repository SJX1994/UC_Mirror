using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryUnitSet : MonoBehaviour
{
    private Renderer spineRenderer;
    private MaterialPropertyBlock spinePropertyBlock;
    // Start is called before the first frame update
    void Start()
    {
        spineRenderer = transform.GetComponent<Renderer>();
        if (spinePropertyBlock == null)
        {
            spinePropertyBlock = new MaterialPropertyBlock();
        }
        spineRenderer .GetPropertyBlock(spinePropertyBlock);
        spinePropertyBlock.SetFloat("_Porcess", 1f);
        spineRenderer.SetPropertyBlock(spinePropertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
