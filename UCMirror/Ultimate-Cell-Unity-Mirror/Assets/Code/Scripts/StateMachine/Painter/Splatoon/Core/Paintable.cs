using UnityEngine;
using Mirror;
using UC_PlayerData;
public class Paintable : NetworkBehaviour {
    const int TEXTURE_SIZE = 1024;

    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;
    
    Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        if(Local())
        {
            maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
            maskRenderTexture.filterMode = FilterMode.Bilinear;

            extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
            extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

            uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
            uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

            supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
            supportTexture.filterMode =  FilterMode.Bilinear;

            rend = GetComponent<Renderer>();
            rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

            PaintManager.instance.initTextures(this);
        }else
        {
            if(!isServer)
            {
                enabled = false;
                return;
            }
        }
    }

    void OnDisable(){
        if(!maskRenderTexture || !uvIslandsRenderTexture || !extendIslandsRenderTexture || !supportTexture)return;
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }
    bool Local()
    {
        if(RunModeData.CurrentRunMode == RunMode.Local)return true;
        return false;
    }
}