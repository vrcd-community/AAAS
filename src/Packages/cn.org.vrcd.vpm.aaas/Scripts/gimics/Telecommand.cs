using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace cdes_presets
{
    public class Telecommand : UdonSharpBehaviour
    {
        [SerializeField] private Sliden _sliden = null;
        private bool isPickUp = false;
        private bool isVR = false;
        private bool isVRTrigger = false;

        private void Start()
        {
            isVR = Networking.LocalPlayer.IsUserInVR();
        }

        public override void OnPickup()
        {
            isPickUp = true;
        }

        public override void OnDrop()
        {
            isPickUp = false;
        }

        public void Update()
        {
            if (!isPickUp) return;
            if (!isVR)
            {
                // 在这里修改PC玩家翻动上一页的按键，只需替换KeyCode.Q当中的Q为任意按键即可
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _sliden.PrevPage();
                }
                // 在这里修改PC玩家翻动上一页的按键，只需替换KeyCode.E当中的E为任意按键即可
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    _sliden.NextPage();
                }
            }
        }

        // 如果你想要修改VR玩家翻动页面的具体方式，请重写这一整段逻辑
        // _sliden.PrevPage();对应PPT翻至上一页的事件，_sliden.NextPage();对应PPT翻至下一页的事件
        public override void InputLookVertical(float isUpDown, UdonInputEventArgs args)
        {
            if (isVRTrigger) return;
            if (isVR)
            {
                if (isUpDown > 0.5)
                {
                    _sliden.PrevPage();
                    isVRTrigger = true;
                }
                else if (isUpDown < -0.5)
                {
                    _sliden.NextPage();
                    isVRTrigger = true;
                }
                else
                {
                    isVRTrigger = false;
                }
                return;
            }
        }
    }
}