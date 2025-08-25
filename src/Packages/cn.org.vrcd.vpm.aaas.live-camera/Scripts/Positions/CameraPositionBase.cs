using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

namespace AAAS.LiveCamera.Positions {
    public abstract class CameraPositionBase : UdonSharpBehaviour {
        [PublicAPI]
        protected UdonSharpBehaviour[] positionChangedEventReceivers = new UdonSharpBehaviour[0];
        [PublicAPI]
        protected string[] positionChangedEventNames = new string[0];
        [PublicAPI]
        protected string[] positionChangedEventNonces = new string[0];
        
        [PublicAPI]
        public virtual Transform GetCameraTransform() {
            return transform;
        }

        [PublicAPI]
        public bool RegisterPositionChangedEvent(UdonSharpBehaviour receiver, string eventName, string nonce = "") {
            if (!receiver) {
                Debug.LogWarning("[CameraPositionBase] RegisterPositionChangedEvent: receiver is null.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventName)) {
                Debug.LogWarning("[CameraPositionBase] RegisterPositionChangedEvent: eventName is null or empty.", this);
                return false;
            }

            if (nonce == null) {
                Debug.LogWarning("[CameraPositionBase] RegisterPositionChangedEvent: nonce is null.", this);
                return false;
            }
            
            positionChangedEventReceivers = AddToArray(positionChangedEventReceivers, receiver);
            positionChangedEventNames = AddToArray(positionChangedEventNames, eventName);
            positionChangedEventNonces = AddToArray(positionChangedEventNonces, nonce);
            
            return true;
        }

        [PublicAPI]
        protected void NotifyPositionChanged() {
            for (var i = 0; i < positionChangedEventReceivers.Length; i++) {
                var receiver = positionChangedEventReceivers[i];
                var eventName = positionChangedEventNames[i];
                var nonce = positionChangedEventNonces[i];
                
                if (string.IsNullOrEmpty(nonce)) {
                    receiver.SendCustomEvent(eventName);
                } else {
                    receiver.SendCustomNetworkEvent(NetworkEventTarget.Self, eventName, nonce);
                }
            }
        }

        private static T[] AddToArray<T>(T[] array, T element) {
            var newArray = new T[array.Length + 1];
            for (var i = 0; i < array.Length; i++) {
                newArray[i] = array[i];
            }
            
            newArray[array.Length] = element;
            return newArray;
        }
    }
}