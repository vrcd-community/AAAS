
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace cdes_presets
{
    // 这个脚本在接收事件（按钮被按下）的时候发送发起事件的玩家名称到 CameraFollow 脚本
    public class MenuTargetMe : UdonSharpBehaviour
    {
        public CameraFollow CamFollow = null;

        //当按钮被按下时
        public void _OnButtonDown()
        {
            //当摄像机不为空时
            if (CamFollow != null)
            {
                //向 CameraFollow 发送 StartTargetSync 事件，携带点击了按钮的玩家名称（被用作玩家名称输入）
                CamFollow.StartTargetSync(Networking.LocalPlayer.displayName);
            }
        }
    }
}