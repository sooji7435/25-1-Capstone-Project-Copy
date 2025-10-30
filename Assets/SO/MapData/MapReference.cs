using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapReference : ScriptableObject
{
#if UNITY_EDITOR
    public SceneAsset[] sceneList;
#endif
    public List<string> sceneNames;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var scene in sceneList)

            if (!sceneNames.Contains(scene.name))
                sceneNames.Add(scene.name);
    }
#endif
}