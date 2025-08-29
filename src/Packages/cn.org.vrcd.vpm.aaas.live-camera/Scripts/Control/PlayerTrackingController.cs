using System;
using AAAS.LiveCamera.CameraFilters;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace AAAS.LiveCamera.Control {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTrackingController : UdonSharpBehaviour {
        public PlayerTrackingCameraFilter playerTrackingCameraFilter;

        public TMP_Dropdown playerListDropdown;
        public Toggle enableTrackingToggle;
        public Slider headWidthSlider;

        private VRCPlayerApi[] _players = new VRCPlayerApi[0];

        private void Start() {
            if (!Guard()) {
                enabled = false;
                return;
            }
            
            UpdatePlayerList();
            enableTrackingToggle.SetIsOnWithoutNotify(playerTrackingCameraFilter._GetEnableTracking());
            headWidthSlider.SetValueWithoutNotify(playerTrackingCameraFilter._GetHeadWidth());
        }

        private void LateUpdate() {
            if (!Guard())
                return;
            
            var isTrackingEnabled = playerTrackingCameraFilter._GetEnableTracking();
            if (enableTrackingToggle.isOn != isTrackingEnabled) {
                enableTrackingToggle.SetIsOnWithoutNotify(isTrackingEnabled);
            }
            
            var headWidth = playerTrackingCameraFilter._GetHeadWidth();
            if (Math.Abs(headWidthSlider.value - headWidth) > 0.01f) {
                headWidthSlider.SetValueWithoutNotify(headWidth);
            }
            
            var currentPlayerId = playerTrackingCameraFilter._GetTrackingPlayerNetworkId();
            var currentPlayer = VRCPlayerApi.GetPlayerById(currentPlayerId);
            if (!Utilities.IsValid(currentPlayer) || !currentPlayer.IsValid()) {
                return;
            }
            
            for (var i = 0; i < _players.Length; i++) {
                var player = _players[i];
                if (currentPlayer.playerId != player.playerId)
                    continue;
                
                if (playerListDropdown.value != i) {
                    playerListDropdown.SetValueWithoutNotify(i);
                }
                return;
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player) {
            SendCustomEventDelayedFrames(nameof(UpdatePlayerList), 1);
        }

        public override void OnPlayerLeft(VRCPlayerApi player) {
            SendCustomEventDelayedFrames(nameof(UpdatePlayerList), 1);
        }

        public void UpdatePlayerList() {
            if (!Guard())
                return;

            _players = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
            playerListDropdown.ClearOptions();

            var playerNames = new string[_players.Length];
            var playerIds = new int[_players.Length];
            for (var i = 0; i < _players.Length; i++) {
                var player = _players[i];
                playerNames[i] = player.displayName;
                playerIds[i] = VRCPlayerApi.GetPlayerId(player);
            }
            
            playerListDropdown.AddOptions(playerNames);
            
            var currentPlayer = VRCPlayerApi.GetPlayerById(playerTrackingCameraFilter._GetTrackingPlayerNetworkId());
            if (!Utilities.IsValid(currentPlayer) || !currentPlayer.IsValid()) {
                Debug.LogWarning("[PlayerTrackingController] Currently tracked player (id) is not valid. Will set to first player in dropdown", this);
                if (playerIds.Length <= 0) return;
                
                playerTrackingCameraFilter._SetTrackingPlayerNetworkId(playerIds[0]);
                return;
            }
            
            for (var i = 0; i < playerIds.Length; i++) {
                var playerId = playerIds[i];
                if (currentPlayer.playerId != playerId)
                    continue;
                
                playerListDropdown.SetValueWithoutNotify(i);
                return;
            }
        }

        [PublicAPI]
        public void OnDropdownValueChanged() {
            if (!Guard())
                return;

            var index = playerListDropdown.value;
            if (index < 0 || index >= _players.Length) {
                Debug.LogWarning("[PlayerTrackingController] Selected index is out of range.", this);
            }
            
            var selectedPlayer = _players[index];
            var selectedPlayerId = VRCPlayerApi.GetPlayerId(selectedPlayer);
            
            playerTrackingCameraFilter._SetTrackingPlayerNetworkId(selectedPlayerId);
        }
        
        [PublicAPI]
        public void OnToggleValueChanged() {
            if (!Guard())
                return;

            var enableTracking = enableTrackingToggle.isOn;
            playerTrackingCameraFilter._SetEnableTracking(enableTracking);
        }

        [PublicAPI]
        public void OnSliderValueChanged() {
            if (!Guard())
                return;
            
            var headWidth = headWidthSlider.value;
            playerTrackingCameraFilter._SetHeadWidth(headWidth);
        }

        private bool Guard() {
            if (!playerTrackingCameraFilter) {
                Debug.LogError("[PlayerTrackingController] PlayerTrackingCameraFilter reference is missing.", this);
                return false;
            }

            if (!playerListDropdown) {
                Debug.LogError("[PlayerTrackingController] PlayerListDropdown reference is missing.", this);
                return false;
            }
            
            if (!enableTrackingToggle) {
                Debug.LogError("[PlayerTrackingController] EnableTrackingToggle reference is missing.", this);
                return false;
            }
            
            if (!headWidthSlider) {
                Debug.LogError("[PlayerTrackingController] HeadWidthSlider reference is missing.", this);
                return false;
            }
            
            return true;
        }
    }
}