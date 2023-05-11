using UnityEngine;
using UnityEditor;

public class MaterialShaderConverter : EditorWindow
{
    [MenuItem("Window/Material Shader Converter")]
    private static void ShowWindow()
    {
        MaterialShaderConverter window = GetWindow<MaterialShaderConverter>();
        window.titleContent = new GUIContent("Material Shader Converter");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Shader Converter", EditorStyles.boldLabel);

        if (GUILayout.Button("Convert Materials"))
        {
            ConvertMaterials();
        }
    }

    private void ConvertMaterials()
    {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material != null && material.shader.name.Contains("Universal Render Pipeline/Lit"))
            {
                Undo.RecordObject(material, "Change Material Shader");
                material.shader = Shader.Find("Standard");
                EditorUtility.SetDirty(material);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Material Shader Conversion complete.");
    }
}
