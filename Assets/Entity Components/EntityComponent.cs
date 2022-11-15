using UnityEditor;
using UnityEngine;

public class EntityComponent : ScriptableObject
{
    public string m_Metal;
    public float m_Amount;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/EntityComponent")]
    public static void CreateEntityComponent()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Entity Component", "New Entity Component", "Asset", "Save Entity Component", "Assets");
        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EntityComponent>(), path);
    }
#endif
}
