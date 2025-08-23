using System;
using AAAS.Broadcaster.Tools;
using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace AAAS.Broadcaster.VideoSwitch.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public sealed class BroadcasterVideoSwitch : UdonSharpBehaviour {
        [SerializeField] private BroadcasterVideoInputBase[] videoInputs = new BroadcasterVideoInputBase[0];

        [SerializeField] [CanBeNull] private BroadcasterVideoSwitch parentSwitch;

        [CanBeNull] private Texture _currentOutputTexture;

        private UdonSharpBehaviour[] _listeners = new UdonSharpBehaviour[0];
        private string[] _eventNames = new string[0];

    #region Network Sync Properties

        [UdonSynced] [FieldChangeCallback(nameof(CurrentInputIndex))]
        private int _currentInputIndex;

        public int CurrentInputIndex {
            get => _currentInputIndex;
            private set {
                _currentInputIndex = value;
                _OnVideoInputIndexChanged();
            }
        }

    #endregion

        private void Start() {
            if (videoInputs.Length == 0 && (!parentSwitch || parentSwitch._GetVideoInputs().Length == 0)) {
                Debug.LogError("[BroadcasterVideoSwitch] No video inputs registered. VIDEO SWITCH WILL SHUTDOWN NOW.",
                    this);
                Shutdown();
                return;
            }

            if (parentSwitch) {
                Debug.Log("[BroadcasterVideoSwitch] Inheriting video inputs from parent switch.", this);
                videoInputs = ArrayTools.AddRange(videoInputs, parentSwitch._GetVideoInputs());
            }

            for (var index = 0; index < videoInputs.Length; index++) {
                var input = videoInputs[index];
                input.RegisterVideoTextureChangedListener(this, nameof(_OnVideoInputTextureChanged), index.ToString());
            }

            SwitchVideoInputInternal(0);
        }

        [PublicAPI]
        public BroadcasterVideoInputBase[] _GetVideoInputs() {
            var newVideoInputs = new BroadcasterVideoInputBase[videoInputs.Length];
            Array.Copy(videoInputs, newVideoInputs, videoInputs.Length);
            return newVideoInputs;
        }

        [PublicAPI]
        public Texture _GetOutputTexture() {
            if (CurrentInputIndex < 0 || CurrentInputIndex >= videoInputs.Length) {
                Debug.LogError(
                    $"Failed to get output texture, current input index {CurrentInputIndex} is out of bounds for video inputs array. DO NO REMOVE INPUTS DURING RUNTIME.",
                    this);
                Shutdown();
                return null;
            }

            if (!_currentOutputTexture)
                SwitchVideoInputInternal(CurrentInputIndex);

            return _currentOutputTexture;
        }

        [PublicAPI]
        public bool _SwitchVideoInput(int index) {
            return SwitchVideoInputInternal(index);
        }

        private bool SwitchVideoInputInternal(int index) {
            if (videoInputs.Length == 0) {
                Debug.LogError(
                    "[BroadcasterVideoSwitch] No video inputs registered. DO NO REMOVE INPUTS DURING RUNTIME.",
                    this);
                Shutdown();
                return false;
            }

            if (CurrentInputIndex < 0 || CurrentInputIndex >= videoInputs.Length) {
                Debug.LogError(
                    $"[BroadcasterVideoSwitch] Current input index {CurrentInputIndex} is out of bounds for video inputs array. DO NO REMOVE INPUTS DURING RUNTIME.",
                    this);

                Shutdown();
                return false;
            }

            if (index < 0 || index >= videoInputs.Length) {
                Debug.LogError(
                    $"[BroadcasterVideoSwitch] Invalid input index {index}. Must be between 0 and {videoInputs.Length - 1}.",
                    this);
                return false;
            }

            SetVideoInputIndexWithoutNotify(index);
            _currentOutputTexture = videoInputs[CurrentInputIndex].GetVideoTexture();

            if (!_currentOutputTexture) {
                Debug.LogError(
                    $"[BroadcasterVideoSwitch] Failed to get video texture for input index {CurrentInputIndex}. The texture is null.",
                    this);
                return false;
            }

            NotifyVideoTextureChanged();

            return true;
        }

        [PublicAPI]
        public bool _RegisterOutputTextureChangedListener(UdonSharpBehaviour listener, string eventName) {
            if (!listener) {
                Debug.LogWarning(
                    "[BroadcasterVideoSwitch] RegisterVideoTextureChangedReceiver called with null receiver.", this);
                return false;
            }

            if (string.IsNullOrEmpty(eventName)) {
                Debug.LogWarning(
                    "[BroadcasterVideoSwitch] RegisterVideoTextureChangedReceiver called with empty event name.", this);
                return false;
            }

            _listeners = ArrayTools.Add(_listeners, listener);
            _eventNames = ArrayTools.Add(_eventNames, eventName);
            return true;
        }

        private void NotifyVideoTextureChanged() {
            for (var index = 0; index < _listeners.Length; index++) {
                var listener = _listeners[index];
                var eventName = _eventNames[index];
                if (!listener || string.IsNullOrEmpty(eventName)) {
                    Debug.LogWarning(
                        $"[BroadcasterVideoSwitch] Listener at index {index} is null or has an empty event name. Skipping notification.",
                        this);
                    continue;
                }

                listener.SendCustomEvent(eventName);
            }
        }

        private void Shutdown() {
            Debug.LogWarning(
                "[BroadcasterVideoSwitch] Shutting down video switch due to no video inputs registered or other errors. See console for more details.",
                this);
            enabled = false;
        }

    #region Networking

        private void _OnVideoInputIndexChanged() {
            if (CurrentInputIndex < 0 || CurrentInputIndex >= videoInputs.Length) {
                Debug.LogError(
                    $"Invalid video input index: {CurrentInputIndex}. Must be between 0 and {videoInputs.Length - 1}.",
                    this);
                return;
            }

            SwitchVideoInputInternal(CurrentInputIndex);
        }

        private void SetVideoInputIndexWithoutNotify(int index) {
            TakeOwnership();
            _currentInputIndex = index;
            RequestSerialization();
        }

        private void TakeOwnership() {
            if (Networking.GetOwner(gameObject).isLocal)
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

    #endregion

    #region Video Input Event Hadnle

        // "a network event which should only be called by the local player"
        [NetworkCallable(
            maxEventsPerSecond:
            1)] // won't affect local "network" event call, see https://creators.vrchat.com/worlds/udon/networking/events#rate-limiting
        public void _OnVideoInputTextureChanged(string nonce) {
            if (NetworkCalling.CallingPlayer == null) {
                Debug.LogWarning(
                    "[BroadcasterVideoSwitch] Video Input Texture Changed received with no calling player, ignoring.",
                    this);
                return;
            }

            if (!NetworkCalling.CallingPlayer.isLocal) {
                Debug.LogWarning(
                    "[BroadcasterVideoSwitch] Video Input Texture Changed received from non-local player, ignoring.",
                    this);
                return;
            }

            if (!int.TryParse(nonce, out var inputIndex)) {
                Debug.LogError($"[BroadcasterVideoSwitch] Invalid nonce received: {nonce}. Expected an integer index.",
                    this);
                return;
            }

            if (inputIndex < 0 || inputIndex >= videoInputs.Length) {
                Debug.LogError(
                    $"[BroadcasterVideoSwitch] Received a OnVideoInputTextureChanged event with input index {inputIndex} is out of bounds for video inputs array. Must be between 0 and {videoInputs.Length - 1}.",
                    this);
                return;
            }

            // Force update if the input index is out of bounds
            if (videoInputs.Length == 0 || CurrentInputIndex < 0 || CurrentInputIndex >= videoInputs.Length) {
                Debug.LogError("[BroadcasterVideoSwitch] Current input index is out of bounds. Forcing update to run error handling logic.", this);
                SwitchVideoInputInternal(CurrentInputIndex);
                return;
            }

            if (inputIndex != CurrentInputIndex)
                return;

            SwitchVideoInputInternal(CurrentInputIndex);
        }

    #endregion
    }
}
