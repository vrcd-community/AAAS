﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace cdes_presets
{
    public class MenuCore : UdonSharpBehaviour
    {
        public int ButtonID = 0;

        public CameraManage CameraManageOBJ = null;

        void Start()
        {

        }

        public void _OnButtonClick()
        {
            if (CameraManageOBJ != null)
            {
                CameraManageOBJ.SendCameraPos(ButtonID);
            }
        }
    }
}