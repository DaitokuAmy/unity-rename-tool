using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// Prefixを設定するModifier
    /// </summary>
    [Serializable]
    public class PrefixRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("文字列の頭につける文字列")]
        public string prefix = "";

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, int index) {
            fileName.Insert(0, prefix);
        }
    }
}
