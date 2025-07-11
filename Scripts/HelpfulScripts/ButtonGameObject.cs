using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace cdes_presets
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ButtonGameObject : UdonSharpBehaviour
    {
        [Header("这是一个对象按钮脚本")]
        [Header("是否同步？")]
        [SerializeField] private bool isGlobal = false;
        [Header("目标开启对象")]
        public GameObject[] TargetGameObjectOn;
        [Header("目标关闭对象")]
        public GameObject[] TargetGameObjectOff;
        [Header("按钮")]
        public Button buttonGameObject;

        public void isTrigger()
        {
            if (!isGlobal)
            {
                ButtonTriggered();
            }
            else
            {
                if (!Networking.IsOwner(gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                }

                ButtonTriggered();
                RequestSerialization();
            }
        }
        public override void OnDeserialization()
        {
            if (isGlobal && !Networking.IsOwner(gameObject))
            {
                ButtonTriggered();
            }
        }

        public void ButtonTriggered()
        {
            foreach (var gameObject in TargetGameObjectOn)
            {
                gameObject.SetActive(true);
            }
            foreach (var gameObject in TargetGameObjectOff)
            {
                gameObject.SetActive(false);
            }
        }
    }
}