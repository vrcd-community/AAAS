
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace cdse_presets
{
	public class AnchorsBind : UdonSharpBehaviour
	{
		public TeleportGameObject _obj = null;

		private void OnEnable()
		{
				var obj = GameObject.Find("PPT_Pad").GetComponent<Transform>();
				_obj.targetGameObject = obj;
		}
	}
}
