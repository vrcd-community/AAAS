using UdonSharp;
using UnityEngine;

namespace cdes_presets
{
    public class interactToggle : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] TargetGameObject;
        private int count = 1;

        public override void Interact()
        {

            if(count == 1)
            {
                foreach (var gameObject in TargetGameObject)
                {
                    if (gameObject.activeSelf == true)
                    {
                        gameObject.SetActive(false);
                    }
                    else if (gameObject.activeSelf == false)
                    {
                        gameObject.SetActive(true);
                    }
                }
                count++;
            }
            else
            {
                return;
            }
        }
    }
}