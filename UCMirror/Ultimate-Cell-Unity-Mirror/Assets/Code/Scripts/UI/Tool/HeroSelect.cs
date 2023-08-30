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
    #region ����
    public static HeroSelect instance;
    #endregion

    #region ����
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

    [Tooltip("��ƬԤ����")]
    [SerializeField] private GameObject itemPrefab;
    [Tooltip("��Ƭ������")]
    [SerializeField] private RectTransform itemParent;
    [Tooltip("��Ƭ��Ϣ")]
    [SerializeField] public ItemInfo[] itemInfos;
    [Tooltip("��ʾ����")]
    [SerializeField] private int displayNumber;
    [Tooltip("��Ƭ���")]
    [SerializeField] private float itemSpace;
    [Tooltip("�ƶ���ֵ")]
    [SerializeField] private float moveSmooth;
    [Tooltip("�϶��ٶ�")]
    [SerializeField] private float dragSpeed;
    [Tooltip("���ű���")]
    [SerializeField] private float scaleMultipying;
    [Tooltip("��ɫ�仯")]
    [SerializeField] private float colorMultipying;
    [Tooltip("λ�Ʊ���")]
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
    /// ��ʼ��
    /// </summary>
    private void Init()
    {
        //��Ƭ�������
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
    /// �ƶ��б�
    /// </summary>
    /// <param name="offsetTimes">ƫ�ƴ���</param>
    private void MoveItems(int offsetTimes)
    {
        //�����п�Ƭ�Ƶ���ȷ��λ��
        for(int i = 0; i < displayNumber; i++)
        {
            float x = itemSpace * (i - offsetTimes) - displayWidth / 2;
            items[i].rectTransform.localPosition = new Vector2(x, items[i].rectTransform.localPosition.y);
        }
        //ѭ����ʾ
        int middle;
        if(offsetTimes > 0)
        {
            middle = itemInfos.Length - offsetTimes % itemInfos.Length;
        }
        else
        {
            middle = -offsetTimes % itemInfos.Length;
        }
        //���м�����ѭ����ֵ
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
        //���м����һ������ѭ����ֵ
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
    /// ���ѡ��
    /// </summary>
    /// <param name="itemIndex">����Ŀ�Ƭ����</param>
    /// <param name="infoIndex">�����ѡ����Ϣ����</param>
    public void Select(int itemIndex, int infoIndex,int heroIndex, RectTransform itemRectTransform)
    {
        //�жϵ�ǰѡ���Ƿ��Ǿ��еĿ�Ƭ
        if(itemIndex == currentItemIndex)
        {
            SelectAction?.Invoke(infoIndex);
            //isSelected = true;
        }
        else
        {
            //�����ƶ�ѡ��
            isSelectMove = true;
            selectItemX = itemRectTransform.localPosition.x;
        }
    }

    /// <summary>
    /// �������ĵ�
    /// </summary>
    public void Adsorption()
    {
        //���һ��X�����Ԥ��Ŀ���
        float targetX;

        if (!isSelectMove)
        {
            //û����קʱ����ҵ�����Ŀ�Ƭ
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
            //�о���ѡ��Ŀ�Ƭ
            targetX = -selectItemX;
        }
        itemParent.localPosition = new Vector2(Mathf.Lerp(itemParent.localPosition.x, targetX, moveSmooth / 10), itemParent.localPosition.y);
    }

    /// <summary>
    /// ��ƬԪ�ش�С����ɫ����
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
            Debug.Log("���ݲ�����");
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
