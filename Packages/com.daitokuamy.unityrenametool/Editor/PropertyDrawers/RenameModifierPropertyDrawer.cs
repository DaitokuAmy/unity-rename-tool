using UnityEditor;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameModifierのGUI拡張
    /// </summary>
    [CustomPropertyDrawer(typeof(RenameModifierAttribute))]
    public class RenameModifierPropertyDrawer : PropertyDrawer {
        private const float HeaderHeight = 26.0f;
        private const float HelpHeight = 40.0f;
        private const float IndentWidth = 15.0f;
        
        /// <summary>
        /// GUI描画
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var rect = position;
            var baseLabel = label.text;
            var active = true;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            foreach (SerializedProperty prop in property) {
                if (prop.name == "active") {
                    rect.height = HeaderHeight;
                    DrawHeader(rect, prop, baseLabel);
                    rect.xMin += IndentWidth;
                    if (!prop.boolValue) {
                        active = false;
                        break;
                    }
                }
                else {
                    rect.height = EditorGUI.GetPropertyHeight(prop);
                    EditorGUI.PropertyField(rect, prop, true);
                }

                rect.y += rect.height;
                rect.y += EditorGUIUtility.standardVerticalSpacing * 2;
            }

            if (active && attribute is RenameModifierAttribute attr) {
                if (!string.IsNullOrEmpty(attr.HelpMessage)) {
                    rect.height = HelpHeight;
                    EditorGUI.HelpBox(rect, attr.HelpMessage, MessageType.Info);

                    rect.y += rect.height;
                    rect.y += EditorGUIUtility.standardVerticalSpacing * 2;
                }
            }
        }

        /// <summary>
        /// GUI高さ取得
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var height = 0.0f;
            height += EditorGUIUtility.standardVerticalSpacing;
            var active = true;
            
            foreach (SerializedProperty prop in property) {
                if (prop.name == "active") {
                    height += HeaderHeight;
                    if (!prop.boolValue) {
                        active = false;
                        break;
                    }
                }
                else {
                    height += EditorGUI.GetPropertyHeight(prop);
                }

                height += EditorGUIUtility.standardVerticalSpacing * 2;
            }

            if (active && attribute is RenameModifierAttribute attr) {
                if (!string.IsNullOrEmpty(attr.HelpMessage)) {
                    height += HelpHeight;
                }
            }

            return height;
        }

        /// <summary>
        /// ヘッダー部分描画
        /// </summary>
        private void DrawHeader(Rect position, SerializedProperty activeProperty, string label) {
            EditorGUI.DrawRect(position, new Color(0.15f, 0.15f, 0.15f));
            position.xMin += 4;
            var prev = GUI.color;
            GUI.color = activeProperty.boolValue ? Color.green : Color.gray;
            activeProperty.boolValue =
                EditorGUI.ToggleLeft(position, label, activeProperty.boolValue, EditorStyles.boldLabel);
            GUI.color = prev;
        }
    }
}