using UnityEditor;
using UC_PlayerData;
[CustomEditor(typeof(RunModeChanger))]
public class RunModeChangerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RunModeChanger myClass = (RunModeChanger)target;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("运行模式", EditorStyles.boldLabel);

        // Save the modified value to EditorPrefs
        // RunMode newValue = (RunMode)EditorGUILayout.EnumPopup("运行模式:", myClass.runMode);
        // if (newValue != myClass.runMode)
        // {
        //     myClass.runMode = newValue;
        //     // EditorPrefs.SetInt("MyValue", newValue);
        // }
    }

}
