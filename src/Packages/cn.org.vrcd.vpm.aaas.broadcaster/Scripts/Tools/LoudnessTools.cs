using JetBrains.Annotations;
using UnityEngine;

namespace AAAS.Broadcaster.Tools {
    public static class LoudnessTools {
        [PublicAPI]
        public static float GetRmsLoudness(float[] audioData, float smoothing = 0.8f, float lastRms = 0f) {
            float sum = 0f;
            foreach (float sample in audioData)
            {
                sum += sample * sample;
            }

            var newRms = Mathf.Sqrt(sum / audioData.Length);

            return Mathf.Lerp(lastRms, newRms, 1 - smoothing);
        }

        [PublicAPI]
        public static int GetWindowSize(int windowMs = 50) {
            return (int)(AudioSettings.outputSampleRate * windowMs / 1000f);
        }
    }
}
