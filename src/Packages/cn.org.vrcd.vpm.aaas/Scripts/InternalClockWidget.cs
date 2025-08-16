using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.TempAssembly {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class InternalClockWidget : UdonSharpBehaviour {
        public string[] formats = { "yyyy/MM/dd", "HH:mm:ss" };
        public TextMeshProUGUI[] texts;

        public Image hourBar;
        public Image minuteBar;
        public Image secondBar;

        public string timeZoneID = "China Standard Time";

        private TimeZoneInfo _timeZoneInfo;

        private void Start() {
            if (string.IsNullOrWhiteSpace(timeZoneID)) {
                Debug.LogWarning("TimeZoneID is not set, using local time zone.");
                _timeZoneInfo = TimeZoneInfo.Local;
                return;
            }

            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);
        }

        private void LateUpdate() {
            var datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);

            for (var i = 0; i < formats.Length && i < texts.Length; i++) {
                if (!texts[i]) continue;

                texts[i].text = datetime.ToString(formats[i]);
            }

            if (hourBar) {
                hourBar.fillAmount = datetime.Hour / 12f;
            }

            if (minuteBar) {
                minuteBar.fillAmount = datetime.Minute / 60f;
            }

            if (secondBar) {
                secondBar.fillAmount = datetime.Second / 60f;
            }
        }
    }
}
