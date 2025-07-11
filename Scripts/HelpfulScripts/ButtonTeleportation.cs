
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonTeleportation : UdonSharpBehaviour
{

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			var pos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.Head);
			var rot = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.Head);

			Vector3 tgtPos = pos + (rot * new Vector3(0,0,2));
			Quaternion tgtRot = rot * new Quaternion(0, 0.707f, 0, 0.707f);

			transform.rotation = tgtRot;
			transform.position = tgtPos;
		}

	}
}
