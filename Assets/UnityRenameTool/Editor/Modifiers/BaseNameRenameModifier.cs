using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// ベース文字列設定用Modifier
    /// </summary>
    [Serializable]
    public class BaseNameRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("ベースにする文字列")]
        public string baseText;

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, int index) {
            fileName.Clear();
            fileName.Append(baseText);
        }
    }
}
