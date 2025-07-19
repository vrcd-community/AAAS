using UdonSharp;

namespace cdes_presets
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalTablet : Tablet
    {
        protected override void Start()
        {
            base.Start();
            Lock = false;
        }
    }

}