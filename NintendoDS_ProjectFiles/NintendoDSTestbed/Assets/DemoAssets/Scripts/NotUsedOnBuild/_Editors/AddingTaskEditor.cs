using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// Editor to handle the look of the Inspector Window for the AddingTask class
[CustomEditor(typeof(AddingTask))]
public class AddingTaskEditor : Editor
{
    //Top Screen Variables
    //Variables for gameObjects containing the Task Instructions & Progress
    SerializedProperty Bowl;
    SerializedProperty Instructions;
    SerializedProperty ProgressBar;

    //Bottom Screen Variables
    //Contains each plate in the scene (Color, Ingredient Icon, Color Name) 
    public ReorderableList list;

    //Sets the Inspector Window with the Public Variables
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Top Screen Variables
        EditorGUILayout.PropertyField(Bowl);
        EditorGUILayout.PropertyField(Instructions);
        EditorGUILayout.PropertyField(ProgressBar);

        //Bottom Screen Variables List
        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    //Configures Inspector Variables with the public variables in AddingTask class
    public void OnEnable()
    {
        //Sets Top Screen Variables leaving unformatted
        Bowl = serializedObject.FindProperty("Bowl");
        Instructions = serializedObject.FindProperty("Instructions");
        ProgressBar = serializedObject.FindProperty("ProgressBar");

        //Creates a list for multiple varaibles to be contained and formatted 
        //Adds varaible containers from struct list in AddingTask
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Plates"), true, true, true, true);

        //Formats list to present all variables on single line in inspector
        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            //Color
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("PlateColor"), GUIContent.none);

            //Ingredient
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 90, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Ingredient"), GUIContent.none);

            //Color Name
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Name"), GUIContent.none);
        };

        //Creates header over list labeling variables contained as descriptors of a plate
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Plate List");
        };
    }
}
