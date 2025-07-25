using AAAS.Slide.Core;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace AAAS.Slide.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(VRC_Pickup))]
    public class SlidePresenterRemoteController : UdonSharpBehaviour {
        [SerializeField] protected SlideCore slideCore;

        private bool isPickUp;
        private bool isVR;
        private bool isVRTriggered;

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError("[SlideController] SlideCore is not assigned. Please assign a SlideCore instance in the inspector.", this);
                return;
            }

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
            if (isVR) return;

            // 在这里修改PC玩家翻动上一页的按键，只需替换KeyCode.Q当中的Q为任意按键即可
            if (Input.GetKeyDown(KeyCode.Q))
            {
                slideCore._PreviousPage();
            }
            // 在这里修改PC玩家翻动上一页的按键，只需替换KeyCode.E当中的E为任意按键即可
            else if (Input.GetKeyDown(KeyCode.E))
            {
                slideCore._NextPage();
            }
        }

        // 如果你想要修改VR玩家翻动页面的具体方式，请重写这一整段逻辑
        // _sliden.PrevPage();对应PPT翻至上一页的事件，_sliden.NextPage();对应PPT翻至下一页的事件
        public override void InputLookVertical(float isUpDown, UdonInputEventArgs args)
        {
            if (isVRTriggered) return;
            if (!isVR) return;

            if (isUpDown > 0.5)
            {
                slideCore._PreviousPage();
                isVRTriggered = true;
            }
            else if (isUpDown < -0.5)
            {
                slideCore._NextPage();
                isVRTriggered = true;
            }
            else
            {
                isVRTriggered = false;
            }
        }
    }
}
