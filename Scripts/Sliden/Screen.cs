﻿
using UdonSharp;

namespace cdes_presets
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Screen : UdonSharpBehaviour
    {
        public Sliden Sliden;
        void Start()
        {
            if (Sliden) {
                Sliden.AddScreen(gameObject);
            }
        }
    }
}

