using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private StringBuilder _workBuilder1 = new StringBuilder();
        private StringBuilder _workBuilder2 = new StringBuilder();
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
                RenameAssets(settings, true);
                _dirtyPreview = true;
            }
            
            // PreviewWindow描画
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            using (var scope = new EditorGUILayout.ScrollViewScope(_previewScroll, "Box", GUILayout.Height(PreviewHeight))) {
                var prevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = position.width * 0.5f - 20;
                for (var i = 0; i < _previewInfos.Count; i++) {
                    var info = _previewInfos[i];
                    EditorGUILayout.LabelField(info.oldText, info.newText);
                }
                EditorGUIUtility.labelWidth = prevLabelWidth;
                _previewScroll = scope.scrollPosition;
            }

            // リネーム処理
            if (GUILayout.Button("Rename")) {
                RenameAssets(settings);
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
        /// 選択中のアセットをリネーム
        /// </summary>
        private void RenameAssets(RenameToolSettings settings, bool previewMode = false) {
            if (previewMode) {
                _previewInfos.Clear();
            }
            
            var objects = Selection.objects
                .OrderByDescending(x => AssetDatabase.GetAssetPath(x).Split("/").Length)
                .ThenBy(AssetDatabase.GetAssetPath)
                .ToArray();
            for (var i = 0; i < objects.Length; i++) {
                var obj = objects[i];
                var gameObject = obj as GameObject;
                
                // Hierarchyの変換
                if (gameObject != null && !AssetDatabase.Contains(gameObject)) {
                    try {
                        if (!previewMode) {
                            EditorUtility.DisplayProgressBar("Rename", gameObject.name, i / (float)objects.Length);
                        }
                        
                        _workBuilder1.Clear();
                        _workBuilder2.Clear();
                        _workBuilder1.Append(gameObject.name);
                        settings.Modify(_workBuilder1, _workBuilder2, i);
                        if (previewMode) {
                            _previewInfos.Add(new PreviewInfo {
                                oldText = gameObject.name,
                                newText = _workBuilder1.ToString()
                            });
                        }
                        else {
                            gameObject.name = _workBuilder1.ToString();
                            EditorUtility.SetDirty(gameObject);
                        }
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
                    var extension = Path.GetExtension(path);
                    try {
                        if (!previewMode) {
                            EditorUtility.DisplayProgressBar("Rename", path, i / (float)objects.Length);
                        }

                        _workBuilder1.Clear();
                        _workBuilder2.Clear();
                        _workBuilder1.Append(fileName);
                        _workBuilder2.Append(extension);
                        settings.Modify(_workBuilder1, _workBuilder2, i);
                        if (previewMode) {
                            _previewInfos.Add(new PreviewInfo {
                                oldText = $"{fileName}{extension}",
                                newText = $"{_workBuilder1}{_workBuilder2}"
                            });
                        }
                        else {
                            var newFileName = _workBuilder1.ToString();
                            var newExtension = _workBuilder2.ToString();
                            RenameAsset(path, newFileName, newExtension);
                        
                            // Asset側の名前も更新
                            obj.name = newFileName;
                            EditorUtility.SetDirty(obj);
                        }
                    }
                    catch (Exception exception) {
                        Debug.LogError($"Rename failed. [{fileName}]");
                        Debug.LogException(exception);
                    }
                }
            }

            if (!previewMode) {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// アセットのリネーム処理
        /// </summary>
        /// <param name="basePath">元ファイルのパス</param>
        /// <param name="newFileName">変換後のファイル名</param>
        /// <param name="newExtension">変換後の拡張子</param>
        private void RenameAsset(string basePath, string newFileName, string newExtension) {
            var oldFileName = Path.GetFileNameWithoutExtension(basePath);
            var oldExtension = Path.GetExtension(basePath);
            if (oldFileName == newFileName && oldExtension == newExtension) {
                return;
            }
            
            var isDirectory = Directory.Exists(basePath);
            if (isDirectory) {
                var directoryName = Directory.GetParent(basePath)?.FullName ?? "";
                
                // DirectoryのRename
                _workBuilder1.Clear();
                _workBuilder1.Append(directoryName);
                _workBuilder1.Append(Path.DirectorySeparatorChar);
                _workBuilder1.Append(newFileName);
                Directory.Move(basePath, _workBuilder1.ToString());
            }
            else {
                var directoryName = Path.GetDirectoryName(basePath);

                // FileのRename
                _workBuilder1.Clear();
                _workBuilder1.Append(directoryName);
                _workBuilder1.Append(Path.DirectorySeparatorChar);
                _workBuilder1.Append(newFileName);
                _workBuilder1.Append(newExtension);
                File.Move(basePath, _workBuilder1.ToString());
            }

            // MetaのRename
            _workBuilder1.Append(".meta");
            File.Move(basePath + ".meta", _workBuilder1.ToString());
        }
    }
}