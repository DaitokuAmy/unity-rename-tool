using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameModifierEditor拡張用のAttribute
    /// </summary>
    public class RenameModifierAttribute : PropertyAttribute {
        public string HelpMessage { get; }

        public RenameModifierAttribute(string helpMessage = "") {
            HelpMessage = helpMessage;
        }
    }
}
