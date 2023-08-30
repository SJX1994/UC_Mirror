using System;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSelect : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    #region 单例
    public static HeroSelect instance;
    #endregion

    #region 参数
    [Serializable]
    public struct ItemInfo
    {
        public string name;
        public GameObject hero;
        public int heroIndex;
        public ItemInfo(string name,GameObject hero,int heroIndex)
        {
            this.heroIndex = heroIndex;
            this.name = name;
            this.hero = hero;
        }
    }

    [Tooltip("卡片预制体")]
    [SerializeField] private GameObject itemPrefab;
    [Tooltip("卡片父物体")]
    [SerializeField] private RectTransform itemParent;
    [Tooltip("卡片信息")]
    [SerializeField] public ItemInfo[] itemInfos;
    [Tooltip("显示数量")]
    [SerializeField] private int displayNumber;
    [Tooltip("卡片间隔")]
    [SerializeField] private float itemSpace;
    [Tooltip("移动插值")]
    [SerializeField] private float moveSmooth;
    [Tooltip("拖动速度")]
    [SerializeField] private float dragSpeed;
    [Tooltip("缩放倍率")]
    [SerializeField] private float scaleMultipying;
    [Tooltip("颜色变化")]
    [SerializeField] private float colorMultipying;
    [Tooltip("位移倍率")]
    [SerializeField] private float colorDistance;


    public event Action<int> SelectAction;

    private HeroSelectItem[] items;
    private float displayWidth;
    private int offsetTimes;
    private bool isDrag;
    private int currentItemIndex;
    private float[] distances;
    private float selectItemX;
    private bool isSelectMove;
    //private bool isSelected;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    public void OnStart()
    {
        Init();
        MoveItems(0);
    }

    private void Update()
    {
        if (!isDrag)
        {
            Adsorption();
        }
        if(itemInfos.Length > 0)
        {
            int currentOffsetTimes = Mathf.FloorToInt(itemParent.localPosition.x / itemSpace);
            if (currentOffsetTimes != offsetTimes)
            {
                offsetTimes = currentOffsetTimes;
                MoveItems(offsetTimes);
            }
            ItemsControl();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //卡片间隔计算
        displayWidth = (displayNumber - 1) * itemSpace;
        items = new HeroSelectItem[displayNumber];
        for(int i = 0; i < displayNumber; i++)
        {
            HeroSelectItem item = Instantiate(itemPrefab, itemParent).GetComponent<HeroSelectItem>();
            item.itemIndex = i;
            items[i] = item;
        }
    }

    /// <summary>
    /// 移动列表
    /// </summary>
    /// <param name="offsetTimes">偏移次数</param>
    private void MoveItems(int offsetTimes)
    {
        //将所有卡片移到正确的位置
        for(int i = 0; i < displayNumber; i++)
        {
            float x = itemSpace * (i - offsetTimes) - displayWidth / 2;
            items[i].rectTransform.localPosition = new Vector2(x, items[i].rectTransform.localPosition.y);
        }
        //循环显示
        int middle;
        if(offsetTimes > 0)
        {
            middle = itemInfos.Length - offsetTimes % itemInfos.Length;
        }
        else
        {
            middle = -offsetTimes % itemInfos.Length;
        }
        //从中间正向循环赋值
        int infoIndex = middle;
        for (int i = Mathf.FloorToInt(displayNumber / 2f); i < displayNumber; i++)
        {
            if(infoIndex >= itemInfos.Length)
            {
                infoIndex = 0;
            }
            items[i].SetInfo(itemInfos[infoIndex].hero, itemInfos[infoIndex].name, itemInfos[infoIndex].heroIndex,infoIndex, this);
            infoIndex++;
        }
        //从中间的上一个反向循环赋值
        infoIndex = middle - 1;
        for(int i = Mathf.FloorToInt(displayNumber / 2f) - 1; i >= 0 ; i--)
        {
            if(infoIndex <= -1)
            {
                infoIndex = itemInfos.Length - 1;
            }
            items[i].SetInfo(itemInfos[infoIndex].hero, itemInfos[infoIndex].name, itemInfos[infoIndex].heroIndex, infoIndex, this);
            infoIndex--;
        }
        
    }

    /// <summary>
    /// 点击选择
    /// </summary>
    /// <param name="itemIndex">点击的卡片索引</param>
    /// <param name="infoIndex">点击的选择信息索引</param>
    public void Select(int itemIndex, int infoIndex,int heroIndex, RectTransform itemRectTransform)
    {
        //判断当前选项是否是居中的卡片
        if(itemIndex == currentItemIndex)
        {
            SelectAction?.Invoke(infoIndex);
            //isSelected = true;
        }
        else
        {
            //不是移动选项
            isSelectMove = true;
            selectItemX = itemRectTransform.localPosition.x;
        }
    }

    /// <summary>
    /// 吸附中心点
    /// </summary>
    public void Adsorption()
    {
        //标记一个X轴向的预设目标点
        float targetX;

        if (!isSelectMove)
        {
            //没有拖拽时候就找到最近的卡片
            float distance = itemParent.localPosition.x % itemSpace;
            int times = Mathf.FloorToInt(itemParent.localPosition.x / itemSpace);
            if(distance > 0)
            {
                if(distance < itemSpace / 2)
                {
                    targetX = times * itemSpace;
                }
                else
                {
                    targetX = (times + 1) * itemSpace;
                }
            }
            else
            {
                if (distance < -itemSpace / 2)
                {
                    targetX = times * itemSpace;
                }
                else
                {
                    targetX = (times + 1) * itemSpace;
                }
            }
        }
        else
        {
            //有就是选择的卡片
            targetX = -selectItemX;
        }
        itemParent.localPosition = new Vector2(Mathf.Lerp(itemParent.localPosition.x, targetX, moveSmooth / 10), itemParent.localPosition.y);
    }

    /// <summary>
    /// 卡片元素大小和颜色控制
    /// </summary>
    private void ItemsControl()
    {
        distances = new float[displayNumber];
        for(int i= 0; i < displayNumber; i++)
        {
            float distance = Mathf.Abs(items[i].rectTransform.position.x - transform.position.x);
            distances[i] = distance;
            float scale = 1 - distance * scaleMultipying;
            float color = 0.5f - distance * colorMultipying;
            items[i].heroItem.GetComponent<RectTransform>().localScale = new Vector3((float)scale, (float)scale, 1);
            Color currentColor = items[i].heroItem.GetComponent<SkeletonGraphic>().color;
            if (!isDrag)
            {
                if (distance < colorDistance)
                {
                    items[i].heroItem.GetComponent<SkeletonGraphic>().color = Color.Lerp(currentColor,Color.white,colorMultipying * Time.deltaTime);
                }
                else
                {
                    items[i].heroItem.GetComponent<SkeletonGraphic>().color = Color.Lerp(currentColor, new Color(0.2f, 0.2f, 0.2f, 1f), colorMultipying * Time.deltaTime);
                }
            }
            else if(isDrag)
            {
                items[i].heroItem.GetComponent<SkeletonGraphic>().color = Color.Lerp(currentColor, Color.white, colorMultipying * Time.deltaTime);
            }
        }

        float minDistance = itemSpace * displayNumber;
        int minIndex = 0;
        for(int i= 0; i < displayNumber; i++)
        {
            if(distances[i] < minDistance)
            {
                minDistance = distances[i];
                minIndex = i;
            }
        }

        for(int i= 0; i < items.Length; i++)
        {
            if(i == minIndex)
            {
                items[i].textMeshPro.gameObject.SetActive(true);
            }
            else
            {
                items[i].textMeshPro.gameObject.SetActive(false);
            }
        }

        currentItemIndex = items[minIndex].itemIndex;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isSelectMove = false;
        itemParent.localPosition = new Vector2(itemParent.localPosition.x + eventData.delta.x * dragSpeed, itemParent.localPosition.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
    }


    public void ToLeft()
    {
        AudioSystemManager.Instance.PlaySound("UI_Page_Change_Left");
        isSelectMove = false;
        itemParent.localPosition = new Vector2(Mathf.Lerp(itemParent.localPosition.x, itemParent.localPosition.x + 30000, Time.deltaTime * moveSmooth), itemParent.localPosition.y);
    }

    public void ToRight()
    {
        AudioSystemManager.Instance.PlaySound("UI_Page_Change_Right");
        isSelectMove = false;
        itemParent.localPosition = new Vector2(Mathf.Lerp(itemParent.localPosition.x, itemParent.localPosition.x - 30000, Time.deltaTime * moveSmooth), itemParent.localPosition.y);
    }

    public void SetItemsInfo(string[] names, GameObject[] heros, int[] heroIndexs)
    {
        if(names.Length!= heros.Length)
        {
            Debug.Log("数据不完整");
            return;
        }
        itemInfos = new ItemInfo[names.Length];
        for(int i = 0; i < itemInfos.Length; i++)
        {
            itemInfos[i] = new ItemInfo(names[i], heros[i], heroIndexs[i]);
        }
        SelectAction = null;
        //isSelected = false;
    }


}
