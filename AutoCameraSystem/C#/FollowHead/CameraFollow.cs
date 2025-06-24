
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class CameraFollow : UdonSharpBehaviour
{

    [SerializeField] private GameObject Target = null; // 锁定被追踪玩家的眼睛位置
    [SerializeField] private GameObject SpinObject = null; // 相机
    
    private VRCPlayerApi[] Players = null; // 房间内所有人的API信息存储

    private VRCPlayerApi PlayerApi = null; // 被追踪玩家的 API
    private bool OnTarget = false; // 是否有玩家在被追踪的状态指示
    
    [UdonSynced]
    [HideInInspector]
    public string PlayerName = ""; // 要追踪的目标名称

    void Start()
    {
        // 创建80位的玩家API数组，判空逻辑（够用）
        Players = new VRCPlayerApi[80];
        if (Players == null)
            return;
    }

    private void Update()
    {
        // 当OnTarget为true，target、摄像机、被追踪玩家的API不为空，且确定有效时
        if (OnTarget)
        {
            if(Target != null && SpinObject != null && PlayerApi != null) 
            {
                if(PlayerApi.IsValid() == true)
                {
                    // 获取被追踪玩家脚底的位置+玩家眼睛距离脚底的高度=玩家眼睛的位置
                    // 将玩家眼睛的位置设置为Target的位置
                    Vector3 tmp = PlayerApi.GetPosition();
                    tmp.y += PlayerApi.GetAvatarEyeHeightAsMeters();
                    Target.transform.position = tmp;
                }
            }
        }
        // 摄像机看向被追踪玩家眼睛的位置
        SpinObject.transform.LookAt(Target.transform);
    }

    // 开始追踪
    bool StartTarget()
    {
        // 当target、摄像机、被追踪玩家的API不为空时
        if (Target != null && SpinObject != null && Players != null)
        {
            // 获取房间内所有玩家的API（数组）
            VRCPlayerApi.GetPlayers(Players);
            // 遍历数组内的玩家名称，当玩家名称与要追踪的目标名称相同时，记录该玩家的 PlayerApi，将 OnTarget 翻转为 true
            for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
            {
                if (Players[i].displayName == PlayerName)
                {
                    PlayerApi = Players[i];
                    OnTarget = true;
                    return true;
                }
            }
        }
        return false;
    }

    // 开始追踪同步
    // 从 MenuTarget，MenuTargetMe 脚本接收事件（要跟踪的玩家名称，也就是 string Name）
    // 也就是说，每当被追踪玩家被修改时，触发这个事件
    public void StartTargetSync(string Name)
    {
        // 设置同步主人
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        // 新建叫做 PlayerNameTemp 的字符串变量，用于备份 PlayerName（要追踪的目标名称）
        string PlayerNameTemp = PlayerName;
        // 把从 MenuTarget，MenuTargetMe 脚本获取的要追踪的目标名称赋值给 PlayerName
        PlayerName = Name;
        // 如果 StartTarget()返回值true，则序列化（发送手动同步数据修改被追踪玩家），否则撤回修改
        if (StartTarget())
        {
            RequestSerialization();
        }
        else
        {
            PlayerName = PlayerNameTemp;
        }
    }

    // 反序列化（接收手动同步数据）
    // 也就是说，每当 StartTarget 返回值为 false，且你不是同步主人（未同步玩家名称）时，同步被追踪玩家名称并执行 StartTarget 追踪
    // 随后，你的 StartTarget 返回值
    public override void OnDeserialization()
    {
        // 当你不是主人的时候，触发 StartTarget()（开始追踪）事件
        if (!Networking.IsOwner(gameObject))
        {
            StartTarget();
        }
    }

}
