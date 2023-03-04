using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// 文字列の大文字/小文字を変換するModifier
    /// </summary>
    [Serializable]
    public class UpperLowerCaseRenameModifier : IRenameModifier {
        /// <summary>
        /// テキスト表記形式
        /// </summary>
        public enum TextCase {
            Lower,
            Upper,
        }
        
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("表記形式")]
        public TextCase textCase;

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, StringBuilder extension, int index) {
            var lowerCase = textCase == TextCase.Lower;
            for (var i = 0; i < fileName.Length; i++) {
                var c = fileName[i];
                var lower = char.IsLower(c);
                if (lower != lowerCase) {
                    if (lower) {
                        fileName[i] = char.ToUpper(c);
                    }
                    else {
                        fileName[i] = char.ToLower(c);
                    }
                }
            }
        }
    }
}
