#if (UNITY_EDITOR) 
#undef DEBUG
 
using UnityEditor;
using System.Linq;
using System.Xml.Linq;
// 加快VScode的编译速度
public class AnalyzerRemover : AssetPostprocessor
{
    private const string menuPath = "Custom/Remove analyzers from .csproj files";
    private const string editorPrefsKey = "AnalyzerRemover.isEnabled";
 
    public static bool isEnabled
    {
        get => EditorPrefs.GetBool(editorPrefsKey, defaultValue: true);
        set => EditorPrefs.SetBool(editorPrefsKey, value);
    }
 
    [MenuItem(menuPath)]
    private static void OnMenuButtonPressed()
    {
        isEnabled = !isEnabled;
    }
 
    [MenuItem(menuPath, isValidateFunction: true)]
    private static bool ValidateMenuButton()
    {
        Menu.SetChecked(menuPath, isEnabled);
        return true;
    }
 
    private static string OnGeneratedCSProject(string path, string contents)
    {
        if (!isEnabled) return contents;
 
        var csprojXML = XDocument.Parse(contents);
        var ns = csprojXML.Root.Name.Namespace;
        var analyzers = csprojXML
            .Descendants(ns + "Analyzer")
            .Where(a => a.Attribute("Include")?.Value.Contains("SourceGenerators") ?? false)
            .ToList();
 
        if (analyzers.Count == 0) return contents;
 
        analyzers.Remove();
#if DEBUG
        UnityEngine.Debug.Log($"Removed analyzers from: {path}");
#endif
        return csprojXML.ToString();
    }
}
#endif