
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class AudioPlug : UdonSharpBehaviour
{

    public Slider SliderUI = null;
    public Toggle IsPlugOn = null;

    private VRCPlayerApi[] Players = null;
    private float gain = 0;

    private const int MAXRoomPlayers = 80;                                  //房间最大人数
    private const int VoiceDistanceNear = 100;                              //音频衰减
    private const int VoiceDistanceFar = 100;

    [HideInInspector] public bool toggleState = true;
    [HideInInspector] public int buttonID = -1;

    void Start()
    {
        Players = new VRCPlayerApi[MAXRoomPlayers];
        if (Players == null)
            return;
    }

    private void Update()
    {

    }

    public void _OnButtonEvent()
    {
        reflashALL();
    }

    public void _OnSliderEvent()
    {
        if (SliderUI != null)
        {
            gain = SliderUI.value;
        }
    }

    public void _OnToggleEvent()
    {
        reflashALL();
    }

    private void reflashALL()
    {
        if(IsPlugOn != null)
        {
            if (IsPlugOn.isOn == true)
            {
                if (Players != null)
                {
                    VRCPlayerApi.GetPlayers(Players);
                    if (toggleState == true)
                    {
                        for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
                        {
                            if (Players[i].GetPlayerTag("isOnSpeakSpace") == "Y")
                            {
                                Players[i].SetVoiceDistanceNear(VoiceDistanceNear);                                         //设置语音增益
                                Players[i].SetVoiceDistanceFar(VoiceDistanceFar);
                                Players[i].SetVoiceGain(gain);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
                        {
                            if (Players[i].GetPlayerTag("isOnSpeakSpace") == "Y")
                            {
                                Players[i].SetVoiceDistanceNear(0);                                                         //清除语音增益
                                Players[i].SetVoiceDistanceFar(25);
                                Players[i].SetVoiceGain(15);
                            }
                        }
                    }
                }
            }
            else                                                                                                            //插件关闭
            {
                if (Players != null)
                {
                    VRCPlayerApi.GetPlayers(Players);
                    for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
                    {
                        Players[i].SetVoiceDistanceNear(0);                                                                 //清除语音增益
                        Players[i].SetVoiceDistanceFar(25);
                        Players[i].SetVoiceGain(15);
                    }
                }
            }
        }
    }

    void reflash(VRCPlayerApi Player)
    {
        if (IsPlugOn.isOn == true && toggleState == true)
        {
            if (Player != null)
            {
                if (Player.GetPlayerTag("isOnSpeakSpace") == "Y")
                {
                    Player.SetVoiceDistanceNear(VoiceDistanceNear);                                                     //设置语音增益
                    Player.SetVoiceDistanceFar(VoiceDistanceFar);
                    Player.SetVoiceGain(gain);
                }
                else if (Player.GetPlayerTag("isOnSpeakSpace") == "N")
                {
                    Player.SetVoiceDistanceNear(0);                                                                     //清除语音增益
                    Player.SetVoiceDistanceFar(25);
                    Player.SetVoiceGain(15);
                }
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi Player)
    {
        Player.SetPlayerTag("isOnSpeakSpace", "Y");
        reflash(Player);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi Player)
    {
        Player.SetPlayerTag("isOnSpeakSpace", "N");
        reflash(Player);
    }
}
