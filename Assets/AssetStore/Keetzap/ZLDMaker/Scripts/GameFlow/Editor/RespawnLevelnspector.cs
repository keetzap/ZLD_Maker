using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RespawnLevel))]
    public class RespawnLevelnspector : RespawnSystemInspector
    {
        private RespawnLevel respawnLevel;

        protected override void OnEnable()
        {
            base.OnEnable();

            respawnLevel = (RespawnLevel)target;
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("RESPAWNING POINTS", SectionRespawningPoints);

            EndInspector(respawnLevel, "Respawning level asset");
        }
    }
}