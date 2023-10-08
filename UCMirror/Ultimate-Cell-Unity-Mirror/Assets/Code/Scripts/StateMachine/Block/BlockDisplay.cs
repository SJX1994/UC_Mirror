using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UC_PlayerData;

public class BlockDisplay : MonoBehaviour
{
#region 数据对象
    // private float speed = 2f;
    /// <summary>
    /// 单个砖块位置
    /// </summary>
    public Vector2 posId = new();
    /// <summary>
    /// 单个砖块启用表现
    /// </summary>
    public SpriteRenderer spriteRenderer_Bright;
    /// <summary>
    /// 单个砖块武器表现
    /// </summary>
    public SpriteRenderer spriteRenderer_Weapon;
    public SpriteRenderer spriteRenderer_Faction;
    /// <summary>
    /// 单个砖块武器类型
    /// </summary>
    [Header("武器类型")]
    public Sprite sprite_Sword, sprite_Bow,sprite_Spear,sprite_Shield;
     /// <summary>
    /// 单个砖块建筑类型
    /// </summary>
    [Header("建筑类型")]
    public Sprite sprite_Environmentalism, sprite_Socialism,sprite_Capitalism,sprite_Technological,sprite_Cthulhuism;
    public float finalHigh;
    [HideInInspector]
    public bool isOccupy = false;
    /// <summary>
    /// unity编辑器使用
    /// </summary>
    public Color blockColorLight = new Color(1,1,1,1);
    public Color blockColorDark = new Color(1,1,1,0);
    public Color blockColorRed = new Color(1,0,0,1f);
    public Color blockColorBlue = new Color(0,0,1,1f);
    public Color blockColorGreen = new Color(0,1,0,1f);
    public Color blockColorPurple = new Color(1f,0,1,1f);
    public Color blockColorYellow = new Color(1f, 1, 0, 1f);
    Color tempColor;
   

    public SpriteRenderer _renderer;
    Color _rendererTemp;
    List<SpriteRenderer> _rendererChilden = new();
    Color _rendererChildenTemp;
    private BlockBuoyHandler blockBuoyHandler;
    public BlockBuoyHandler BlockBuoyHandler{
        get
        {
            if(!blockBuoyHandler)blockBuoyHandler = GetComponent<BlockBuoyHandler>();
            return blockBuoyHandler;
        }
    }
    #endregion 数据对象
    #region 数据关系
    void Start()
    {
        SpawnCoroutine();
        if(!spriteRenderer_Bright)spriteRenderer_Bright = transform.Find("BlockBright").GetComponent<SpriteRenderer>();
        spriteRenderer_Bright.gameObject.SetActive(true);
        if(!spriteRenderer_Weapon)spriteRenderer_Weapon = transform.Find("BlockWeapon").GetComponent<SpriteRenderer>();
        spriteRenderer_Weapon.gameObject.SetActive(false);
        if(!spriteRenderer_Faction)spriteRenderer_Faction = transform.Find("BlockFaction").GetComponent<SpriteRenderer>();
        spriteRenderer_Faction.gameObject.SetActive(false);
        BrightCoroutine(0.5f,0,3.5f);
        _renderer = transform.GetComponent<SpriteRenderer>();
        Transform[] allChildren = _renderer.GetComponentsInChildren<Transform>();
        // foreach(Transform child in allChildren)
        // {
        //     if(child.GetComponent<SpriteRenderer>()!=null)
        //     {
        //         SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
        //         _rendererChilden.Add(sr);
               
        //     }
        // }
        _rendererTemp = _renderer.color;
        _rendererChildenTemp = new Color(Color.white.r,Color.white.g,Color.white.b,0.6f);
        tempColor = transform.GetComponent<SpriteRenderer>().color;
    }
    void Update()
    {
        
    }
    /// <summary>
    /// 进入心流模式表现
    /// </summary>
    public void InFlow()
    {
        _renderer.color = _rendererTemp;
        spriteRenderer_Weapon.color = Color.white;
        spriteRenderer_Weapon.sortingOrder = Dispaly.FlowOrder;
        spriteRenderer_Faction.color = Color.white;
        spriteRenderer_Faction.sortingOrder = Dispaly.FlowOrder;
        _renderer.sortingOrder = Dispaly.NotFlowOrder;
       
    }
    /// <summary>
    /// 退出心流模式表现
    /// </summary>
    public void OutFlow()
    {
        _renderer.color = _rendererTemp;
        spriteRenderer_Weapon.color = _rendererChildenTemp;
        spriteRenderer_Weapon.sortingOrder = Dispaly.NotFlowOrder;
        spriteRenderer_Faction.color = _rendererChildenTemp;
        spriteRenderer_Faction.sortingOrder = Dispaly.NotFlowOrder;
         _renderer.sortingOrder = Dispaly.NotFlowOrder;
            
    }
  
    /// <summary>
    /// 被激活表现
    /// </summary>
    public void Bright()
    {
       // StartCoroutine(BrightMoveCoroutine(finalHigh,finalHigh+0.3f,1.5f));
        //StartCoroutine(BrightCoroutine(0,1,0.1f));
        spriteRenderer_Bright.color = blockColorLight;
        isOccupy = true;
    }
    
    public void Bright(BuildingSlot slot,bool stayLogo = false)
    {
        
        if(slot.Building!=null)
        {
            
            isOccupy = true;
            switch(PVP_faction.MyFaction)
            {
                case PVP_faction.faction.Environmentalism:
                    transform.GetComponent<SpriteRenderer>().color = Color.green;
                    if(stayLogo)
                    {
                        spriteRenderer_Bright.color = Color.green;
                        spriteRenderer_Faction.sprite = sprite_Environmentalism;
                        spriteRenderer_Faction.gameObject.SetActive(true);
                    }
                break;
                case PVP_faction.faction.Capitalism:
                    
                break;
                case PVP_faction.faction.Socialism:
                    transform.GetComponent<SpriteRenderer>().color = Color.red;
                    if(stayLogo)
                    {
                        spriteRenderer_Bright.color = Color.red;
                        spriteRenderer_Faction.sprite = sprite_Environmentalism;
                        spriteRenderer_Faction.gameObject.SetActive(true);
                    }
                break;
                case PVP_faction.faction.Technological:
                break;
                case PVP_faction.faction.Cthulhuism:
                break;
                case PVP_faction.faction.None:
                break;

            }
        }else
        {
            transform.GetComponent<SpriteRenderer>().color = tempColor;
            isOccupy = false;
        }
    }
    public void Bright(EventType.UnitColor color)
    {
       // StartCoroutine(BrightMoveCoroutine(finalHigh,finalHigh+0.3f,1.5f));
        //StartCoroutine(BrightCoroutine(0,1,0.1f));
        
        switch(color)
        {
            case EventType.UnitColor.red:
                spriteRenderer_Bright.color = blockColorRed;
                spriteRenderer_Weapon.sprite = sprite_Sword;
                spriteRenderer_Weapon.gameObject.SetActive(true);
            break;
            case EventType.UnitColor.blue:
                spriteRenderer_Bright.color = blockColorBlue;
                spriteRenderer_Weapon.sprite = sprite_Spear;
                spriteRenderer_Weapon.gameObject.SetActive(true);
            break;
            case EventType.UnitColor.green:
                spriteRenderer_Bright.color = blockColorGreen;
                spriteRenderer_Weapon.sprite = sprite_Shield;
                spriteRenderer_Weapon.gameObject.SetActive(true);
            break;
            case EventType.UnitColor.purple:
                spriteRenderer_Bright.color = blockColorPurple;
                spriteRenderer_Weapon.sprite = sprite_Bow;
                spriteRenderer_Weapon.gameObject.SetActive(true);
            break;
            case EventType.UnitColor.yellow:
                spriteRenderer_Bright.color = blockColorYellow;
                spriteRenderer_Weapon.gameObject.SetActive(true);
                break;
        }
        isOccupy = true;
        // spriteRenderer_Bright.color = blockColorLight;
    }
    /// <summary>
    /// 不激活表现
    /// </summary>
    public void NotBright()
    {
       // StartCoroutine(BrightMoveCoroutine(finalHigh+0.3f,finalHigh,1.5f));
        //StartCoroutine(BrightCoroutine(1,0,0.1f));
        spriteRenderer_Bright.color = blockColorDark;
        spriteRenderer_Weapon.gameObject.SetActive(false);
        isOccupy = false;
    }
    public void NotBright(BuildingSlot slot)
    {
        spriteRenderer_Bright.color = blockColorDark;
        spriteRenderer_Faction.gameObject.SetActive(false);
        isOccupy = false;
    }
#endregion 数据关系
#region 数据方法
    bool IsLastChange()
    {
        return spriteRenderer_Bright.color.a > 0;
    }
    void SpawnCoroutine()
    {
        Vector3 finalPos = new Vector3(transform.position.x, 0.0f , transform.position.z);
       
        transform.position = finalPos;
    }
    void BrightCoroutine( float v_start, float v_end, float duration )
    {
        /*float elapsed = 0.0f;
        float value = v_start;
        while (elapsed < duration )
        {
            value = Mathf.Lerp( v_start, v_end, elapsed / duration );
            elapsed += Time.deltaTime;
            spriteRenderer_Bright.color = new Color(1,1,1,value);
            yield return null;
        }
        value = v_end;*/

        spriteRenderer_Bright.color = blockColorDark;
    }
    IEnumerator BrightMoveCoroutine( float v_start, float v_end, float duration )
    {
        float elapsed = 0.0f;
        float value = v_start;
        while (elapsed < duration )
        {
            value = Mathf.Lerp( v_start, v_end, elapsed / duration );
            elapsed += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, value, transform.position.z);
        yield return null;
        }
        value = v_end;
    }
    
#endregion 数据方法
}
