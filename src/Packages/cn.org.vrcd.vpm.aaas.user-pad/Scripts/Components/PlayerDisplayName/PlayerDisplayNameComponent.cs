using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AAAS.UserPad.Components.PlayerDisplayName {
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PlayerDisplayNameComponent : UdonSharpBehaviour {
		public TextMeshProUGUI displayNameText;

		private void Start() {
			if (!displayNameText) {
				Debug.LogError("[PlayerDisplayNameComponent] displayNameText is not assigned.", this);
				return;
			}

			var localPlayer = Networking.LocalPlayer;

			displayNameText.text = localPlayer.displayName;
		}
    }
}