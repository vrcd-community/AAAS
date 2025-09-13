using AAAS.Broadcaster.Tools;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

namespace AAAS.Broadcaster.VideoSwitch.Input
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class BroadcasterVideoInputBase : UdonSharpBehaviour {
        [PublicAPI]
        protected UdonSharpBehaviour[] _listeners = new UdonSharpBehaviour[0];
        [PublicAPI]
        protected string[] _eventNames = new string[0];
        [PublicAPI]
        protected string[] _nonce = new string[0];
        [PublicAPI]
        protected bool[] _isNetworkEvent = new bool[0];

        [PublicAPI]
        public virtual Texture GetVideoTexture()
        {
            return null;
        }

        [PublicAPI]
        public virtual bool RegisterVideoTextureChangedListener(UdonSharpBehaviour listeners, string eventName, string nonce)
        {
            if (!listeners) {
                Debug.LogWarning("[BroadcasterVideoInputBase] RegisterVideoTextureChangedReceiver called with null receiver.", this);
                return false;
            }

            if (string.IsNullOrEmpty(eventName)) {
                Debug.LogWarning("[BroadcasterVideoInputBase] RegisterVideoTextureChangedReceiver called with empty event name.", this);
                return false;
            }

            _listeners = ArrayTools.Add(_listeners, listeners);
            _eventNames = ArrayTools.Add(_eventNames, eventName);
            _nonce = ArrayTools.Add(_nonce, nonce);
            _isNetworkEvent = ArrayTools.Add(_isNetworkEvent, true);
            return true;
        }
        
        [PublicAPI]
        public virtual bool RegisterVideoTextureChangedListener(UdonSharpBehaviour listeners, string eventName)
        {
            if (!listeners) {
                Debug.LogWarning("[BroadcasterVideoInputBase] RegisterVideoTextureChangedReceiver called with null receiver.", this);
                return false;
            }

            if (string.IsNullOrEmpty(eventName)) {
                Debug.LogWarning("[BroadcasterVideoInputBase] RegisterVideoTextureChangedReceiver called with empty event name.", this);
                return false;
            }

            _listeners = ArrayTools.Add(_listeners, listeners);
            _eventNames = ArrayTools.Add(_eventNames, eventName);
            _nonce = ArrayTools.Add(_nonce, "");
            _isNetworkEvent = ArrayTools.Add(_isNetworkEvent, false);
            return true;
        }

        [PublicAPI]
        protected virtual void NotifyVideoTextureChanged() {
            for (var index = 0; index < _listeners.Length; index++) {
                var listener = _listeners[index];
                var eventName = _eventNames[index];
                var nonce = _nonce[index];

                if (listener && string.IsNullOrEmpty(eventName))
                    continue;

                if (!_isNetworkEvent[index]) {
                    _listeners[index].SendCustomEvent(eventName);
                    return;
                }
                
                _listeners[index].SendCustomNetworkEvent(NetworkEventTarget.Self, eventName, nonce);
            }
        }
    }
}
