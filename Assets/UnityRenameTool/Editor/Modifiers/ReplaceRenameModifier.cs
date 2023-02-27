using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// 文字列を置き換えるModifier
    /// </summary>
    [Serializable]
    public class ReplaceRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("置き換え対象の文字列")]
        public string oldText = "";
        [Tooltip("置き換え後の文字列")]
        public string newText = "";

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, int index) {
            fileName.Replace(oldText, newText);
        }
    }
}
