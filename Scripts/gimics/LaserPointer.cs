using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace cdes_presets {
    public class LaserPointer : UdonSharpBehaviour
    {
        [SerializeField] private LineRenderer laserLine;
        [SerializeField] private GameObject laserDot;
        public bool active;
        public LayerMask hitLayers;

        private VRC_Pickup pickup;

        private void Start()
        {
            pickup = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
        }

        public override void OnPickup()
        {
        }

        public override void OnDrop()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "LaserDisable");
        }

        public override void OnPickupUseDown()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "LaserToggle");
        }

        public void LaserToggle()
        {
            active = !active;
            laserLine.gameObject.SetActive(active);
            laserDot.SetActive(active);
        }

        public void LaserDisable()
        {
            active = false;
            laserLine.gameObject.SetActive(active);
            laserDot.SetActive(active);
        }

        private void LateUpdate()
        {
            if (!active) return;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 30f, hitLayers))
            {
                laserLine.gameObject.SetActive(true);
                laserDot.SetActive(true);
                var point = hit.point;
                laserLine.SetPosition(0, transform.position);
                laserLine.SetPosition(1, point);
                laserDot.transform.position = point;
                laserDot.transform.LookAt(point + hit.normal * -2);
            }
            else
            {
                laserLine.gameObject.SetActive(false);
                laserDot.SetActive(false);
            }
        }
    }
}
