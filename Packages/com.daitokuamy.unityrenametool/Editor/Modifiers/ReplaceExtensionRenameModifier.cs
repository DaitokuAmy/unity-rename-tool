using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// 拡張子を置き換えるModifier
    /// </summary>
    [Serializable]
    public class ReplaceExtensionRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("置き換え対象の拡張子")]
        public string oldExtension = "";
        [Tooltip("置き換え後の拡張子")]
        public string newExtension = "";

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, StringBuilder extension, int index) {
            if (string.IsNullOrEmpty(oldExtension)) {
                return;
            }
            
            // 拡張子なし
            if (extension.Length <= 0) {
                return;
            }
            
            // 置き換え対象チェック
            if (extension.Length != oldExtension.Length + 1) {
                return;
            }
            
            for (var i = 1; i < extension.Length; i++) {
                if (extension[i] != oldExtension[i - 1]) {
                    return;
                }
            }
            
            // 置き換え
            extension.Remove(1, extension.Length - 1);
            extension.Append(newExtension);
        }
    }
}
