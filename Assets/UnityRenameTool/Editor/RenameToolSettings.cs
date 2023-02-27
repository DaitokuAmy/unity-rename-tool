using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameTool用の設定ファイル
    /// </summary>
    [FilePath("UnityRenameTool/Settings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class RenameToolSettings : ScriptableSingleton<RenameToolSettings> {
        [Tooltip("ベース文字列設定")]
        public BaseNameRenameModifier baseName;
        [Tooltip("大文字小文字設定")]
        public UpperLowerCaseRenameModifier upperLowerCase;
        [Tooltip("Replace設定")]
        public ReplaceRenameModifier replace;
        [Tooltip("番号設定")]
        public NumberingRenameModifier numbering;
        [Tooltip("Prefix編集用設定")]
        public PrefixRenameModifier prefix;
        [Tooltip("Suffix編集用設定")]
        public SuffixRenameModifier suffix;

        private IRenameModifier[] _modifiers;
        private IRenameModifier[] Modifilers {
            get {
                if (_modifiers == null) {
                    _modifiers = new IRenameModifier[] {
                        baseName,
                        upperLowerCase,
                        replace,
                        numbering,
                        prefix,
                        suffix,
                    };
                }

                return _modifiers;
            }
        }

        /// <summary>
        /// 編集処理
        /// </summary>
        /// <param name="fileName">編集対象のファイル名</param>
        /// <param name="index">置き換えファイル名のIndex</param>
        public void Modify(StringBuilder fileName, int index) {
            var modifiers = Modifilers;
            for (var i = 0; i < modifiers.Length; i++) {
                var modifier = modifiers[i];
                if (!modifier.IsActive) {
                    continue;
                }

                modifier.Modify(fileName, index);
            }
        }
    }
}