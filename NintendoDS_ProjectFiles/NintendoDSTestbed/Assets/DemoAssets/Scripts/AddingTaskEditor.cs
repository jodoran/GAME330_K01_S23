using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(AddingTask))]
public class AddingTaskEditor : Editor
{
    public ReorderableList list;
    SerializedProperty Bowl;
    SerializedProperty Instructions;
    SerializedProperty ProgressBar;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(Bowl);
        EditorGUILayout.PropertyField(Instructions);
        EditorGUILayout.PropertyField(ProgressBar);

        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable()
    {
        Bowl = serializedObject.FindProperty("Bowl");
        Instructions = serializedObject.FindProperty("Instructions");
        ProgressBar = serializedObject.FindProperty("ProgressBar");

        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Plates"), true, true, true, true);

        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("PlateColor"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 90, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Ingredient"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Name"), GUIContent.none);
        };


        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Ingredient List");
        };
    }
}
