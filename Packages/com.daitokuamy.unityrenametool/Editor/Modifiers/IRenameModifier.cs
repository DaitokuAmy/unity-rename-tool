using System.Text;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// Rename用のModifier用インターフェース
    /// </summary>
    public interface IRenameModifier {
        /// <summary>
        /// アクティブ状態
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// 編集処理
        /// </summary>
        /// <param name="fileName">編集対象のファイル名</param>
        /// <param name="extension">編集対象の拡張子</param>
        /// <param name="index">置き換えファイル名のIndex</param>
        public void Modify(StringBuilder fileName, StringBuilder extension, int index);
    }
}
