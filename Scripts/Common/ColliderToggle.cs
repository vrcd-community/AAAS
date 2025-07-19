
using System;
using System.Text;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class ColliderToggle : UdonSharpBehaviour
{
    [Header("这是一个对象开关脚本（碰撞体事件）")]
    [Header("目标对象(开启)")]
    [SerializeField] private GameObject[] targetGameObject;
    [UdonSynced] private bool[] isEnabled;

    private void Start()
    {
        isEnabled = new bool[targetGameObject.Length];

        for (int i = 0; i < targetGameObject.Length; i++)
        {
            isEnabled[i] = targetGameObject[i].activeSelf;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            ToggleLocal();
        }

    }

    //当玩家离开触发器时
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            ToggleLocal();
        }
    }

    public void ToggleLocal()
    {
        for (int num = 0; num < targetGameObject.Length; num++)
        {
            if (targetGameObject[num] != null)
            {
                targetGameObject[num].SetActive(!targetGameObject[num].activeSelf);
            }
        }
    }
}
