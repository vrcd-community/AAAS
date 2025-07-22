using System;
using System.Text;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace AAAS.Slide.Debugging {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SlideDebugOverlay : UdonSharpBehaviour {
        [SerializeField] private Text overlayText;

        [SerializeField] private GameObject targetDebugObject;

        private VRCPlayerApi _localPlayer;

        private bool _isTargetDebugObjectSet;

        private void Start() {
            _isTargetDebugObjectSet = targetDebugObject;

            _localPlayer = Networking.LocalPlayer;

            if (!overlayText) {
                enabled = false;
                Debug.LogError(
                    "[SlideDebugOverlay] Overlay text is not assigned. Please assign a Text component in the inspector.");
            }
        }

        private void Update() {
            overlayText.text = BuildDebugInfo();
        }

        private string BuildDebugInfo() {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Slide Debug Overlay");

            stringBuilder.AppendLine("Time:");

            stringBuilder.AppendFormat("UTC Time: {0:yyyy-MM-dd HH:mm:ss}\n", DateTime.UtcNow);
            stringBuilder.AppendFormat("Local Time: {0:yyyy-MM-dd HH:mm:ss}\n", DateTime.Now);

            stringBuilder.AppendLine("Players:");

            stringBuilder.AppendFormat("Local Player: [{0}] {1}\n", _localPlayer.playerId, _localPlayer.displayName);
            stringBuilder.AppendFormat("Master: [{0}] {1}\n", Networking.Master.playerId, Networking.Master.displayName);

            stringBuilder.AppendLine("Networking:");

            stringBuilder.AppendFormat("IsNetworkSettled: {0}\n", Networking.IsNetworkSettled);
            stringBuilder.AppendFormat("IsClogged: {0}\n", Networking.IsClogged);

            if (!_isTargetDebugObjectSet) {
                stringBuilder.AppendLine("Target Debug Object: None");
                return stringBuilder.ToString();
            }

            stringBuilder.AppendLine("Target Debug Object Info:");

            stringBuilder.AppendFormat("Object Name: {0}\n", targetDebugObject.name);

            stringBuilder.AppendFormat("Time.realTimeSinceStartup - Simulation Time: {0}\n",
                Time.realtimeSinceStartup - Networking.SimulationTime(targetDebugObject));

            var owner = Networking.GetOwner(targetDebugObject);
            stringBuilder.AppendFormat("Owner: [{0}] {1}\n", owner.playerId, owner.displayName);

            stringBuilder.AppendFormat("ReliableEventsInOutboundQueue: {0}\n",
                VRC.SDK3.Network.Stats.ReliableEventsInOutboundQueue(targetDebugObject));

            return stringBuilder.ToString();
        }
    }
}
