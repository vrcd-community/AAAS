
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ToggleManage : UdonSharpBehaviour
{

    public UIButton[] Button = null;                                        //用这个指定UIB

    [SerializeField] public UdonBehaviour Callback = null;                  //指向回调

    [HideInInspector] public UIButton inButton = null;                      //指向触发回调函数

    [UdonSynced] [HideInInspector] public string toggle = null;

    void Start()
    {
        if (toggle != null)
        {
            UIButton objectToggle = this.transform.Find(toggle).GetComponent<UIButton>();
            if (objectToggle != null) 
            {
                objectToggle._SetButtonToggle(true);
            }
        }
    }

    private void Update()
    {

    }

    void sendButtonPush()                                                               // 向BM,ToggleMag 传递事件
    {
        if (Button == null) return;
        if (Callback == null) return;
        if (toggle == null) return;

        Callback.SetProgramVariable("button", toggle);
        Callback.SendCustomEvent("_OnButtonEvent");

    }

    public void _OnButtonPressed()                                                      //回调函数，来自UIB
    {
        if (inButton.toggleState == true)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            toggle = inButton.name;

            for (int i = 0; i < Button.Length; i++)
            {
                if (Button[i].name != toggle)
                {
                    Button[i]._SetButtonToggle(false);
                }
            }

            sendButtonPush();

            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(gameObject))
        {
            UIButton objectToggle = this.transform.Find(toggle).GetComponent<UIButton>();

            if (objectToggle.toggleState == true)
                return;

            for (int i = 0; i < Button.Length; i++)
            {
                if (Button[i].name != toggle)
                {
                    Button[i]._SetButtonToggle(false);
                }
            }

            objectToggle._SetButtonToggle(true);

            sendButtonPush();
        }
    }
}