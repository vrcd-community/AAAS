
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace cdes_presets
{
    public class Waiting : UdonSharpBehaviour
    {

        public MeshRenderer Tagget;

        public Texture2D WaitingPNG;
        public RenderTexture CameraTex;

        [HideInInspector] public bool toggleState = false;
        [HideInInspector] public int buttonID = -1;

        void Start()
        {

        }

        public void _OnButtonEvent()
        {
            if (WaitingPNG != null && CameraTex != null && Tagget != null)
            {
                if (toggleState)
                {
                    Tagget.material.mainTexture = WaitingPNG;
                }
                else
                {
                    Tagget.material.mainTexture = CameraTex;
                }
            }
        }
    }
}