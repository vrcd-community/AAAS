
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ButtonManage : UdonSharpBehaviour
{
    public UIButton Button = null;                                          //用这个指定UIB
    public int ButtonID = -1;                                               //指定BID

    public UdonBehaviour[] Callback = null;                                 //指向回调
    public bool Inversion = false;                                          //反转

    [HideInInspector] public UIButton inButton = null;                      //这个保留不使用

    [UdonSynced][HideInInspector] public bool isButtonDown = false;         //设置Button是否按下

    void Start()
    {
        
    }

    private void Update()
    {

    }

    void sendButtonPush()                                                               // 向BM,ToggleMag 传递事件
    {
        if (Button == null) return;
        if (Callback == null) return;
        if (Callback.Length == 0) return;
        if (ButtonID == -1) return;

        if (Button.buttonType == 1)
        {
            for (int i = 0; i < Callback.Length; i++)
            {
                if (Inversion)
                {
                    Callback[i].SetProgramVariable("toggleState", !isButtonDown);
                }
                else
                {
                    Callback[i].SetProgramVariable("toggleState", isButtonDown);
                }
            }
        }

        for (int i = 0; i < Callback.Length; i++)
        {
            Callback[i].SetProgramVariable("buttonID", ButtonID);
            Callback[i].SendCustomEvent("_OnButtonEvent");
        }

    }

    public void _OnButtonPressed()                                                      //回调函数，来自UIB
    {
        
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        if (Button.buttonType == 1)
        {
            isButtonDown = Button.toggleState;
            Button._SetButtonToggle(isButtonDown);
        }

        sendButtonPush();

        RequestSerialization();
    }

    private void _SetToggle(bool toggle)                                                //局部设置按钮 弃用
    {
        if (Button == null) return;
        Button._SetButtonToggle(toggle);
    }

    public void _ReSync(bool state)                                                     //全局重新同步
    {
        if (state != isButtonDown)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            isButtonDown = state;

            if (Button.buttonType == 1)
            {
                Button._SetButtonToggle(isButtonDown);
            }

            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(gameObject))
        {
            if (Button.toggleState == isButtonDown && Button.buttonType == 1)
                return;

            if (Button.buttonType == 1)
            {
                Button._SetButtonToggle(isButtonDown);
            }

            sendButtonPush();
        }
    }
}
