using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.VideoSwitch.Output {
    public class RawImageBroadcasterVideoOutput : BroadcasterVideoOutputBase {
        [SerializeField] private RawImage rawImage;

        protected override void UpdateOutputTexture(Texture texture) {
            rawImage.texture = texture;
        }
    }
}
