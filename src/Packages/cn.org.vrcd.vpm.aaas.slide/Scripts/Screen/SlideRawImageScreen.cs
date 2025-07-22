using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Slide.Screen {
    public sealed class SlideRawImageScreen : SlideScreenBase {
        [SerializeField] private RawImage rawImage;

        protected override void SetScreenTexture(Texture texture) {
            if (!rawImage) {
                Debug.LogError("[SlideRawImageScreen] RawImage is not assigned. Please assign a RawImage in the inspector.", this);
                return;
            }

            rawImage.texture = texture;
        }
    }
}
