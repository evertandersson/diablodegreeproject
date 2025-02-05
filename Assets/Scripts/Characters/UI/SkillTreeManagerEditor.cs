using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTreeManager))]
public class SkillTreeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillTreeManager manager = (SkillTreeManager)target;
        if (GUILayout.Button("Redraw lines"))
        {
            manager.ViewTreeInEditor();
        }
    }
}
