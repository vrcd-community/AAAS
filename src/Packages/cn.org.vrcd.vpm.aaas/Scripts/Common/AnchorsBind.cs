using UdonSharp;
using UnityEngine;

namespace cdes_presets
{
	public class AnchorsBind : UdonSharpBehaviour
	{
		public TeleportGameObject _obj = null;

		private void OnEnable()
		{
				var obj = GameObject.Find("PlayerPad").GetComponent<Transform>();
				_obj.targetGameObject = obj;
		}
	}
}
