using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace cdes_presets
{
    public class OffSet : UdonSharpBehaviour
    {
        private int offSet = 0;

        [SerializeField] private Text _offsetText;

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
            _offsetText.text = offSet.ToString();
        }

    }
}
