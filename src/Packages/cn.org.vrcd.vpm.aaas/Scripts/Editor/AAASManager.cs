using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using UnityEditor.Search;



public class AAASManager : EditorWindow
{
    int gameobjectButtonCount = 0;
    string toggleName = null;
    public GameObject[] targetGameObject = null;

    [MenuItem("Tools/CCES/AAAS")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AAASManager));
    }

    void OnGUI()
    {        
        GUILayout.Label("设置界面");

        if (GUILayout.Button("创建对象开关"))
        {
        }

        toggleName = EditorGUILayout.TextField("名称", toggleName);

        //targetGameObject = (GameObject[])EditorGUILayout.ObjectField(targetGameObject, true);

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("targetGameObject");

        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("生成"))
        {
            Debug.Log(toggleName);
        }

        if (GUILayout.Button("关闭"))
        {
            Close();
        }
    }
}