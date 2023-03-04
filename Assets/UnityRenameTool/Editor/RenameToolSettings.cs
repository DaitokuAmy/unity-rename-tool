using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameTool用の設定ファイル
    /// </summary>
    [FilePath("UnityRenameTool/Settings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class RenameToolSettings : ScriptableSingleton<RenameToolSettings> {
        [Tooltip("文字列トリミング設定"), RenameModifier("[startIndex:0, length:4]hoge_000 -> hoge")]
        public TrimmingRenameModifier trimming;
        [Tooltip("ベース文字列設定"), RenameModifier]
        public BaseNameRenameModifier baseName;
        [Tooltip("大文字小文字設定"), RenameModifier("[case:Upper]hoge -> HOGE, [case:Lower]HOGE -> hoge")]
        public UpperLowerCaseRenameModifier upperLowerCase;
        [Tooltip("置き換え設定"), RenameModifier]
        public ReplaceRenameModifier replace;
        [Tooltip("番号設定"), RenameModifier("[format:_{0:000}]hoge -> hoge_001")]
        public NumberingRenameModifier numbering;
        [Tooltip("Prefix編集用設定"), RenameModifier("[prefix:tex_]hoge -> tex_hoge")]
        public PrefixRenameModifier prefix;
        [Tooltip("Suffix編集用設定"), RenameModifier("[suffix:_normal]hoge -> hoge_normal")]
        public SuffixRenameModifier suffix;
        [Tooltip("拡張子置き換え設定"), RenameModifier]
        public ReplaceExtensionRenameModifier replaceExtension;

        private IRenameModifier[] _modifiers;
        private IRenameModifier[] Modifilers {
            get {
                if (_modifiers == null) {
                    _modifiers = new IRenameModifier[] {
                        trimming,
                        baseName,
                        upperLowerCase,
                        replace,
                        numbering,
                        prefix,
                        suffix,
                        replaceExtension,
                    };
                }

                return _modifiers;
            }
        }

        /// <summary>
        /// 編集処理
        /// </summary>
        /// <param name="fileName">編集対象のファイル名</param>
        /// <param name="extension">編集対象の拡張子</param>
        /// <param name="index">置き換えファイル名のIndex</param>
        public void Modify(StringBuilder fileName, StringBuilder extension, int index) {
            var modifiers = Modifilers;
            for (var i = 0; i < modifiers.Length; i++) {
                var modifier = modifiers[i];
                if (!modifier.IsActive) {
                    continue;
                }

                modifier.Modify(fileName, extension, index);
            }
        }
    }
}