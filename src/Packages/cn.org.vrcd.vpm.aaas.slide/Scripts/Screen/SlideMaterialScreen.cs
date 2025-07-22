using UnityEngine;

namespace AAAS.Slide.Screen {
    public sealed class SlideMaterialScreen : SlideScreenBase {
        [SerializeField] private Material material;

        [SerializeField] [Header("Left empty will set texture to mainTexture of the material")]
        private string texturePropertyName = "_MainTex";

        protected override void SetScreenTexture(Texture texture) {
            if (!material) {
                Debug.LogError(
                    "[SlideMaterialScreen] Material is not assigned. Please assign a Material in the inspector.", this);
                return;
            }

            if (!string.IsNullOrWhiteSpace(texturePropertyName)) {
                material.SetTexture(texturePropertyName, texture);
                return;
            }

            material.mainTexture = texture;
        }
    }
}
