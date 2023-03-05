using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// 文字列トリミング設定用Modifier
    /// </summary>
    [Serializable]
    public class TrimmingRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("トリミング開始位置")]
        public int startIndex;
        [Tooltip("トリミング長さ")]
        public int length;

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, StringBuilder extension, int index) {
            fileName.Remove(0, startIndex);
            fileName.Remove(length, fileName.Length - length);
        }
    }
}
