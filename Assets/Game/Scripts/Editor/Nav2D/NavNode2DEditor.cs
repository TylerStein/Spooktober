using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavNode2D))]
[CanEditMultipleObjects]
public class NavNode2DEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        NavNode2D component = (NavNode2D)target;

        if (GUILayout.Button("+ Normal")) {
            GameObject addedObject = AddNode(component, NavNodeType.NORMAL);
            Selection.SetActiveObjectWithContext(addedObject, addedObject);
        }

        if (GUILayout.Button("+ Stairs")) {
            GameObject addedObject = AddNode(component, NavNodeType.STAIRS);
            Selection.SetActiveObjectWithContext(addedObject, addedObject);
        }

        if (GUILayout.Button("- Remove Node")) {
            RemoveNode(component);
        }
    }

    public GameObject AddNode(NavNode2D source, NavNodeType type) {
        Undo.SetCurrentGroupName("Create Nav2D Node All");
        int undoGroup = Undo.GetCurrentGroup();

        GameObject newObject = new GameObject();
        NavNode2D newNavNode = newObject.AddComponent<NavNode2D>();
        newNavNode.nav2d = source.nav2d;
        newNavNode.navNodeType = type;
        newNavNode.connections.Add(source);

        if (source.nav2d) {
            int count = source.nav2d.nodes.Count;
            newNavNode.name = $"Node ({count}) ({type.ToString()})";
        }

        newObject.transform.position = source.transform.position + Vector3.right;
        newObject.transform.parent = source.transform.parent;
        source.connections.Add(newNavNode);

        Undo.RegisterCreatedObjectUndo(newObject, "Create Nav2D Node Object");
        Undo.CollapseUndoOperations(undoGroup);

        return newObject;
    }

    public void RemoveNode(NavNode2D source) {
        Undo.SetCurrentGroupName("Remove Nav2D Node");
        int group = Undo.GetCurrentGroup();

        Undo.DestroyObjectImmediate(source.gameObject);
        Undo.CollapseUndoOperations(group);
    }
}
