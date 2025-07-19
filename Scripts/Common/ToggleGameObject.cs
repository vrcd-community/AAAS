using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace cdes_presets
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToggleGameObject : UdonSharpBehaviour
    {
        [Header("这是一个对象开关脚本")]
        [Header("是否同步？")]
        [SerializeField] private bool isGlobal = false;
        [Header("目标对象(开启)")]
        [SerializeField] private MeshRenderer[] targetMesh;
		[UdonSynced] private bool[] isEnabled_targetMesh;
		[SerializeField] private GameObject[] targetGameObject;
		[UdonSynced] private bool[] isEnabled;

		public void Start()
		{
			isEnabled = new bool[targetGameObject.Length];
			isEnabled_targetMesh = new bool[targetMesh.Length];


			for (int i = 0; i < targetGameObject.Length; i++)
            {
                isEnabled[i] = targetGameObject[i].activeSelf;
            }

			for (int i = 0; i < targetMesh.Length; i++)
			{
				isEnabled_targetMesh[i] = targetMesh[i].enabled;
			}
		}

		public void isTrigger()
        {
            if (!isGlobal)
            {
                ToggleLocal();
            }
            else
            {
                if (!Networking.IsOwner(gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                }
                for (int num = 0; num < targetGameObject.Length; num++)
                {
                    if (targetGameObject[num] != null)
                    {
                        isEnabled[num] = !isEnabled[num];
                    }
                }

				for (int num = 0; num < targetMesh.Length; num++)
				{
					if (targetMesh[num] != null)
					{
						isEnabled_targetMesh[num] = !isEnabled_targetMesh[num];
					}
				}
				ToggleGlobal();
                RequestSerialization();
            }
        }

        public override void OnDeserialization()
        {
            if (isGlobal && !Networking.IsOwner(gameObject))
            {
                ToggleGlobal();
            }
        }

        public void ToggleGlobal()
        {
            for (int num = 0; num < targetGameObject.Length; num++)
            {
                if (targetGameObject[num] != null)
                {
                    targetGameObject[num].SetActive(isEnabled[num]);
                }
            }

			for (int num = 0; num < targetMesh.Length; num++)
			{
				if (targetMesh[num] != null)
				{
					targetMesh[num].enabled = isEnabled_targetMesh[num];
				}
			}
		}

        public void ToggleLocal()
        {
            for (int num = 0; num < targetGameObject.Length; num++)
            {
                if (targetGameObject[num] != null)
                {
                    targetGameObject[num].SetActive(!targetGameObject[num].activeSelf);
                }
            }

			for (int num = 0; num < targetMesh.Length; num++)
			{
				if (targetMesh[num] != null)
				{
					targetMesh[num].enabled = !targetMesh[num].enabled;
				}
			}
		}
    }
}