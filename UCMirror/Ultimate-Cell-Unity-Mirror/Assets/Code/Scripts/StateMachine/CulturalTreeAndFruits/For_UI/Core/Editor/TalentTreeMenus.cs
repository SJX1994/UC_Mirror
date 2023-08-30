using UnityEngine;
using UnityEditor;

namespace com.cygnusprojects.TalentTree.Editor
{
	public static class TalentTreeMenus 
	{
		[MenuItem("Tools/Cygnus Projects/Talent Tree Editor")]
        public static void InitTalentTreeEditor()
        {
            TalentTreeWindow.InitEditorWindow();
        }

        [MenuItem("Tools/Cygnus Projects/Talent Tree UI Template")]
        public static void InitTalentTreeUITemplate()
        {
            TalentTreeUITemplateWindow.InitEditorWindow();
        }
    }
}
