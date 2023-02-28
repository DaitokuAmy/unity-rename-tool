using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityRenameTool.Editor {
    /// <summary>
    /// RenameToolのメインウィンドウ
    /// </summary>
    public class RenameToolWindow : EditorWindow {
        private const float PreviewHeight = 150.0f; 
        
        /// <summary>
        /// プレビュー用のテキスト情報
        /// </summary>
        private struct PreviewInfo {
            public string oldText;
            public string newText;
        }
        
        private UnityEditor.Editor _settingsEditor;
        private StringBuilder _workBuilder = new StringBuilder();
        private Vector2 _settingsScroll;
        private Vector2 _previewScroll;
        private bool _dirtyPreview;
        private List<PreviewInfo> _previewInfos = new List<PreviewInfo>();

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

            using (var scope = new EditorGUILayout.ScrollViewScope(_settingsScroll, "Box")) {
                _settingsEditor.OnInspectorGUI();
                _settingsScroll = scope.scrollPosition;
            }
            
            // Preview情報の更新
            if (_dirtyPreview) {
                var objects = Selection.objects;
                _previewInfos.Clear();
                for (var i = 0; i < objects.Length; i++) {
                    var obj = objects[i];
                    var gameObject = obj as GameObject;
                    
                    // Hierarchyの変換
                    if (gameObject != null && !AssetDatabase.Contains(gameObject)) {
                        _workBuilder.Clear();
                        _workBuilder.Append(gameObject.name);
                        settings.Modify(_workBuilder, i);
                        _previewInfos.Add(new PreviewInfo {
                            oldText = gameObject.name,
                            newText = _workBuilder.ToString()
                        });
                    }
                    // ProjectAssetの変換
                    else if (obj != null) {
                        var path = AssetDatabase.GetAssetPath(obj);
                        var fileName = Path.GetFileNameWithoutExtension(path);
                        _workBuilder.Clear();
                        _workBuilder.Append(fileName);
                        settings.Modify(_workBuilder, i);
                        _previewInfos.Add(new PreviewInfo {
                            oldText = fileName,
                            newText = _workBuilder.ToString()
                        });
                    }
                }
                
                _dirtyPreview = true;
            }
            
            // PreviewWindow描画
            using (var scope = new EditorGUILayout.ScrollViewScope(_previewScroll, "Box", GUILayout.Height(PreviewHeight))) {
                for (var i = 0; i < _previewInfos.Count; i++) {
                    var info = _previewInfos[i];
                    EditorGUILayout.LabelField(info.oldText, info.newText);
                }
                _previewScroll = scope.scrollPosition;
            }

            // リネーム処理
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
        /// アクティブ時の処理
        /// </summary>
        private void OnEnable() {
            _dirtyPreview = true;
        }

        /// <summary>
        /// 選択内容に変更があった際の通知
        /// </summary>
        private void OnSelectionChange() {
            _dirtyPreview = true;
            Repaint();
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