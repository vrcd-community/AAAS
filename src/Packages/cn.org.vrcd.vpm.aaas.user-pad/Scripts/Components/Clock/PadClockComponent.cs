using System;
using TMPro;
using UdonSharp;
using UnityEngine;

namespace AAAS.UserPad.Components.Clock {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PadClockComponent : UdonSharpBehaviour {
        public TextMeshProUGUI clockText;

        private void Start() {
            if (!clockText) {
                Debug.LogError("[PadClockComponent] clockText is not assigned.", this);
                enabled = false;
                return;
            }
            
            UpdateTimeText();
        }

        private void Update() {
            UpdateTimeText();
        }

        private void UpdateTimeText() {
            var datetime = DateTime.Now;
            
            clockText.text = datetime.ToString("HH:mm");
        }
    }
}