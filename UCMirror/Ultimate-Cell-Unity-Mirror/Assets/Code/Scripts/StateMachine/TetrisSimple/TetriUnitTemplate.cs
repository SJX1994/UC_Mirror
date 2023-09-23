using UnityEngine;

[CreateAssetMenu(fileName = "TetriUnit", menuName = "TetriUnit/data", order = 1)]

public class TetriUnitTemplate: ScriptableObject
{
    [Tooltip("组内ID")]
    public int index = 0;
}