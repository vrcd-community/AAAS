
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using System;

namespace Chikuwa.Sliden
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class SpeakerTablet : Tablet
    {
        //返回页面信息字符串，格式为 当前页+1/总页数+1（如 "1/5"）。
        internal static string GetPageText(uint maxPage, uint page)
        {
            return (page + 1) + "/" + (maxPage + 1);
        }

        //根据 SlidenError 枚举值返回对应的错误消息（如 "Unknown"、"AccessDenied" 等）。
        internal static string GetMessage(SlidenError error)
        {
            switch (error)
            {
                case SlidenError.Unknown:
                    return "Unknown";
                case SlidenError.AccessDenied:
                    return "AccessDenied";
                case SlidenError.RateLimit:
                    return "RateLimit";
                case SlidenError.InvalidURL:
                    return "InvalidURL";
                case SlidenError.Player:
                    return "PlayerError";
                default:
                    return "";
            }
        }

        [SerializeField] private AudioSource _chime;//一个音频源，用于播放提示音
        [SerializeField] private UdonBehaviour _hideButton;//一个按钮，用于切换屏幕的显示/隐藏状态。
        [SerializeField] private VRCUrlInputField _urlInputField;//一个输入框，允许用户输入 URL。
        [SerializeField] private Text _currentUrlText;//显示当前加载的 URL。
        [SerializeField] private Button _resetButton;//重置 URL 的按钮。
        [SerializeField] private Text _pageText;//显示当前页面信息（如 "1/5"）。
        [SerializeField] private Text _messageText;//显示加载状态或错误信息。
        [SerializeField] private Button[] _nextButtons = Array.Empty<Button>();
        [SerializeField] private Button[] _prevButtons = Array.Empty<Button>();
        //控制页面导航的“下一页”和“上一页”按钮数组。

        private bool _initialized;//标志是否完成初始化。

        private bool CanNext { get { return Sliden != null && Sliden.Page < Sliden.MaxPage; } }//检查是否可以跳转到下一页
        private bool CanPrev { get { return Sliden != null && Sliden.Page > 0; } }//检查是否可以跳转到上一页。

        //调用基类的 Start() 方法。
        //设置 Lock = false。
        //调用 InitializeSyncTablet() 初始化平板。
        protected override void Start()
        {
            base.Start();

            Lock = false;

            InitializeSyncTablet();
        }

        //切换 Sliden 的屏幕显示状态（隐藏/显示）。
        public void ToggleHide()
        {
            Sliden.SetScreenHidden(!Sliden.ScreenHidden);
        }

        //将平板的拥有者设置为本地玩家，放下拾取状态（Pickup.Drop），并将平板的位置和旋转设置为目标 Transform 的值。触发同步和通知所有客户端。
        public override void ResetPosition(Transform target)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            Pickup.Drop();
            transform.position = target.position;
            transform.rotation = target.rotation;

            RequestSerialization();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetPositionAll));
        }

        //在所有客户端上放下拾取状态。
        public void ResetPositionAll()
        {
            Pickup.Drop();
        }

        //当前玩家设置为对象拥有者后播放 _chime 音频，并通过网络事件通知所有玩家（SendCustomizationNetworkEvent）。
        public void PlayChime()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _chime.Play();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(OnPlayChime));
        }

        //在所有客户端上播放音频。
        public void OnPlayChime()
        {
            _chime.Play();
        }

        //如果未初始化（_initialized = false），直接返回。
        //否则调用基类的 Update 方法。
        protected override void Update()
        {
            if (!_initialized)
            {
                return;
            }

            base.Update();
        }

        //当新玩家加入时，如果当前客户端是对象的拥有者（Networking.IsOwner），触发状态同步（RequestSerialization）。
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject))
            {
                return;
            }
            RequestSerialization();
        }

        //根据 Sliden.ScreenHidden 的状态，设置 _hideButton 的状态（显示为“开”或“关”）。
        //设置 _initialized = true，表示初始化完成。
        public void InitializeSyncTablet()
        {
            _hideButton.SendCustomEvent(Sliden.ScreenHidden ? "SetOff" : "SetOn");
            _initialized = true;
        }

        //当用户在 _urlInputField 中输入新 URL 时，清空 _currentUrlText，并调用 Sliden.Load 加载新 URL。
        public void OnUrlChanged()
        {
            _currentUrlText.text = "";
            if (Sliden != null)
            {
                Sliden.Load(_urlInputField.GetUrl());
            }
        }

        //重置 _urlInputField 和 _currentUrlText 为默认空值，并调用 Sliden.ResetUrl() 重置 Sliden 的 URL。
        public void ResetUrl()
        {
            _urlInputField.SetUrl(VRCUrl.Empty);
            _currentUrlText.text = "";

            if (Sliden != null)
            {
                Sliden.ResetUrl();
            }
        }

        //当 Sliden 开始加载 URL 时：
        //禁用 _resetButton 和 _pageText。
        //显示 _messageText 为 "Loading..."。
        //隐藏所有导航按钮（_nextButtons 和 _prevButtons）。
        //如果当前 URL 与输入框不匹配，重置输入框。
        public override void OnSlidenLoad(VRCUrl url)
        {
            base.OnSlidenLoad(url);
            _resetButton.interactable = false;
            _pageText.enabled = false;
            _messageText.enabled = true;
            _messageText.text = "Loading...";

            foreach (var button in _nextButtons)
            {
                button.gameObject.SetActive(false);
            }
            foreach (var button in _prevButtons)
            {
                button.gameObject.SetActive(false);
            }
            if (!VRCUrl.Equals(url, _urlInputField.GetUrl()))
            {
                _urlInputField.SetUrl(VRCUrl.Empty);
                var color = _urlInputField.placeholder.color;
                color.a = 0.5f;
                _urlInputField.placeholder.color = color;
                _currentUrlText.text = "";
            }
        }

        //当 Sliden 加载完成时：
        //根据 URL 是否为空，决定是否显示 _pageText 并更新页面信息（通过 GetPageText）。
        //隐藏 _messageText。
        //根据 URL 是否与输入框匹配，决定是否显示 URL。
        //如果有多个页面（maxPage > 0），启用导航按钮并根据 CanNext 和 CanPrev 设置按钮的交互状态。
        public override void OnSlidenReady(VRCUrl url, uint maxPage, uint page)
        {
            base.OnSlidenReady(url, maxPage, page);
            _pageText.enabled = !VRCUrl.Empty.Equals(url);
            _pageText.text = GetPageText(maxPage, page);
            _messageText.enabled = false;

            var showUrl = !VRCUrl.Empty.Equals(url) && Equals(url, _urlInputField.GetUrl());
            _urlInputField.SetUrl(VRCUrl.Empty);
            var color = _urlInputField.placeholder.color;
            color.a = showUrl ? 0 : 0.5f;
            _urlInputField.placeholder.color = color;
            _currentUrlText.text = showUrl ? url.ToString() : "";

            var pageEnabled = maxPage > 0;
            foreach (var button in _nextButtons)
            {
                button.gameObject.SetActive(pageEnabled);
                button.enabled = pageEnabled;
                button.interactable = CanNext;
            }
            foreach (var button in _prevButtons)
            {
                button.gameObject.SetActive(pageEnabled);
                button.enabled = pageEnabled;
                button.interactable = CanPrev;
            }
        }

        //当 Sliden 加载出错时，显示错误信息（通过 GetMessage）并禁用 _pageText。
        public override void OnSlidenError(SlidenError error)
        {
            base.OnSlidenError(error);
            _messageText.enabled = true;
            _messageText.text = GetMessage(error);  
            _pageText.enabled = false;
        }

        //当页面切换时，更新 _pageText 和导航按钮的交互状态。
        public override void OnSlidenNavigatePage(uint page)
        {
            base.OnSlidenNavigatePage(page);
            _pageText.text = GetPageText(Sliden.MaxPage, page);

            foreach (var button in _nextButtons)
            {
                button.interactable = CanNext;
            }
            foreach (var button in _prevButtons)
            {
                button.interactable = CanPrev;
            }
        }

        //当 Sliden 可以加载新内容时，启用 _resetButton。
        public override void OnSlidenCanLoad()
        {
            base.OnSlidenCanLoad();
            _resetButton.interactable = true;
        }

        //当 Sliden 的屏幕隐藏状态改变时，更新 _hideButton 的状态。
        public override void OnSlidenScreenHiddenChanged(bool hidden)
        {
            _hideButton.SendCustomEvent(Sliden.ScreenHidden ? "SetOff" : "SetOn");
        }

        //设置 Lock = false
        public void Unlock()
        {           
            Lock = false;
        }

        //当玩家拾取平板时，调用基类的 OnPickup 并通知所有客户端解锁（Unlock）。
        public override void OnPickup()
        {
            base.OnPickup();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Unlock));
        }
    }
}