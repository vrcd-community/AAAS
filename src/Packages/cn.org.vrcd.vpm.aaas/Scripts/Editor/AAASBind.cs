using Mono.Cecil;
using Sonic853.Translate;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class NewBehaviourScript : Editor
{
	[MenuItem("Tools/CCES/Bind")]
	public static void Bind()
	{
		var tlsObject = FindObjectOfType<TranslateManager>();

		var objText = FindObjectsOfType<Text>();
		var objTMP = FindObjectsOfType<TMP_Text>();

		var dstText = new List<Text>();
		var dstTMP = new List<TMP_Text>();

		for(int i = 0; i < objText.Length; i++)
		{
			if (Regex.IsMatch(getScenePath(objText[i].gameObject), @"AAAS"))
			{
				dstText.Add(objText[i]);
			}
		}

		for (int i = 0; i < objTMP.Length; i++)
		{
			if (Regex.IsMatch(getScenePath(objTMP[i].gameObject), @"AAAS"))
			{
				dstTMP.Add(objTMP[i]);
			}
		}

        // TODO: Use a right way to add translation OR just remove this library
		// tlsObject.texts = dstText.ToArray();
		// tlsObject.tMP_Texts = dstTMP.ToArray();

        var translateManagerType = typeof(TranslateManager);
        var textsField = translateManagerType.GetField("texts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var tMP_TextsField = translateManagerType.GetField("tMP_Texts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (textsField != null)
        {
            textsField.SetValue(tlsObject, dstText.ToArray());
        }
        else
        {
            Debug.LogError("TranslateManager does not have a 'texts' field.");
        }

        if (tMP_TextsField != null)
        {
            tMP_TextsField.SetValue(tlsObject, dstTMP.ToArray());
        }
        else
        {
            Debug.LogError("TranslateManager does not have a 'tMP_Texts' field.");
        }

		EditorUtility.SetDirty(tlsObject);
	}

	private static string getScenePath(GameObject obj)
	{
		if (obj == null) return string.Empty;

		Transform current = obj.transform;
		string path = "/" + current.name;

		while (current.parent != null)
		{
			current = current.parent;
			path = "/" + current.name + path;
		}

		return path;
	}
}
