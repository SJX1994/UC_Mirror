using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UC_PlayerData;
using Mirror;

public class BuoyTurnHandle : NetworkBehaviour
{
    BuoyInfo buoyInfo;
    BlocksCreator_Main blocksCreator;
    float zoomSpeed = 0.5f;
    float minValue = 1f;
    float maxValue = 5f;
    private Vector2 centerOfControl;
    public Vector2 CenterOfControl
    {
        get { return centerOfControl; }
        set { 
            if (value == centerOfControl)return;
            centerOfControl = value; 
        }
    }
    public float currentValue = 1f; // 初始值
    public bool actived = false;
    List<TetrisBuoySimple> tetrisBuoysControled = new();
    List<BlockBuoyHandler> blocksScanned = new(); 
    public float CurrentValue
    {
        get { return currentValue; }
        set {
                if (value == currentValue)return;
                currentValue = value;
                Dispaly_TurnHandleTurning(currentValue);
                if(buoyInfo.Local())
                {
                    colliderDraw.size = new Vector3(currentValue,1,currentValue);
                    CountScanning();
                }else
                {
                    if(isServer)
                    {
                        colliderDraw.size = new Vector3(currentValue,1,currentValue);
                        CountScanning();
                    }
                    if(!isLocalPlayer)return;
                    colliderDraw.size = new Vector3(currentValue,1,currentValue);
                    CountScanning();
                }
                
            }
    }
    public enum TurnHandleControlState
    {
        None,
        Scaning_1,
        Scaning_9,
        Scaning_25,
    }
    TurnHandleControlState turnHandleControlState = TurnHandleControlState.None;
    
    float shaderTurnHandle;
    List<Material> progressBarMats = new();
    Vector3[] pointsToDraw;
    LineRenderer lineRenderer;
    BoxCollider colliderDraw;
    Bounds bounds;
    bool isWarningSystem;
    void Start()
    {
        buoyInfo = transform.parent.GetComponent<BuoyInfo>();
        if(buoyInfo.Local())
        {
            InitRender();
        }else
        {
            if(isServer)InitRender();
            if(!isLocalPlayer)return;
            InitRender();
        }
        
    }
    public void Active()
    {
        blocksCreator = transform.parent.parent.GetComponent<BlocksCreator_Main>();
        actived = true;
        isWarningSystem = FindObjectOfType<WarningSystem>();
    }
    void LateUpdate()
    {
        if(buoyInfo.Local())
        {
            ScrollSize();
            Dispaly_PaintColliderBox();
        }else
        {
            if(isServer)
            {
                Dispaly_PaintColliderBox();
            }   
            if(!isLocalPlayer)return;
            ScrollSize();
            Dispaly_PaintColliderBox();
        }
    }
    void InitRender()
    {
        pointsToDraw = new Vector3[10];
        buoyInfo.OnBuoyPosIDChange += (posId) =>
        {
            centerOfControl = posId;
        };
        buoyInfo.OnBuoyDrag += () =>
        {
            Display_InFlow();
        };
        buoyInfo.OnBuoyEndDrag += () =>
        {
            Display_OutFlow();
        };
        // 材质
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();
        foreach (var renderer in renderers)
        {
            progressBarMats.Add(renderer.material);
        }
        // linerender
        colliderDraw = GetComponent<BoxCollider>();
        if(!colliderDraw)return;
        lineRenderer = GetComponent<LineRenderer>();
        Dispaly_PaintColliderBox();
        // 初始化材质
        CurrentValue = 0;
        CurrentValue = currentValue;
        CenterOfControl = new Vector2(-1f,-1f);
        Dispaly_TurnHandleTurning(CurrentValue);
        
    }
    void ScrollSize()
    {
        // 滚轮
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        CurrentValue += scrollInput * zoomSpeed;
        CurrentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        if(buoyInfo.Local())return;
        Cmd_ScrollSize(CurrentValue);
    }
    [Command(requiresAuthority = true)]
    void Cmd_ScrollSize(float newValue)
    {
        CurrentValue = Mathf.Clamp(newValue, minValue, maxValue);
    }
    public void CountScanning(bool showWarning = true)
    {
        
        int currentValueInt = Mathf.RoundToInt(currentValue);
        if(currentValueInt>0 && currentValueInt<3)
        {
            // Debug.Log("控制1格");
            turnHandleControlState = TurnHandleControlState.Scaning_1;
            Warning("1",showWarning);
        }
        else if(currentValueInt>=3 && currentValueInt<5)
        {   
            // Debug.Log("控制9格");
            turnHandleControlState = TurnHandleControlState.Scaning_9;
            Warning("9",showWarning);
            
        }
        else if(currentValueInt>=5)
        {   
            // Debug.Log("控制25格");
            turnHandleControlState = TurnHandleControlState.Scaning_25;
            Warning("25",showWarning);
            
        }
        // 圈 + 1
        Vector2 Up,Down,Left,Right,UpRight,DownRight,DownLeft,UpLeft;
        Vector2 UpUp,DownDown,LeftLeft,RightRight;
        Vector3 UpLeft_0,UpLeft_1,UpLeft_2;
        Vector3 UpRight_0,UpRight_1,UpRight_2;
        Vector3 DownLeft_0,DownLeft_1,DownLeft_2;
        Vector3 DownRight_0,DownRight_1,DownRight_2;
        

        // 圈 + 2
        switch(turnHandleControlState)
        {
            case TurnHandleControlState.None:
                blocksScanned.Clear();
                break;
            case TurnHandleControlState.Scaning_1:
                List<Vector2> posIds_1 = new(){centerOfControl};
                CountScannedBlocksAndTetris(posIds_1);
                break;
            case TurnHandleControlState.Scaning_9:
                Up = new Vector2(centerOfControl.x,centerOfControl.y+1);
                Down = new Vector2(centerOfControl.x,centerOfControl.y-1);
                Left = new Vector2(centerOfControl.x-1,centerOfControl.y);
                Right = new Vector2(centerOfControl.x+1,centerOfControl.y);
                UpRight = new Vector2(centerOfControl.x+1,centerOfControl.y+1);
                DownRight = new Vector2(centerOfControl.x+1,centerOfControl.y-1);
                DownLeft = new Vector2(centerOfControl.x-1,centerOfControl.y-1);
                UpLeft = new Vector2(centerOfControl.x-1,centerOfControl.y+1);
                List<Vector2> posIds_9 = new(){centerOfControl,UpLeft,DownRight,UpRight,DownLeft,Up,Down,Left,Right};
                CountScannedBlocksAndTetris(posIds_9);
                break;
            case TurnHandleControlState.Scaning_25:
                Up = new Vector2(centerOfControl.x,centerOfControl.y+1);
                Down = new Vector2(centerOfControl.x,centerOfControl.y-1);
                Left = new Vector2(centerOfControl.x-1,centerOfControl.y);
                Right = new Vector2(centerOfControl.x+1,centerOfControl.y);
                UpRight = new Vector2(centerOfControl.x+1,centerOfControl.y+1);
                DownRight = new Vector2(centerOfControl.x+1,centerOfControl.y-1);
                DownLeft = new Vector2(centerOfControl.x-1,centerOfControl.y-1);
                UpLeft = new Vector2(centerOfControl.x-1,centerOfControl.y+1);
                // UpLeft * 3
                UpLeft_0 = new Vector2(UpLeft.x-1,UpLeft.y);
                UpLeft_1 = new Vector2(UpLeft.x,UpLeft.y+1);
                UpLeft_2 = new Vector2(UpLeft.x-1,UpLeft.y+1);
                // UpRight * 3
                UpRight_0 = new Vector2(UpRight.x+1,UpRight.y);
                UpRight_1 = new Vector2(UpRight.x,UpRight.y+1);
                UpRight_2 = new Vector2(UpRight.x+1,UpRight.y+1);
                // DownLeft * 3
                DownLeft_0 = new Vector2(DownLeft.x-1,DownLeft.y);
                DownLeft_1 = new Vector2(DownLeft.x,DownLeft.y-1);
                DownLeft_2 = new Vector2(DownLeft.x-1,DownLeft.y-1);
                // DownRight * 3
                DownRight_0 = new Vector2(DownRight.x+1,DownRight.y);
                DownRight_1 = new Vector2(DownRight.x,DownRight.y-1);
                DownRight_2 = new Vector2(DownRight.x+1,DownRight.y-1);
                // UpUp
                UpUp = new Vector2(Up.x,Up.y+1);
                DownDown = new Vector2(Down.x,Down.y-1);
                LeftLeft = new Vector2(Left.x-1,Left.y);
                RightRight = new Vector2(Right.x+1,Right.y);
                List<Vector2> posIds_25 = new(){centerOfControl,UpLeft,DownRight,UpRight,DownLeft,Up,Down,Left,Right,UpLeft_0,UpLeft_1,UpLeft_2,UpRight_0,UpRight_1,UpRight_2,DownLeft_0,DownLeft_1,DownLeft_2,DownRight_0,DownRight_1,DownRight_2,UpUp,DownDown,LeftLeft,RightRight};
                CountScannedBlocksAndTetris(posIds_25);
                break;
        }
    }
    public void Dispaly_PaintColliderBox()
    {
        // 获取碰撞盒的八个顶点
        // Vector3[] corners = new Vector3[5];
        // bounds = colliderDraw.bounds;
        // corners[0] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        // corners[1] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
        // corners[2] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
        // corners[3] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
        // corners[4] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        // pointsToDraw = corners.ToList();
        // 获取盒子的中心和大小
        Vector3 center = colliderDraw.center;
        Vector3 size = colliderDraw.size;
        // 获取物体的旋转信息
        Quaternion rotation = transform.rotation;
        // 应用物体的旋转到盒子的大小
        Vector3 rotatedSize = rotation * size;
        // 计算旋转后的包围盒最小和最大点
        Vector3 minPoint = center - rotatedSize * 0.5f;
        Vector3 maxPoint = center + rotatedSize * 0.5f;
        // 计算旋转后的包围盒的8个点
        maxPoint.y += 0.3f;
        pointsToDraw[0] = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        pointsToDraw[1] = new Vector3(minPoint.x, maxPoint.y, maxPoint.z);
        pointsToDraw[2] = new Vector3(maxPoint.x, maxPoint.y, maxPoint.z);
        pointsToDraw[3] = new Vector3(maxPoint.x, maxPoint.y, minPoint.z);
        pointsToDraw[4] = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        maxPoint.y += 0.6f;
        pointsToDraw[5] = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        pointsToDraw[6] = new Vector3(minPoint.x, maxPoint.y, maxPoint.z);
        pointsToDraw[7] = new Vector3(maxPoint.x, maxPoint.y, maxPoint.z);
        pointsToDraw[8] = new Vector3(maxPoint.x, maxPoint.y, minPoint.z);
        pointsToDraw[9] = new Vector3(minPoint.x, maxPoint.y, minPoint.z);
        
        lineRenderer.positionCount = pointsToDraw.Length;
        for (int i = 0; i < pointsToDraw.Length; i++)
        {
            Vector3 point =  pointsToDraw[i];
            lineRenderer.SetPosition(i, point);
        }
        
    }
    void Dispaly_TurnHandleTurning(float value)
    {
        shaderTurnHandle = Remap(value,minValue,maxValue,0,1);
        foreach (var progressBarMat in progressBarMats)
        {
            progressBarMat.SetFloat("_Progress", shaderTurnHandle);
        }   
        
    }
    static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }
    void CountScannedBlocksAndTetris(List<Vector2> posIds)
    {
        if(!actived)return;
        blocksScanned.Clear();
        foreach (var posId in posIds)
        {
            BlockBuoyHandler blockCurrent = null;
            var b = blocksCreator.blocks.Find((block) => block.posId == posId);
            if(!b)continue;
            blockCurrent = blocksCreator.blocks.Find((block) => block.posId == posId).GetComponent<BlockBuoyHandler>();
            if(blocksScanned.Contains(blockCurrent))continue;
            blocksScanned.Add(blockCurrent);
            // 可视化
            // blockCurrent.gameObject.SetActive(false);
        }
        if(blocksScanned.Count == 0)return;
        tetrisBuoysControled.Clear();
        foreach (var block in blocksScanned)
        {
            if(!block.GetTetris())continue;
            //if(block.posId!=block.GetTetris().posId)continue;// 获得稳定的正确的俄罗斯方块组
            TetrisBuoySimple tbs = block.GetTetris().transform.GetComponent<TetrisBuoySimple>();
            if(!tbs)continue;
            if(tetrisBuoysControled.Contains(tbs))continue;
            tetrisBuoysControled.Add(tbs);
        }
    }
    public List<TetrisBuoySimple> GetControledTetris()
    {
        return tetrisBuoysControled;
    }
    public TurnHandleControlState GetControlState()
    {
        CountScanning();
        return turnHandleControlState;
    }
    public void Display_InFlow()
    {
       // lineRenderer.sortingOrder = Dispaly.FlowOrder+10;
    }
    public void Display_OutFlow()
    {
       //  lineRenderer.sortingOrder = Dispaly.NotFlowOrder+1;
    }
    void Warning(string showText,bool showWarning = true)
    {
        if(!isWarningSystem || !showWarning)return;
        // if(!MechanismInPut.Instance.warningSystem)return;
        // MechanismInPut.Instance.warningSystem.inText1 = showText;
        // MechanismInPut.Instance.warningSystem.changeWarningTypes = WarningSystem.WarningType.BuoyInfo;
        
    }
}
