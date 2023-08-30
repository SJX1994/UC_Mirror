using UnityEngine;

public class SpawnClass : MonoBehaviour
{
    public GameObject[] Tetrominoes;

    public GameObject[] TetrominoesSimulated;

    [Header("生成砖块")]
    public Transform Trans = null;

    [Header("预计生成砖块")]
    public Transform SimTrans = null;
}
