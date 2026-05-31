using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    /// <summary>
    /// Automatically snaps prefabs to the nearest 0.5 grid when they are
    /// dragged into the Scene view. Listens to hierarchy changes and
    /// corrects the position of any newly instantiated prefab.
    /// Can be toggled on/off via the Keetzap menu.
    /// </summary>
    [InitializeOnLoad]
    public static class PrefabDropSnapper
    {
        private const string MenuPath = "Keetzap/Auto-Snap Prefabs on Drop";
        private const string PrefKey = "Keetzap_AutoSnapEnabled";
        private const float SnapStep = 0.5f;

        private static bool _isEnabled;

        static PrefabDropSnapper()
        {
            // Defer the menu check‑mark initialization until the editor is ready.
            EditorApplication.delayCall += () =>
            {
                _isEnabled = EditorPrefs.GetBool(PrefKey, true);
                Menu.SetChecked(MenuPath, _isEnabled);
            };

            ObjectChangeEvents.changesPublished += OnChangesPublished;
        }

        [MenuItem(MenuPath, false, 200)]
        private static void ToggleAutoSnap()
        {
            _isEnabled = !_isEnabled;
            EditorPrefs.SetBool(PrefKey, _isEnabled);
            Menu.SetChecked(MenuPath, _isEnabled);

            Debug.Log($"[PrefabDropSnapper] Auto-snap is now {(_isEnabled ? "ON" : "OFF")}.");
        }

        /// <summary>
        /// Rounds a value to the nearest multiple of <see cref="SnapStep"/> (0.5).
        /// Examples: 0.16 → 0.0,  -3.38 → -3.5,  2.74 → 2.5,  2.76 → 3.0
        /// </summary>
        private static float SnapToHalf(float value)
        {
            return Mathf.Round(value / SnapStep) * SnapStep;
        }

        private static void OnChangesPublished(ref ObjectChangeEventStream stream)
        {
            if (!_isEnabled)
                return;

            for (int i = 0; i < stream.length; i++)
            {
                // Detect newly created GameObjects (happens when a prefab is dropped).
                if (stream.GetEventType(i) == ObjectChangeKind.CreateGameObjectHierarchy)
                {
                    stream.GetCreateGameObjectHierarchyEvent(i, out var createEvent);

                    GameObject go = EditorUtility.InstanceIDToObject(createEvent.instanceId) as GameObject;
                    if (go == null)
                        continue;

                    Vector3 pos = go.transform.position;
                    Vector3 snapped = new Vector3(
                        SnapToHalf(pos.x),
                        SnapToHalf(pos.y),
                        SnapToHalf(pos.z)
                    );

                    if (pos != snapped)
                    {
                        Undo.RecordObject(go.transform, "Auto-Snap Prefab Position");
                        go.transform.position = snapped;
                        Debug.Log($"[PrefabDropSnapper] Snapped \"{go.name}\" from {pos} to {snapped}.");
                    }
                }
            }
        }
    }
}
