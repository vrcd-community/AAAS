
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class GetContext : UdonSharpBehaviour
{
	public string Defect;
	private string DefectName;
	private int Index;
	private UdonBehaviour main;

	private Text text;

    private void Start()
    {
        text = transform.Find("Title").GetComponent<Text>();
		if (DefectName == null) { DefectName = Defect; }
        text.text = DefectName;
        main = transform.parent.parent.parent.parent.parent.Find("脚本挂载").GetComponent<UdonBehaviour>();
    }

    public void Init()
	{
        text = transform.Find("Title").GetComponent<Text>();
        text.text = DefectName;
		main = transform.parent.parent.parent.parent.parent.Find("脚本挂载").GetComponent<UdonBehaviour>();
	}

	public void OnChick()
	{
		main.SetProgramVariable("Index", Index);
		main.SendCustomEvent("CheckToken");
	}

	public void NewIndex()
	{
        main.SendCustomEvent("NewIndex");
    }
}

