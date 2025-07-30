using System;
using AAAS.Broadcaster.AudioControl.Core;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.AudioControl.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterAudioAdaptorList : UdonSharpBehaviour {
        [SerializeField] private BroadcasterAudioControlHub controlHub;

        [SerializeField] private GameObject adaptorItemPrefab;

        private void Start() {
            if (!controlHub) {
                Debug.LogError("[BroadcasterAudioAdaptorList] Control Hub is not assigned.", this);
                enabled = false;
                return;
            }

            if (!adaptorItemPrefab) {
                Debug.LogError("[BroadcasterAudioAdaptorList] Adaptor Item Prefab is not assigned.", this);
                enabled = false;
                return;
            }

            var adaptors = controlHub._GetAudioSourceAdaptors();
            for (var index = 0; index < adaptors.Length; index++) {
                var adaptor = adaptors[index];

                var adaptorItem = Instantiate(adaptorItemPrefab, transform);

                var text = adaptorItem.GetComponentInChildren<Text>();
                text.text = $"Adaptor {index + 1}: {adaptor.name}";

                var itemComponent = adaptorItem.GetComponent<BroadcasterAudioAdaptorItem>();
                itemComponent.audioSourceAdaptor = adaptor;
            }
        }
    }
}
