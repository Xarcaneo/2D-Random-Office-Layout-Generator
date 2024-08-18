using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutManager), true)]  
public class LayoutGeneratorEditor : Editor
{
    LayoutManager layoutManager;

    private void Awake()
    {
        layoutManager = (LayoutManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Layout"))
        {
            layoutManager.GenerateLayout();
        }
    }
}
