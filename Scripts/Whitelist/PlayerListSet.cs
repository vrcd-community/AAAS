
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace cdes_presets
{
	public class PlayerListSet : UdonSharpBehaviour
	{
		[Header("玩家列表")]
		[SerializeField] private string[] playerList = new string[] { };
		[Header("切换列表中对象的开关状态")]
		[SerializeField] private GameObject[] TargetGameObjects = new GameObject[0];
		private bool masterIsCdse = true;

		public void Start()
		{
			foreach (GameObject gameObject in TargetGameObjects)
				gameObject.SetActive(false);

			for (int i = 0; i < playerList.Length; i++)
			{
				if (Networking.LocalPlayer.displayName == playerList[i])
				{
					foreach (GameObject gameObject in TargetGameObjects)
						gameObject.SetActive(!gameObject.activeSelf);
					break;
				}
			}
		}
	}
}
