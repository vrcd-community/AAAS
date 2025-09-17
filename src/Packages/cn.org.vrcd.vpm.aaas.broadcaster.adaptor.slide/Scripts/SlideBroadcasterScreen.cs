using AAAS.Slide.Screen;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Input.Slide {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(BroadcasterSlideVideoInput))]
    public sealed class SlideBroadcasterScreen : SlideScreenBase {
        [SerializeField] private BroadcasterSlideVideoInput videoInput;

        protected override void SetScreenTexture(Texture texture) {
            videoInput.NotifySlideOutputTextureChanged(texture);
        }
    }
}
