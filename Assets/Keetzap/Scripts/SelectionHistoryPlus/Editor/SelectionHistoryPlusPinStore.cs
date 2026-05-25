//-----------------------------------------------------------------------
// SelectionHistoryPlusPinStore.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    internal sealed class SelectionHistoryPlusPinStore
    {
        private readonly HashSet<string> _pinnedIds = new HashSet<string>();

        public void Load()
        {
            _pinnedIds.Clear();
            var raw = EditorPrefs.GetString(SelectionHistoryPlusPrefs.GetPinnedPrefsKey(), string.Empty);

            if (string.IsNullOrEmpty(raw))
            {
                return;
            }

            foreach (string id in raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                _pinnedIds.Add(id);
            }
        }

        public void Save()
        {
            _pinnedIds.RemoveWhere(id => ResolveIdToObject(id) == null);

            var raw = string.Join(";", _pinnedIds);
            EditorPrefs.SetString(SelectionHistoryPlusPrefs.GetPinnedPrefsKey(), raw);
        }

        public bool IsPinned(Object obj)
        {
            var id = GetObjectId(obj);
            return !string.IsNullOrEmpty(id) && _pinnedIds.Contains(id);
        }

        public void TogglePin(Object obj)
        {
            var id = GetObjectId(obj);

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (_pinnedIds.Contains(id))
            {
                _pinnedIds.Remove(id);
            }
            else
            {
                _pinnedIds.Add(id);
            }

            Save();
        }

        public void Unpin(Object obj)
        {
            var id = GetObjectId(obj);

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (_pinnedIds.Remove(id))
            {
                Save();
            }
        }

        public void Pin(Object obj)
        {
            var id = GetObjectId(obj);

            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (_pinnedIds.Add(id))
            {
                Save();
            }
        }

        internal static string GetObjectId(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var path = AssetDatabase.GetAssetPath(obj);

            if (!string.IsNullOrEmpty(path))
            {
                return "A:" + AssetDatabase.AssetPathToGUID(path);
            }

            var gid = GlobalObjectId.GetGlobalObjectIdSlow(obj);

            return "S:" + gid.ToString();
        }

        internal static Object ResolveIdToObject(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (id.StartsWith("A:"))
            {
                var guid = id.Substring(2);
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                return AssetDatabase.LoadMainAssetAtPath(path);
            }

            if (id.StartsWith("S:"))
            {
                var gidStr = id.Substring(2);

                if (GlobalObjectId.TryParse(gidStr, out GlobalObjectId gid))
                {
                    return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                }
            }

            return null;
        }
    }
}
