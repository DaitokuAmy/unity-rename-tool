# unity-rename-tool
<img width="613" alt="image" src="https://user-images.githubusercontent.com/114905982/221768604-568b458c-884c-4b59-ba61-2d864860c99d.png">

## 概要
#### 特徴
* 大量のアセットを同時に変換しても高速なリネームツール
## セットアップ
#### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力してインストール
   * https://github.com/DaitokuAmy/unity-rename-tool.git?path=/Packages/com.daitokuamy.unityrenametool
   ![image](https://user-images.githubusercontent.com/6957962/209446846-c9b35922-d8cb-4ba3-961b-52a81515c808.png)
あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記します。
```json
{
    "dependencies": {
        "com.daitokuamy.unityrenametool": "https://github.com/DaitokuAmy/unity-rename-tool.git?path=/Packages/com.daitokuamy.unityrenametool"
    }
}
```
バージョンを指定したい場合には以下のように記述します。  
https://github.com/DaitokuAmy/unity-rename-tool.git?path=/Packages/com.daitokuamy.unityrenametool#1.0.0
