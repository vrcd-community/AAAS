using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Input.Slide {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(SlideBroadcasterScreen))]
    public sealed class BroadcasterSlideVideoInput : BroadcasterVideoInputBase {
        [CanBeNull] private Texture OutputTexture;

        public override Texture GetVideoTexture() {
            return OutputTexture;
        }

        internal void NotifySlideOutputTextureChanged(Texture texture) {
            OutputTexture = texture;
            NotifyVideoTextureChanged();
        }

    }
}
