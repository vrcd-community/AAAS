
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace cdes_presets
{
    public class CameraSwitchManager : UdonSharpBehaviour
    {
        public CameraManage CameraManageOBJ = null;
        [SerializeField] private Button[] button;
        [UdonSynced][HideInInspector] public string buttonName = null;

        public void OnButtonChange(int buttonOrder)
        {
            CameraManageOBJ.SetCameraPos(buttonOrder);
        }

        //如果你有布置超过9个点位的需求，在这里添加事件到相应数量即可
        public void IsTrigger0() => OnButtonChange(0);
        public void IsTrigger1() => OnButtonChange(1);
        public void IsTrigger2() => OnButtonChange(2);
        public void IsTrigger3() => OnButtonChange(3);
        public void IsTrigger4() => OnButtonChange(4);
        public void IsTrigger5() => OnButtonChange(5);
        public void IsTrigger6() => OnButtonChange(6);
        public void IsTrigger7() => OnButtonChange(7);
        public void IsTrigger8() => OnButtonChange(8);
    }
}