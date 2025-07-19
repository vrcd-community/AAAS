using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class QuickNameChoose : UdonSharpBehaviour
{
    public Text DisplayerName; // 用于显示当前选中的玩家名字的文本框。
    public UdonBehaviour Main; // 关键！这里需要拖入上面那个 CameraFollow 脚本所在的对象。
    private int Index; // 存储当前选中的玩家在列表中的索引。
    private VRCPlayerApi LocalPlayer;
    private VRCPlayerApi[] Players;

    public string[] Displayers; // 存储所有玩家名字的字符串数组。

    public GameObject _prefab; // 玩家按钮的预制体。
    public Transform ObjParent; // 所有生成的按钮都将放在这个父对象下。
    public GameObject[] Buttoms; // 用于管理所有已生成的按钮对象。

    void Start()
    {
        LocalPlayer = Networking.LocalPlayer;
        Buttoms = new GameObject[0]; // 初始化按钮数组为空。
    }

    // 用于刷新和显示玩家列表。
    public void Display()
    {
        Main.SendCustomEvent("ResetDisplay") ;
        //更新玩家列表
        Players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]; // 创建正确大小的数组。
        VRCPlayerApi.GetPlayers(Players); // 获取所有玩家。
        Displayers = new string[Players.Length]; // 创建对应大小的名字数组。
        for (int i = 0; i < Players.Length; i++)
        {
            var Name = Players[i].displayName;
            Displayers[i] = Name;
        }
        //更新玩家名称

        var Buttomss = new GameObject[Displayers.Length];
        if (Displayers.Length > Buttoms.Length)
        {
            for (int i = Buttoms.Length; i < Buttomss.Length; i++)
            {
                var a = Instantiate(_prefab, ObjParent);
                Buttomss[i] = a;
            }
            for (int i = 0; i < Buttoms.Length; i++)
            {
                Buttomss[i] = Buttoms[i];
            }
            Buttoms = Buttomss;
        }
        else if (Displayers.Length < Buttoms.Length)
        {
            for (int i = Displayers.Length; i < Buttoms.Length; i++)
            {
                Destroy(Buttoms[i]);
            }
            for (int i = 0; i < Buttomss.Length; i++)
            {
                Buttomss[i] = Buttoms[i];
            }
            Buttoms = Buttomss;
        }

        for (int i = 0; i < Displayers.Length; i++)
        {
            var cmpObj = Buttoms[i].GetComponent<UdonBehaviour>();
            cmpObj.SetProgramVariable("DefectName", $"#{i}  " + Displayers[i]);
            cmpObj.SetProgramVariable("Index", i);
            cmpObj.SendCustomEvent("Init");
            Buttoms[i] = cmpObj.gameObject;
        }
    }

    public void SetToken()
    {
        if (Index >= Displayers.Length){return;}
        Main.SetProgramVariable("DisPlayName",Displayers[Index]) ;
        Main.SendCustomEvent("QuickPlayerName");
    }

    public void CheckToken()
    {
        DisplayerName.text = Displayers[Index];
    }


}