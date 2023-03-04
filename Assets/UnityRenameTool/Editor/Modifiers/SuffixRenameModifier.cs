using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// Suffixを設定するModifier
    /// </summary>
    [Serializable]
    public class SuffixRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("文字列の後につける文字列")]
        public string suffix = "";

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, StringBuilder extension, int index) {
            fileName.Append(suffix);
        }
    }
}
