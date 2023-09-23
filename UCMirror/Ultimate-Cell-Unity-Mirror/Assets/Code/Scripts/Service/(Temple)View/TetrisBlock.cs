using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    
    // 旋转变量
    private static Vector3 rotationPoint = new Vector3((float)0.5, (float)0.5, 0);

    // 存在时间
    private float previousTime;

    // 下落时间
    public float fallTime = 0.8f;

    // 左右移动速度
    public float moveSpeed = 0.5f;

    // 将配置高度导入方块中
    private static int _height;

    // 将配置宽度导入方块中
    private static int _width;

    // 检测模拟器
    private static Transform[,] _grid;

    private int logicType;

    // 砖块逻辑坐标存储
    private Dictionary<int, List<int>> _blocks;

    private BroadcastClass broadcastClass;

    private CommunicationInteractionManager CommunicationManager;

    private void Start()
    {
        Debug.LogError(transform.name);
        // 初始化生成战场检测
        _height = BattlefieldClass.height;

        _width = BattlefieldClass.width;

        // 初始化生成砖块在战场中的逻辑位置
        AddToBlocks();


        // 暂时获取方式
        CommunicationManager = GameObject.Find("SceneLoader").gameObject.GetComponent<CommunicationInteractionManager>();

        broadcastClass = CommunicationManager.GetComponent<BroadcastClass>();

        broadcastClass.OnUnitDie += OnUnitDie;

        broadcastClass.OnUIPosChange += OnUIPosChange;

    }

    private void OnUIPosChange(Vector3 Info) 
    {
        var InfoPosZ = Info.z;

        var LogicPos = Remap(InfoPosZ, (float)-8, (float)8, (float)-5, (float)5);

        var MoveType = Math.Round(LogicPos, MidpointRounding.AwayFromZero);

        Debug.Log("MoveType" + MoveType);

        if (MoveType > 0) 
        {
            var step = Math.Abs(logicType) + MoveType;

            Debug.Log("step" + step);

            for (int i = 0; i < step; i++) 
            {
                MoveUp();
            }
        }

        if (MoveType < 0)
        {
            var step = Math.Abs(MoveType) + logicType;

            Debug.Log("step" + step);

            for (int i = 0; i < step; i++)
            {
                MoveDown();
            }

        }

        logicType = (int)MoveType;

    }

    private void MoveDown() 
    {
        // 逻辑位置更新
        UpdateBlocksLogic(MoveType.Down);

        // 显示位置更新
        transform.position += new Vector3(0, 0, -1);

        Debug.Log("向下移动 1 格");

        // 检查移动
        if (!ValidMove())
        {
            // 逻辑位置回退
            UpdateBlocksLogic(MoveType.Up);

            // 显示位置回退
            transform.position -= new Vector3(0, 0, -1);

            return;
        }
    }

    private void MoveUp() 
    {
        UpdateBlocksLogic(MoveType.Up);

        transform.position += new Vector3(0, 0, 1);

        Debug.Log("向下移动 1 格");

        if (!ValidMove())
        {
            UpdateBlocksLogic(MoveType.Down);

            transform.position -= new Vector3(0, 0, 1);

            return;
        }
    }

    public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

    private void onTransPos(Transform trans)
    {
        List<UnitInfoClass> unitInfoUpdateClassList = new();

        foreach (Transform children in trans)
        {
            UnitInfoClass unitInfoClass = new();

            // var tansInfo = FindObjectOfType<BattlefieldControlManager>().transInfo;

            // unitInfoClass.UnitIndexId = tansInfo[children];

            unitInfoClass.UnitPos = children.transform.position;

            unitInfoClass.CreateUnit = false;

            unitInfoUpdateClassList.Add(unitInfoClass);
        }

        CommunicationManager.TetrisInfoUpdate(unitInfoUpdateClassList);
    }

    private void OnUnitDie(int UnitIndexId) 
    {
        Debug.Log("OnUnitDie" + UnitIndexId);
    }

    public enum MoveType 
    {
        Up,
        Down,
        Move,
        Back,
        Trans,
        BackTrans,
    }

    private void UpdateBlocksLogic (MoveType infoType) 
    {
        switch (infoType) 
        {
            case MoveType.Up:
                foreach (int i in _blocks.Keys) 
                {
                    var posInfo = _blocks[i];

                    posInfo[1] += 1;
                }
                break;
            case MoveType.Down:
                foreach (int i in _blocks.Keys)
                {
                    var posInfo = _blocks[i];

                    posInfo[1] -= 1;
                }
                break;
            case MoveType.Move:
                foreach (int i in _blocks.Keys)
                {
                    var posInfo = _blocks[i];

                    posInfo[0] += 1;
                }
                break;
            case MoveType.Back:
                foreach (int i in _blocks.Keys)
                {
                    var posInfo = _blocks[i];

                    posInfo[0] -= 1;
                }
                break;
            case MoveType.Trans:
                foreach (int i in _blocks.Keys)
                {
                    var posInfo = _blocks[i];

                    var posX = posInfo[0];

                    var posY = posInfo[1];
                }

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*var SimTrans = FindObjectOfType<SpawnClass>().SimTrans;

        // important 这里需要将两个砖块放到同一位置进行形状变换
        // 否则会产生一个可以进行变换，另一个不能变换的情况
        SimTrans.position = transform.position;*/

        // 向下移动
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 逻辑位置更新
            UpdateBlocksLogic(MoveType.Down);

            // 显示位置更新
            transform.position += new Vector3(0, 0, -1);

            Debug.Log("向下移动 1 格");

            // 检查移动
            if (!ValidMove())
            {
                // 逻辑位置回退
                UpdateBlocksLogic(MoveType.Up);

                // 显示位置回退
                transform.position -= new Vector3(0, 0, -1);

                return;
            }
        }

        // 向上移动
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateBlocksLogic(MoveType.Up);

            transform.position += new Vector3(0, 0, 1);

            Debug.Log("向下移动 1 格");

            if (!ValidMove()) 
            {
                UpdateBlocksLogic(MoveType.Down);

                transform.position -= new Vector3(0, 0, 1);

                return;
            }
        }

        // 形状变换
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            UpdateBlocksLogic(MoveType.Trans);

       


            if (!ValidMove())
            {
              

                UpdateBlocksLogic(MoveType.BackTrans);

              

                TransMoveAndRoteUp(transform);

               
            }
        }

        // 向右加速
        if (Time.time - previousTime > (Input.GetKey(KeyCode.RightArrow) ? fallTime / 10 : fallTime))
        {
            // 逻辑位置更新
            UpdateBlocksLogic(MoveType.Move);

            transform.position += new Vector3(1, 0, 0);

            if (!ValidMove())
            {
                // 逻辑位置更新
                UpdateBlocksLogic(MoveType.Back);

                transform.position -= new Vector3(1, 0, 0);

                AddToGrid();

                CheckForLines();

                if (ValidCreate()) 
                {
                    // FindObjectOfType<BattlefieldControlManager>().NewTetromino();
                }

                this.enabled = false;

                return;
            }
            previousTime = Time.time;
        }

        onTransPos(transform);

        // CoulateSimPos(SimTrans);

    }

    private bool ValidCreate()
    {
        for (int i = 0; i < _height; i ++)
        {
            if (_grid[0, i] != null)
            {
                return false;
            }
        }

        return true;

    }

    // 无法变形时，移动位置并变形
    private void TransMoveAndRoteUp(Transform trans)
    {
        trans.RotateAround(trans.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);

        if (RoteUpInfo(trans, -1)) return;

        if (RoteUpInfo(trans, -2)) return;

        if (RoteUpInfo(trans, 1)) return;

        if (RoteUpInfo(trans, 2)) return;

        trans.RotateAround(trans.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);

        Debug.Log("无法变形");
    }

    // 变形失败时位置变更函数
    private bool RoteUpInfo(Transform trans, int _moveStep)
    {
        trans.position += new Vector3(0, _moveStep, 0);

        if (!ValidMove())
        {
            trans.position += new Vector3(0, -_moveStep, 0);

            return false;
        }

        return true;
    }

    // 计算砖块预计位置
    private void CoulateSimPos(Transform SimTrans)
    {
        var move = ValidMove();

        if (move)
        {
            SimTrans.position += new Vector3(1, 0, 0);

            CoulateSimPos(SimTrans);
        }
        else
        {
            SimTrans.position -= new Vector3(1, 0, 0);
        }
    }

    // 检查行列
    void CheckForLines()
    {
        for (int i = _width - 1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                DeleteLine(i);

                RowDown(i);
            }
        }
    }

    // 判断是否有整行生成
    bool HasLine(int i)
    {
        for (int j = 0; j< _height; j++)
        {
            if (_grid[i, j] == null)

                return false;
        }

        return true;
    }

    // 删除整行
    void DeleteLine(int i)
    {
        for (int j = 0; j < _height; j++)
        {
            Destroy(_grid[i, j].gameObject);
            _grid[i, j] = null;
        }
    }

    // 行列消除移动
    public void RowDown(int y)
    {
        for (int i = y; i >= 0; i--)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_grid[i, j] != null)
                {
                    _grid[i + 1, j] = _grid[i, j];
                    _grid[i, j] = null;
                    _grid[i + 1, j].transform.position += new Vector3(1, 0, 0);
                }
            }
        }
    }

    // 删除单个砖块
    public static bool DeleteAlone(int i, int j)
    {
        if (_grid[i, j] != null)
        {
            Destroy(_grid[i, j].gameObject);

            _grid[i, j] = null;

            MoveLine(i, j);

            return true;
        }
        else 
        {
            return false;
        }
    }

    private static void MoveLine(int x, int y)
    {
        for (int i = x; i >= 0; i--)
        {
            if (_grid[i, y] != null)
            {
                _grid[i + 1, y] = _grid[i, y];
                _grid[i, y] = null;
                _grid[i + 1, y].transform.position += new Vector3(1, 0, 0);
            }
        }
    }

    // 初始化加入检测模拟器
    // TODO 归一化
    void AddToGrid()
    {
        int sum = 0;

        foreach (Transform children in transform)
        {
            int roundedX = _blocks[sum][0];

            int roundedY = _blocks[sum][1];

            if (_grid[roundedX, roundedY] == null)
            {
                _grid[roundedX, roundedY] = children;
            }
            else 
            {
                this.enabled = false;

                return;
            }
            sum ++ ;
        }
    }

    void AddToBlocks()
    {
        int sum = 0;

        Dictionary<int, List<int>> blocks = new Dictionary<int, List<int>>();

        foreach (Transform children in transform)
        {
            int roundedX = (int)children.transform.localPosition.x;

            int roundedY = (int)children.transform.localPosition.y + Mathf.RoundToInt(_height / 2);

            List<int> list = new List<int>() { roundedX, roundedY };

            blocks.Add(sum, list);

            sum++;
        }

        _blocks = blocks;
    }

    // 删除预计位置砖块
    private void DestorySim()
    {
        var simObj = FindObjectOfType<SpawnClass>().SimTrans;

        GameObject.Destroy(simObj.gameObject);

    }


    // Valid Move 是检测方块处于该位置时是否合法
    // 如果先检测该位置是否合法再进行移动会导致砖块出界/数组越位等问题
    public bool ValidMove()
    {
        // 获取children在碰撞模拟器中的坐标
        foreach (int i in _blocks.Keys)
        {
            int roundedX = _blocks[i][0];

            int roundedY = _blocks[i][1];

            // 边界判断
            if (roundedX < 0 || roundedX >= _width || roundedY < 0 || roundedY >= _height)
            {
                return false;
            }

            // 砖块判断
            if (_grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }

        return true;
    }
}
