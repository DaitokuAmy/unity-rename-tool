using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameToolのメインウィンドウ
    /// </summary>
    public class RenameToolWindow : EditorWindow {
        private UnityEditor.Editor _settingsEditor;
        private StringBuilder _workBuilder = new StringBuilder();
        private Vector2 _scroll;

        /// <summary>
        /// Windowを開く処理
        /// </summary>
        [MenuItem("Window/Unity Rename Tool")]
        private static void Open() {
            GetWindow<RenameToolWindow>("Rename Tool");
        }

        /// <summary>
        /// GUI描画
        /// </summary>
        private void OnGUI() {
            var settings = RenameToolSettings.instance;
            settings.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
            UnityEditor.Editor.CreateCachedEditor(settings, null, ref _settingsEditor);

            using (var scope = new EditorGUILayout.ScrollViewScope(_scroll)) {
                _settingsEditor.OnInspectorGUI();
                _scroll = scope.scrollPosition;
            }

            if (GUILayout.Button("Rename")) {
                var objects = Selection.objects;
                for (var i = 0; i < objects.Length; i++) {
                    var obj = objects[i];
                    var gameObject = obj as GameObject;
                    
                    // Hierarchyの変換
                    if (gameObject != null && !AssetDatabase.Contains(gameObject)) {
                        try {
                            EditorUtility.DisplayProgressBar("Rename", gameObject.name, i / (float)objects.Length);
                            _workBuilder.Clear();
                            _workBuilder.Append(gameObject.name);
                            settings.Modify(_workBuilder, i);
                            gameObject.name = _workBuilder.ToString();
                        }
                        catch (Exception exception) {
                            Debug.LogError($"Rename failed. [{gameObject.name}]");
                            Debug.LogException(exception);
                        }
                    }
                    // ProjectAssetの変換
                    else if (obj != null) {
                        var path = AssetDatabase.GetAssetPath(obj);
                        var fileName = Path.GetFileNameWithoutExtension(path);
                        try {
                            EditorUtility.DisplayProgressBar("Rename", path, i / (float)objects.Length);
                            _workBuilder.Clear();
                            _workBuilder.Append(fileName);
                            settings.Modify(_workBuilder, i);
                            var newFileName = _workBuilder.ToString();
                            RenameAsset(path, newFileName);
                            
                            // Asset側の名前も更新
                            obj.name = newFileName;
                            EditorUtility.SetDirty(obj);
                        }
                        catch (Exception exception) {
                            Debug.LogError($"Rename failed. [{fileName}]");
                            Debug.LogException(exception);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// アセットのリネーム処理
        /// </summary>
        /// <param name="basePath">元ファイルのパス</param>
        /// <param name="newFileName">変換後のファイル名</param>
        private void RenameAsset(string basePath, string newFileName) {
            var isDirectory = Directory.Exists(basePath);
            if (isDirectory) {
                var directoryName = Directory.GetParent(basePath)?.FullName ?? "";
                
                // DirectoryのRename
                _workBuilder.Clear();
                _workBuilder.Append(directoryName);
                _workBuilder.Append(Path.DirectorySeparatorChar);
                _workBuilder.Append(newFileName);
                Directory.Move(basePath, _workBuilder.ToString());
            }
            else {
                var directoryName = Path.GetDirectoryName(basePath);
                var extension = Path.GetExtension(basePath);

                // FileのRename
                _workBuilder.Clear();
                _workBuilder.Append(directoryName);
                _workBuilder.Append(Path.DirectorySeparatorChar);
                _workBuilder.Append(newFileName);
                _workBuilder.Append(extension);
                File.Move(basePath, _workBuilder.ToString());
            }

            // MetaのRename
            _workBuilder.Append(".meta");
            File.Move(basePath + ".meta", _workBuilder.ToString());
        }
    }
}