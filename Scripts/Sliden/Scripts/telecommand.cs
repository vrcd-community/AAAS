
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace Chikuwa.Sliden
{
	public class telecommand : UdonSharpBehaviour
	{
		[SerializeField] private Sliden _sliden = null;

		public override void OnPickupUseDown()
		{
			_sliden.NextPage();
		}
	}

}
