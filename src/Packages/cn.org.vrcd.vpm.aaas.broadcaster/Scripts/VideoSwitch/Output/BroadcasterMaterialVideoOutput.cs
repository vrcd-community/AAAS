using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Output {
    public class BroadcasterMaterialVideoOutput : BroadcasterVideoOutputBase {
        [SerializeField] private Material outputMaterial;

        [SerializeField] [Header("Left empty will set texture to mainTexture of the material")]
        private string texturePropertyName = "_MainTex";

        protected override void UpdateOutputTexture(Texture texture) {
            if (!outputMaterial) {
                Debug.LogError(
                    "[BroadcasterMaterialVideoOutput] Material is not assigned. Please assign a Material in the inspector.", this);
                return;
            }

            if (!string.IsNullOrWhiteSpace(texturePropertyName)) {
                outputMaterial.SetTexture(texturePropertyName, texture);
                return;
            }

            outputMaterial.mainTexture = texture;
        }
    }
}
