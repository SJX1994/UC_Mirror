using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ResManager;

public class HeroSelectItem : MonoBehaviour,IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject heroItem;
    public Transform hero;
    public Text textMeshPro;

    public int itemIndex;
    public int infoIndex;

    private int heroIndex;
    private HeroSelect heroSelect;
    public RectTransform rectTransform;
    private bool isDrag;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetInfo(GameObject heroUi,string name,int infoIndex,int heroIndex,HeroSelect heroSelect)
    {
        if (hero.childCount > 0)
        {
            for (int i = 0; i < hero.childCount; i++)
            {
                Destroy(hero.GetChild(i).gameObject);
            }
        }
        heroItem = Instantiate(heroUi, hero);
        textMeshPro.text = name;
        this.infoIndex = infoIndex;
        this.heroSelect = heroSelect;
        this.heroIndex = heroIndex;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = false;
        heroSelect.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDrag)
        {
            heroSelect.Select(itemIndex, infoIndex,heroIndex, rectTransform);
        }
        heroSelect.OnPointerUp(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
        heroSelect.OnDrag(eventData);
    }



    
}
