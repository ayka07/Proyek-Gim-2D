#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromBootstrap
{
    static PlayFromBootstrap()
    {
        SceneAsset bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/_Project/Scenes/Scn_Bootstrap.unity");
        if (bootstrap != null)
        {
            EditorSceneManager.playModeStartScene = bootstrap;
        }
    }
}
#endif
