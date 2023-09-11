using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
public class TetriDisplayRange : MonoBehaviour
{
    public SpriteRenderer Up,Down,Left,Right;
    // Start is called before the first frame update
    void Awake()
    {
        Up = transform.Find("Up").GetComponent<SpriteRenderer>();
        Down = transform.Find("Down").GetComponent<SpriteRenderer>();
        Left = transform.Find("Left").GetComponent<SpriteRenderer>();
        Right = transform.Find("Right").GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        Up.color = color;
        Down.color = color;
        Left.color = color;
        Right.color = color;
    }
    public void SetSortingOrder(int order)
    {
        Up.sortingOrder = order;
        Down.sortingOrder = order;
        Left.sortingOrder = order;
        Right.sortingOrder = order;
    }
    
    
}
