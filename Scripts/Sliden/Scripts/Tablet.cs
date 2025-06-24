
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using System;

namespace Chikuwa.Sliden
{

    public class Tablet : SlidenListener
    {
        public readonly float HandIntaractableDistance = 0.05f;//定义了交互距离。VR 用户的手部需要离平板小于 0.05 米才能交互。
        public readonly float BodyIntaractableDistance = 0.6f;//桌面用户的身体中心需要离平板小于 0.6 米才能交互。

        //对主幻灯片系统 Sliden 组件的引用。
        public Sliden Sliden;

        //私有变量，用来存储平板的各种状态，并在状态改变时触发 _needUpdate。
        private bool _needUpdate;
        private bool _lastLock;
        private bool _lastClosing;
        private bool _lastFacing;
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private TabletPlaceholder[] _placeholders = Array.Empty<TabletPlaceholder>();
        private Collider _collider;
        private Button _reloadButton;

        internal bool Lock
        {
            set { _needUpdate |= _lastLock != value; _lastLock = value; }
            get { return _lastLock; }
        }
        internal bool Closing { set { _needUpdate |= _lastClosing != value; _lastClosing = value; } get { return _lastClosing; } }
        internal bool Facing { set { _needUpdate |= _lastFacing != value; _lastFacing = value; } get { return _lastFacing; } }

        //缓存的组件引用，用于提高性能，避免在 Update 中反复调用 GetComponent。
        protected VRC_Pickup Pickup { get; private set; }
        protected Canvas Canvas { get; private set; }
        protected UdonBehaviour LockButton { get; private set; }

        //获取并缓存所有必要的组件，如 VRC_Pickup（使物体可拾取）、Canvas（UI界面）等。
        //它通过遍历子对象中的所有按钮，根据按钮名称的后缀（"SLock", "SReload"）来识别并获取锁定按钮和重新加载按钮的引用。
        //平板把自己注册为 Sliden 系统的一个监听器。这样，当 Sliden 系统发生事件时（如幻灯片加载完成），它就能通知这个平板。
        protected virtual void Start()
        {
            Pickup = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
            Canvas = (Canvas)GetComponentInChildren(typeof(Canvas));

            foreach (var button in (Button[])GetComponentsInChildren(typeof(Button), true))
            {
                if (LockButton == null && button.name.EndsWith("SLock"))
                {
                    LockButton = (UdonBehaviour)button.GetComponent(typeof(UdonBehaviour));
                }
                else if (button.name.EndsWith("SReload"))
                {
                    _reloadButton = button;
                }
            }

            _collider = (Collider)GetComponent(typeof(Collider));

            if (Sliden != null)
            {
                Sliden.AddListener(this);
            }

            _needUpdate = true;
        }

        protected virtual void Update()
        {

            if (Sliden == null || Pickup == null || Canvas == null)
            {
                return;
            }

            //检查平板的位置或旋转是否发生了变化。
            if (!Vector3.Equals(_lastPosition, transform.position) || !Quaternion.Equals(_lastRotation, transform.rotation))
            {
                //如果当前玩家是这个平板的“所有者”，代码会强制调整平板的旋转。
                //这段 Quaternion 计算的目的是让平板始终保持竖直。
                if (Networking.IsOwner(gameObject))
                {
                    var right = transform.position + (transform.rotation * Vector3.right);
                    right.y = transform.position.y;
                    var direction = right - transform.position;
                    transform.rotation = Quaternion.LookRotation(direction, transform.rotation * Vector3.up)
                        * Quaternion.FromToRotation(Vector3.right, Vector3.forward);
                }
                //更新平板与所有“底座” (_placeholders) 之间的距离。
                foreach (var placeholder in _placeholders)
                {
                    placeholder.LastTabletDistance = Vector3.Distance(placeholder.transform.position, transform.position);
                    placeholder.UpdateIfNeeded();
                }

                _lastPosition = transform.position;
                _lastRotation = transform.rotation;
            }

            //获取本地玩家 Networking.LocalPlayer 的信息。
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null)
            {
                Closing = true;
                Facing = true;
                UpdateIfNeeded();
                return;
            }

            //获取玩家的头部和手部位置。
            var playerHeadPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            var playerLeftHandPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            var playerRightHandPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            //判断是应该使用手部位置（VR模式）还是身体中心位置（桌面模式，或者VR玩家手部追踪丢失时）。
            var useBodyPosition = !player.IsUserInVR() || (playerLeftHandPosition == Vector3.zero && playerRightHandPosition == Vector3.zero);

            //计算玩家是否正对平板的屏幕。通过计算平板的朝向向量和“从平板到玩家头部”的向量的点积
            //如果结果为负，说明两个向量方向大致相反，即玩家在平板的前方。
            Facing = Vector3.Dot(transform.forward, (playerHeadPosition - transform.position).normalized) < 0;

            Vector3 bodyPosition = player.GetPosition();
            bodyPosition.y = transform.position.y;
            float bodyDistance = Vector3.Distance(bodyPosition, transform.position);
            if (!useBodyPosition && bodyDistance > BodyIntaractableDistance)
            {
                Closing = false;
                UpdateIfNeeded();
                return;
            }

            if (useBodyPosition)
            {
                foreach (var placeholder in _placeholders)
                {
                    bodyPosition.y = placeholder.transform.position.y;
                    placeholder.LastBodyDistance = Vector3.Distance(placeholder.transform.position, bodyPosition);
                    placeholder.UpdateIfNeeded();
                }
                Closing = true;
                UpdateIfNeeded();
                return;
            }

            Closing = Vector3.Distance(_collider.ClosestPoint(playerLeftHandPosition), playerLeftHandPosition) <= HandIntaractableDistance
                || Vector3.Distance(_collider.ClosestPoint(playerRightHandPosition), playerRightHandPosition) <= HandIntaractableDistance;

            //在计算完所有状态后，调用此方法来应用这些变化。
            UpdateIfNeeded();
        }

        private void UpdateIfNeeded()
        {
            if (!_needUpdate)
            {
                return;
            }
            _needUpdate = false;

            Pickup.pickupable = (Networking.IsOwner(gameObject) || _lastFacing) && _lastClosing && !_lastLock;
            //只有当玩家正对平板时，UI 才可见。
            Canvas.enabled = _lastFacing;
            //因为scaler组件总是莫名其妙变成1，所以干脆在这里加上一个只要能看见面板就设置一次的代码
            Canvas.transform.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 4;
            Canvas.transform.GetComponent<CanvasScaler>().enabled = false;
            Canvas.transform.GetComponent<CanvasScaler>().enabled = true;

            //向锁定按钮的 Udon 脚本发送自定义事件（"SetOn" 或 "SetOff"），让按钮的视觉效果（比如图标或颜色）与锁定状态同步。
            if (LockButton != null)
            {
                LockButton.SendCustomEvent(Lock ? "SetOn" : "SetOff");
            }

            //更新所有“底座” (_placeholders) 的状态。
            foreach (var placeholder in _placeholders)
            {
                placeholder.LastTabletLock = Lock;
                placeholder.UpdateIfNeeded();
            }
        }

        //将平板重置到指定位置，常用于将其“吸附”回底座。
        public virtual void ResetPosition(Transform target)
        {
            Pickup.Drop();
            transform.position = target.position;
            transform.rotation = target.rotation;

            foreach (var placeholder in _placeholders)
            {
                placeholder.LastTabletDistance = Vector3.Distance(placeholder.transform.position, transform.position);
                placeholder.UpdateIfNeeded();
            }
        }

        //切换锁定状态。通常由锁定按钮调用。
        public virtual void ToggleLock()
        {
            Pickup.Drop();
            Lock = !Lock;
        }

        public void ReloadLocal()
        {
            if (Sliden != null)
            {
                Sliden.ReloadLocal();
            }
        }

        public void NextPage()
        {
            if (Sliden != null)
            {
                Sliden.NextPage();
            }
        }

        public void PrevPage()
        {
            if (Sliden != null)
            {
                Sliden.PrevPage();
            }
        }

        //允许底座脚本在启动时将自己注册到平板上。
        public virtual void AddPlaceholder(TabletPlaceholder placeholder)
        {
            _placeholders = (TabletPlaceholder[])ArrayUtils.Append(_placeholders, placeholder);

            placeholder.LastTabletLock = Lock;
            placeholder.LastTabletDistance = Vector3.Distance(placeholder.transform.position, transform.position);
            placeholder.LastBodyDistance =
                (Networking.LocalPlayer != null && !Networking.LocalPlayer.IsUserInVR())
                ? Vector3.Distance(placeholder.transform.position, Networking.LocalPlayer.GetPosition())
                : 0;
            placeholder.UpdateIfNeeded();
        }

        //当幻灯片开始加载时调用，这里它将“重新加载”按钮设为不可交互，防止用户重复点击。
        public override void OnSlidenLoad(VRCUrl url)
        {
            _reloadButton.interactable = false;
        }

        public override void OnSlidenReady(VRCUrl url, uint maxPage, uint page)
        {
            /* NOP */
        }

        public override void OnSlidenError(SlidenError error)
        {
            /* NOP */
        }

        public override void OnSlidenNavigatePage(uint page)
        {
            /* NOP */
        }

        //当幻灯片加载完成或失败，可以再次加载时调用，它将“重新加载”按钮恢复为可交互。
        public override void OnSlidenCanLoad()
        {
            _reloadButton.interactable = true;
        }
    }
}