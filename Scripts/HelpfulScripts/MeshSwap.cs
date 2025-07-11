using UdonSharp;
using UnityEngine;

namespace cdes_presets
{
    // 这个脚本的主要功能是根据一个布尔状态（toggleState）来切换显示/隐藏不同的游戏对象。
    public class MeshSwap : UdonSharpBehaviour
    {
        public GameObject[] HideObject;
        public GameObject ShowObject;

        [HideInInspector] public bool toggleState = false;

        public void isTrigger()
        {
            if (HideObject != null && ShowObject != null)
            {
                if (HideObject.Length != 0)
                {
                    if (toggleState)
                    {
                        for (int i = 0; i < HideObject.Length; i++)
                        {
                            HideObject[i].SetActive(false);
                        }
                        ShowObject.SetActive(true);
                        Debug.Log("开关关闭了");
                    }
                    else
                    {
                        for (int i = 0; i < HideObject.Length; i++)
                        {
                            HideObject[i].SetActive(true);
                        }
                        ShowObject.SetActive(false);
                        Debug.Log("开关打开了");
                    }
                }
            }
        }
    }

}