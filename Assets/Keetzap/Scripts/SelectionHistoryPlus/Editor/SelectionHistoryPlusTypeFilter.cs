//-----------------------------------------------------------------------
// SelectionHistoryPlusTypeFilter.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    internal static class SelectionHistoryPlusTypeFilter
    {
        public static bool Passes(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == typeof(DefaultAsset))
            {
                return false;
            }

            bool isPersistent = EditorUtility.IsPersistent(obj);

            if (!isPersistent)
            {
                return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.SceneObjects);
            }

            switch (obj)
            {
                case Object _ when PrefabUtility.IsPartOfPrefabAsset(obj):
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Prefabs);

                case SceneAsset:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Scenes);

                case MonoScript:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Scripts);

                case ScriptableObject:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.ScriptableObjects);

                case Material:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Materials);

                case Shader:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Shaders);

                case Texture:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Textures);

                case Sprite:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.Sprites);

                case AnimationClip:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.AnimationClips);

                case AudioClip:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.AudioClips);

                default:
                    return SelectionHistoryPlusPrefs.TypeFilter.HasFlag(SelectionHistoryPlusPrefs.TypeFilterMask.OtherAssets);
            }
        }
    }
}
