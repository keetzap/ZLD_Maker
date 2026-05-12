using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class RenamingTool : BaseEditorWindow
    {
        public const string Message =
            "This tool renames a batch of assets from the project window regardless of its kind.\n\n" +
            "1) When the prefix amount is equal to 0, the tool just add a prefix with a separator:\n" +
            "\tNew prefix = ANI\t\tPrefix amount = 0\n" +
            "\tNameOfAsset\t->\t[New Prefix][Separator]NameOfAsset\n" +
            "\tGoldMedal\t->\tMSH_GoldMedal\n\n" +
            "  You can include a separator within the new prefix:\n" +
            "\tNew prefix = MSH_CHR\t\tPrefix amount = 0\n" +
            "\tBerserker\t->\tMSH_CHR_Berserker\n\n" +
            "2) When the prefix amount is more than 0, the tool replaces as many prefixes as the prefix amount:\n" +
            "\tNew prefix = ANI\t\tPrefix amount = 1\n" +
            "\tMSH_CHR_Berserker_Idle\t->\tANI_CHR_Berserker_Idle\n\n" +
            "\tNew prefix = TEX_ENV\t\tPrefix amount = 2\n" +
            "\tPRF_BCK_Backgroung_Night\t->\tTEX_ENV_Backgroung_Night\n\n" +
            "3) The suffix utility works the same but starting from the end.\n";

        private string _separator = "_";
        private string _newPrefix;
        private int _prefixAmount;
        private string _newSuffix;
        private int _suffixAmount;

        [MenuItem("Keetzap/Batch Renamer")]
        static void Init()
        {
            SetMargings(5, 5, 10, 5);

            RenamingTool window = (RenamingTool)EditorWindow.GetWindow(typeof(RenamingTool));
            window.maxSize = new Vector2(300 + margings.y + margings.w, 235 + margings.x + margings.z);
            window.minSize = new Vector2(220 + margings.y + margings.w, 235 + margings.x + margings.z);
            window.titleContent.text = "Batch renamer";
            window.Show();
        }

        protected sealed override void MainSection()
        {
            EditorGUILayout.LabelField("PREFIX", EditorStyles.boldLabel);
            _newPrefix = EditorGUILayout.TextField("New prefix", _newPrefix);
            _prefixAmount = EditorGUILayout.IntField("Prefix amount", _prefixAmount);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("SUFFIX", EditorStyles.boldLabel);
            _newSuffix = EditorGUILayout.TextField("New suffix", _newSuffix);
            _suffixAmount = EditorGUILayout.IntField("Suffix amount", _suffixAmount);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("SEPARATOR", EditorStyles.boldLabel);
            _separator = EditorGUILayout.TextField("Pattern (Char)", _separator);
            EditorGUILayout.Space(5);

            if(GUILayout.Button("Clean fields", GUILayout.ExpandWidth(true)))
            {
                _separator = EditorGUILayout.TextField("_");
                _newPrefix = EditorGUILayout.TextField("New prefix", string.Empty);
                _prefixAmount = int.Parse(EditorGUILayout.TextField("Prefix amount", "0"));
                _newSuffix = EditorGUILayout.TextField("New prefix", string.Empty);
                _suffixAmount = int.Parse(EditorGUILayout.TextField("Prefix amount", "0"));
            }

            if(GUILayout.Button("Rename selection", GUILayout.ExpandWidth(true), GUILayout.Height(28)))
            {
                ChangeNames(Selection.assetGUIDs, _newPrefix, _prefixAmount, _newSuffix, _suffixAmount);
            }
        }

        void ChangeNames(string[] objects, string newPrefix, int prefixAmount, string newSuffix, int suffixAmount)
        {
            for(int i = 0; i < objects.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(objects[i]);
                var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                string currentName = obj.name;
                string[] nameData = (currentName.Trim()).Split(_separator[0]);

                string newName = string.Empty;

                if(prefixAmount == 0)
                {
                    if(!string.IsNullOrEmpty(newPrefix) || !string.IsNullOrWhiteSpace(newPrefix))
                    {
                        newName = string.Format($"{newPrefix}_{currentName}");
                    }
                    else
                    {
                        newName = currentName;
                    }
                }
                else
                {
                    newName = newPrefix;
                    for(int j = prefixAmount; j < nameData.Length; j++)
                    {
                        newName = newName + "_" + nameData[j];
                    }
                }

                string[] newNameSuffix = newName.Split(_separator[0]);

                if(suffixAmount == 0)
                {
                    if(!string.IsNullOrEmpty(newSuffix) || !string.IsNullOrWhiteSpace(newSuffix))
                    {
                        newName = string.Format($"{newName}_{newSuffix}");
                    }
                }
                else
                {
                    newName = string.Empty;

                    for(int j = 0; j < newNameSuffix.Length - suffixAmount; j++)
                    {
                        newName = newName + newNameSuffix[j] + "_";
                    }

                    newName = newName.Remove(newName.Length - 1);
                    newName = string.Format($"{newName}_{newSuffix}");
                }

                AssetDatabase.RenameAsset(path, newName);
            }
        }
    }
}
