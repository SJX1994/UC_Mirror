using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UC_PlayerData;
using Mirror;
public class TetriDisplayRange : NetworkBehaviour
{
#region 数据对象
    public SpriteRenderer Up,Down,Left,Right;
#endregion 数据对象
#region 数据关系
    // Start is called before the first frame update
    void Awake()
    {
        Up = transform.Find("Up").GetComponent<SpriteRenderer>();
        Down = transform.Find("Down").GetComponent<SpriteRenderer>();
        Left = transform.Find("Left").GetComponent<SpriteRenderer>();
        Right = transform.Find("Right").GetComponent<SpriteRenderer>();
    }
#endregion 数据关系
#region 数据操作
    public void SetColor(Color color)
    {
        Up.color = color;
        Down.color = color;
        Left.color = color;
        Right.color = color;
    }
    public void SetAlpha(float alpha)
    {
        Up.color = new Color(Up.color.r,Up.color.g,Up.color.b,alpha);
        Down.color = new Color(Down.color.r,Down.color.g,Down.color.b,alpha);
        Left.color = new Color(Left.color.r,Left.color.g,Left.color.b,alpha);
        Right.color = new Color(Right.color.r,Right.color.g,Right.color.b,alpha);
    }
    public void SetSortingOrder(int order)
    {
        Up.sortingOrder = order;
        Down.sortingOrder = order;
        Left.sortingOrder = order;
        Right.sortingOrder = order;
    }
    
#endregion 数据操作
#region 联网数据操作
    [Server]
    public void Server_SetColor(Color color)
    {
        Up.color = color;
        Down.color = color;
        Left.color = color;
        Right.color = color;
        Client_SetColor(color);
    }
    [ClientRpc]
    public void Client_SetColor(Color color)
    {
        Up.color = color;
        Down.color = color;
        Left.color = color;
        Right.color = color;
    }
#endregion 联网数据操作
}
