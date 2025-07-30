using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Input {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class SimpleTextureVideoInput : BroadcasterVideoInputBase {
        [SerializeField] private Texture videoTexture;

        public override Texture GetVideoTexture() {
            return videoTexture;
        }
    }
}
