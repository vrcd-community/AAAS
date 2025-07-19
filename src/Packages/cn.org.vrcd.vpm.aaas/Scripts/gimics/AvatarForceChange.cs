
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace cdes_presets
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AvatarForceChange : UdonSharpBehaviour
    {
        [Header("这是一个强制切换本地虚拟形象脚本")]
        [SerializeField]private VRC_AvatarPedestal Pedestal;
        void OnEnable()
        {
            Pedestal.SetAvatarUse(Networking.LocalPlayer);
        }
        void Start()
        {
            Pedestal.SetAvatarUse(Networking.LocalPlayer);
        }
    }
}

