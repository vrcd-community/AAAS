
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace cdes_presets
{
    // 这个脚本在接收事件（按钮被按下）的时候发送输入框内的玩家名称到 CameraFollow 脚本
    public class MenuTarget : UdonSharpBehaviour
    {
        public CameraFollow CamFollow = null;
        public InputField input = null;

        public void _OnInputEndEdit()
        {
            // 当 input，input.text，摄像机不为空，且 input.text 长度大于0时
            if (input != null && input.text != null && CamFollow != null && input.text.Length > 0)
            {
                //向 CameraFollow 发送 StartTargetSync，并且携带输入的内容信息（被用作玩家名称输入）
                CamFollow.StartTargetSync(input.text);
            }
        }
    }
}