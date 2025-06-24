using Chikuwa.Sliden;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
namespace WangQAQ.Plug
{
    public class OffSet : UdonSharpBehaviour
    {
        private int offSet = 0;

        [SerializeField] private Text _textMeshPro;

        private Sliden _sliden;

        public void _Init(Sliden sliden)
        {
            _sliden = sliden;
        }

        public void _AddOffSet()
        {
            offSet++;
            onDataUpdate();
        }

        public void _SubOffSet()
        {
            offSet--;
            onDataUpdate();
        }

        private void onDataUpdate()
        {
            _sliden._offCount = offSet;
            _sliden._offCountUpdate = true;
            _textMeshPro.text = offSet.ToString();
        }

    }
}
