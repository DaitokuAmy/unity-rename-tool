using System;
using System.Text;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// 番号を付与するModifier
    /// </summary>
    [Serializable]
    public class NumberingRenameModifier : IRenameModifier {
        bool IRenameModifier.IsActive => active;

        [Tooltip("アクティブ状態")]
        public bool active;
        [Tooltip("フォーマット")]
        public string format = @"_{0}";
        [Tooltip("開始番号")]
        public int startNumber = 0;

        /// <summary>
        /// 編集処理
        /// </summary>
        void IRenameModifier.Modify(StringBuilder fileName, int index) {
            fileName.AppendFormat(format, startNumber + index);
        }
    }
}
