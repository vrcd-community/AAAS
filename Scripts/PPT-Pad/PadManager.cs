
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace cdse_presets
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PadManager : UdonSharpBehaviour
    {
        [Header("Basic")]
        [SerializeField] private GameObject[] targetPage;
        [SerializeField] private GameObject[] interactToggle;
        private int count = 1;

        [Header("Home")]
        [SerializeField] private Text LocalPlayerName;
        [SerializeField] private Text timeDis;
        private string disType = "t";

        void Start()
        {
            //获取玩家名称
            LocalPlayerName.text = Networking.LocalPlayer.displayName;
        }

        private void Update()
        {
            //时间
            timeDis.text = System.DateTime.Now.ToString(disType);
        }

        public override void Interact()
        {
            //抓起面板->关闭对象
            if (count == 1)
            {
                foreach (var gameObject in interactToggle)
                {
                    if (gameObject.activeSelf == true)
                    {gameObject.SetActive(false);}
                    else
                    {gameObject.SetActive(true);}
                }
                count++;
            }
            else
            {return;}
        }


        public void SwitchHome()
        {
            foreach (var target in targetPage)
            {
                if (target.name == "home")
                { target.SetActive(true); }
                else
                { target.SetActive(false); }
            }
        }
        public void SwitchRemote()
        {
            foreach (var target in targetPage)
            {
                if (target.name == "remote")
                { target.SetActive(true); }
                else
                { target.SetActive(false); }
            }
        }
        public void SwitchSetting()
        {
            foreach (var target in targetPage)
            {
                if (target.name == "setting")
                { target.SetActive(true); }
                else
                { target.SetActive(false); }
            }
        }
        public void SwitchAdmin()
        {
            foreach (var target in targetPage)
            {
                if (target.name == "admin")
                { target.SetActive(true); }
                else
                { target.SetActive(false); }
            }
        }
    }
}
