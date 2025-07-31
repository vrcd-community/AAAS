using AAAS.Broadcaster.VideoSwitch.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Output {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class BroadcasterVideoOutputBase : UdonSharpBehaviour {
        [SerializeField] [PublicAPI] protected BroadcasterVideoSwitch broadcasterSwitch;

        private void Start() {
            broadcasterSwitch._RegisterOutputTextureChangedListener(this, nameof(_NotifyOutputTextureChanged));

            UpdateOutputTexture(broadcasterSwitch._GetOutputTexture());
        }

        public void _NotifyOutputTextureChanged() {
            UpdateOutputTexture(broadcasterSwitch._GetOutputTexture());
        }

        [PublicAPI]
        protected virtual void UpdateOutputTexture([CanBeNull] Texture texture) {
            Debug.LogWarning("[BroadcasterVideoOutputBase] UpdateOutputTexture not implemented in derived class.",
                this);
        }
    }
}
