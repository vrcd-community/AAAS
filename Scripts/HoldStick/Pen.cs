
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Persistence;
using VRC.SDK3.Components;

namespace VRCholdstickpen
{
    public class Pen : UdonSharpBehaviour
    {
        //是否在使用状态
        [UdonSynced] private bool Isuse;

        //模式
        private int Mode = 0;

        //模式名
        private string[] EventNameArray;

        //模式一部分

        //伸长部分-伸长值
        private float PlayerControlY;

        //伸长部分GameObject
        public GameObject stickObject;

        //伸长部分-伸长值
        private float ScaleStick_Y;

        //顶部GameObject
        public GameObject HeadObject;

        //暂存同步参数
        [UdonSynced] private Vector3 TEMP;


        //模式二部分
        //球体颜色数组
        public Color[] lightColor;

        //球体本身
        public Material Lightmaterlai;

        //球体颜色数值
        [UdonSynced] private int ColorList = 0;

        //模式三部分

        #region Serialized Fields

        [SerializeField]
        [Tooltip("在添加一个新节点之前，笔必须移动的量。增加粗线条，减少平滑线条。")]
        private float minMoveDistance = 0.001f;

        [SerializeField]
        [Tooltip("在绘制之前，必须按住笔的最小时间。如果不绘图，则更改颜色。")]
        private float minHoldTime = 0.2f;

        private float holdTimeStart;

        [SerializeField]
        [Tooltip("每次网络更新将有多少个点被批处理在一起。降低这个数字是为了更频繁地发送更新，提高它是为了等待更多的点，以减少带宽使用。")]
        private int pointsPerUpdate = 10;

        [SerializeField]
        [Tooltip("创建点的Transform")]
        private Transform penTip;

        [SerializeField]
        [Tooltip("Transform包含所有linerenderer作为子元素。")]
        private Transform linesContainer;

        [SerializeField] private Gradient paletteColor;
        [SerializeField][Tooltip("需要更新为线颜色的Mesh")] private Renderer penRenderer;
        [SerializeField][Tooltip("选择的第一颜色")] private int firstColorIndex;

        #endregion

        // 笔的PickUp
        private VRCPickup pickup;
        // 当前正在绘制的线
        private UdonPenLine line;
        // 设置更新循环只在笔关闭时运行
        private bool pickupIsDown;
        // 跟踪笔是否已经移动到足以被视为绘图
        private bool isDrawing;
        // 笔最后更新的位置，用于确定笔是否移动到足以添加新点的位置
        private Vector3 startPosition;

        private MeshRenderer penBody;
        private Collider penCollider;
        private LineRenderer currentLineRenderer;
        private int currentIndex;
        private int nextLineIndex;
        private LineRenderer[] linePool;

        // 颜色用于下一行，更新材质变化
        [UdonSynced, FieldChangeCallback(nameof(ActiveColorIndex))] private int _activeColorIndex;
        public int ActiveColorIndex
        {
            set
            {
                _activeColorIndex = value;
                UpdatePenColor();
            }
            get => _activeColorIndex;
        }

        // 更改可见度
        [UdonSynced, FieldChangeCallback(nameof(IsHeld))] private bool _isHeld;

        public bool IsHeld
        {
            set
            {
                _isHeld = value;
            }
            get => _isHeld;
        }

        //Pool转GameObject[]
        public GameObject[] Lines;


        //开始：初始化数组
        void Start()
        {
            EventNameArray = new string[4] { "RestartScale", "LightsphereChange","DrawStart","DrawReset"};//RestartScale=模式1：伸长 LightsphereChange=模式二:球体颜色变化 DrawStart=画笔 模式四=清空画笔
            Lightmaterlai.SetColor("_Color", lightColor[ColorList]);
            StartFORDraw();
        }

        //当玩家拿起并扣扳机时，切换状态并激活对应系统
        public override void OnPickupUseDown()
        {
            Isuse = true;
            this.SendCustomNetworkEvent(NetworkEventTarget.All, EventNameArray[Mode]);
        }

        //当玩家松开扳机时，切换状态
        public override void OnPickupUseUp()
        {
            Isuse = false;
            this.SendCustomNetworkEvent(NetworkEventTarget.All, "OnPickupUseUpDraw");
        }

        //输出玩家视角垂直上的变量
        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            PlayerControlY = value;
        }

        //模式切换请求
        public void ModChanger()
        {
            Mode += 1;
            if (Mode == EventNameArray.Length) 
            {
                Mode = 0;
            }
        }


        //模式1：伸长
        public void RestartScaleNetWork()
        {
            stickObject.transform.localScale = TEMP;
            if (stickObject.transform.localScale.y < 0.0235f)
            {
                ScaleStick_Y = 0.0235f;
            }
            ScaleStick_Y = PlayerControlY / 40 + stickObject.transform.localScale.y;
            stickObject.transform.localPosition = new Vector3(0, ScaleStick_Y, 0);
            HeadObject.transform.localPosition = new Vector3(0, ScaleStick_Y * 2, 0);
        }

        public void RestartScale()
        {
            //UnityEngine.Debug.Log("JumpSuccessful");
            if (Isuse == true)
            {
                if (Networking.IsOwner(gameObject))
                {
                    ScaleStick_Y = PlayerControlY / 40 + stickObject.transform.localScale.y;
                    UnityEngine.Debug.Log(ScaleStick_Y);
                    if (ScaleStick_Y < 0.0235f)
                    {
                        ScaleStick_Y = 0.0235f;
                    }
                    TEMP = new Vector3(stickObject.transform.localScale.x, ScaleStick_Y, stickObject.transform.localScale.z);
                    this.SendCustomEventDelayedFrames("RestartScale", 1, EventTiming.Update);
                    this.SendCustomNetworkEvent(NetworkEventTarget.All, "RestartScaleNetWork");
                }
                
            }
        }
        //模式2：改变顶端颜色
        public void LightsphereChange()
        {
            if (ColorList + 1 == lightColor.Length)
            {
                ColorList = 0;
            }
            else
            {
                ColorList += 1;
            }
            Lightmaterlai.SetColor("_Color", lightColor[ColorList]);
        }
        //模式3：画笔

        private void StartFORDraw()
        {
            // PickUp
            pickup = GetComponent<VRCPickup>();
            if (Networking.LocalPlayer.IsUserInVR())
            {
                pickup.orientation = VRC_Pickup.PickupOrientation.Any;
            }

            // 颜色指示器
            penBody = GetComponent<MeshRenderer>();

            // 笔碰撞
            penCollider = GetComponent<Collider>();

            // 构建对象池
            linePool = linesContainer.GetComponentsInChildren<LineRenderer>();

            // 设置颜色为第一颜色
            ActiveColorIndex = firstColorIndex;

            //this.GetType().GetProperty("linePool").GetValue(this, Lines); 

            UpdatePenColor();

        }

        public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
        {
            if (!Networking.IsOwner(gameObject)) return;

            if (PlayerData.HasKey(Networking.LocalPlayer, "nextLineIndex"))
            {
                nextLineIndex = PlayerData.GetInt(Networking.LocalPlayer, "nextLineIndex");
            }
        }

        public override void OnPickup()
        {
            IsHeld = true;
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            RequestSerialization();
        }

        public override void OnDrop()
        {
            IsHeld = false;
            RequestSerialization();
        }

        public void DrawStart()
        {
            if (Networking.IsOwner(gameObject)) 
            {
                // 重置变量
                holdTimeStart = Time.time;
                pickupIsDown = true;
                isDrawing = false;
                startPosition = penTip.position;
                currentIndex = 0;
            }
            // 重置变量
            
        }

        private void InitNextLine()
        {
            // 从对象池中换条新线
            Debug.Log($"InitNextLine: nextLineIndex: {nextLineIndex}");
            currentLineRenderer = linePool[nextLineIndex];
            currentLineRenderer.positionCount = 2;
            line = currentLineRenderer.GetComponent<UdonPenLine>();

            // 移动线以抵消偏移量
            currentLineRenderer.transform.localPosition = Vector3.zero - currentLineRenderer.transform.parent.TransformPoint(Vector3.zero);

            // 将线路所有权设置为画者
            Networking.SetOwner(Networking.LocalPlayer, currentLineRenderer.gameObject);
            currentLineRenderer.gameObject.SetActive(true);

            // 在笔尖初始化两条线
            for (int i = 0; i < 2; i++)
            {
                currentLineRenderer.SetPosition(i, penTip.position);
            }

            // 设置颜色
            line.SyncedColor = GetActiveColor();

            // 切换下一条
            nextLineIndex = (nextLineIndex + 1) % linePool.Length;
            PlayerData.SetInt("nextLineIndex", nextLineIndex);
        }

        private void Update()
        {
            // 使用检测
            if (!pickupIsDown) return;

            // 移动检测
            if (Vector3.Distance(penTip.position, startPosition) > minMoveDistance)
            {
                // 停止绘制时-初始化下一行
                if (!isDrawing)
                {
                    InitNextLine();
                    isDrawing = true;
                }
                currentLineRenderer.positionCount = currentIndex + 1;
                startPosition = penTip.position;
                currentLineRenderer.SetPosition(currentIndex, startPosition);
                currentIndex++;
                // 组点更新
                if (currentIndex % pointsPerUpdate == 0)
                {
                    line.OnUpdate();
                }
            }
        }

        private Color GetActiveColor()
        {
            return paletteColor.Evaluate(_activeColorIndex / ((float)paletteColor.colorKeys.Length - 1));
        }

        private void UpdatePenColor()
        {
            penRenderer.material.color = GetActiveColor();
        }

        public void OnPickupUseUpDraw()
        {
            // 重置状态
            pickupIsDown = false;

            // 结束画线
            if (isDrawing)
            {
                line.OnFinish();
                isDrawing = false;
            }

            // 快速点击切换颜色
            if (Time.time - holdTimeStart < minHoldTime && !isDrawing)
            {
                ActiveColorIndex = (ActiveColorIndex + 1) % paletteColor.colorKeys.Length;
                UpdatePenColor();
            }
        }
        public void DrawReset()
        {
            for (int i = 0; i < Lines.Length; i=i+1)
            {
                Networking.SetOwner(Networking.LocalPlayer, Lines[i] );
                var targetPenLine = Lines[i].GetComponent<UdonPenLine>();
                if (Utilities.IsValid(targetPenLine))
                {
                    targetPenLine.Erase();

                    InputManager.EnableObjectHighlight(Lines[i], false);
                }
            }
        }
    }
}