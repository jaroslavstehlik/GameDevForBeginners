using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Networking;

public class GameDevBeginnersHttpServer : EditorWindow
{
    private const string VALID_PACKAGE_URL = "Packages/digital.dream.gamedevforbeginners";
    
    private const string HOST_URL = "http://localhost:8081/";
    private const string ASSETS_FOLDER_NAME = "Assets";
    private const string EXAMPLES_FOLDER_NAME = "Examples";
    
    public enum ActionType
    {
        unknown,
        loadScene,
        loadScript,
        message
    }
    
    private const string GAME_DEV_FOR_BEGINNERS = "Game dev for beginners";
    private HttpListener _listener;
    private Thread _listenerThread;
    private volatile bool _isRunning = false;
    private ConcurrentQueue<(ActionType, string)> _concurrentQueue = new ConcurrentQueue<(ActionType, string)>();
    private StringBuilder _console = new StringBuilder();
    private Vector2 _consoleScrollPosition;
    
    [MenuItem("Window/Game dev for beginners")]
    public static void ShowWindow()
    {
        GameDevBeginnersHttpServer wnd = GetWindow<GameDevBeginnersHttpServer>();
        wnd.titleContent = new GUIContent("Game dev for beginners");
    }

    void OnEnable()
    {
        StartHttpServer(HOST_URL);
        EditorApplication.update += Update;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(_isRunning ? "Connected" : "Disconnected");
        _consoleScrollPosition = EditorGUILayout.BeginScrollView(_consoleScrollPosition, 
            GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        
        string consoleText = _console.ToString();
        GUIStyle labelStyle = new GUIStyle(GUI.skin.textField);
        labelStyle.alignment = TextAnchor.UpperLeft;
        labelStyle.fixedHeight = labelStyle.CalcHeight(new GUIContent(consoleText), 100);
        EditorGUILayout.LabelField(consoleText,  labelStyle, GUILayout.Height(labelStyle.fixedHeight), GUILayout.ExpandWidth(true));
        EditorGUILayout.EndScrollView();
    }

    private void Update()
    {
        if (_concurrentQueue.TryDequeue(out (ActionType, string) output))
        {
            switch (output.Item1)
            {
                case ActionType.loadScene:
                    _console.AppendLine($"Load scene: {output.Item2}");
                    LoadScene(output.Item2);
                    break;
                case ActionType.loadScript:
                    _console.AppendLine($"Load script: {output.Item2}");
                    LoadScript(output.Item2);
                    break;
                case ActionType.message:
                    _console.AppendLine($"Got message: {output.Item2}");
                    break;
            }
        }
    }

    static bool LoadScript(string path)
    {
        string targetPath = Path.Combine(VALID_PACKAGE_URL, path);
        if (!targetPath.EndsWith(".cs") ||
            !File.Exists(targetPath))
            return false;

        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(targetPath);
        AssetDatabase.OpenAsset(textAsset);
        return true;
    }
    
    static void LoadScene(string sceneName)
    {
        string examplesPath = $"{ASSETS_FOLDER_NAME}/{EXAMPLES_FOLDER_NAME}";
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(examplesPath, AssetPathToGUIDOptions.OnlyExistingAssets)))
        {
            AssetDatabase.CreateFolder(ASSETS_FOLDER_NAME, EXAMPLES_FOLDER_NAME);
        }

        string[] sceneAssetGuids = AssetDatabase.FindAssets("t:scene");
        foreach (var sceneAssetGuid in sceneAssetGuids)
        {
            string sceneAssetPath = AssetDatabase.GUIDToAssetPath(sceneAssetGuid);
            // validate correct package url
            if (!sceneAssetPath.StartsWith(VALID_PACKAGE_URL) ||
                !sceneAssetPath.Contains(sceneName))
                continue;
            
            string sceneFileName = Path.GetFileName(sceneAssetPath);
            string clonedScenePath = $"{examplesPath}/{sceneFileName}";
            // validate that we are not opening cloned scene
            if(sceneAssetPath == clonedScenePath)
                continue;
            
            string clonedScenePathGUID = AssetDatabase.AssetPathToGUID(clonedScenePath, AssetPathToGUIDOptions.OnlyExistingAssets);
            if (string.IsNullOrEmpty(clonedScenePathGUID))
            {
                AssetDatabase.CopyAsset(sceneAssetPath, clonedScenePath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                if (EditorUtility.DisplayDialog($"Overwrite existing scene?", $"Overwrite existing scene?\n{clonedScenePath} ", "ok", "cancel"))
                {
                    AssetDatabase.DeleteAsset(clonedScenePath);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    AssetDatabase.CopyAsset(sceneAssetPath, clonedScenePath);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
            
            EditorSceneManager.OpenScene(clonedScenePath);
            break;
        }
    }

    void OnDisable()
    {
        EditorApplication.update -= Update;
        StopHttpServer();
    }

    public void StartHttpServer(string uriPrefix)
    {
        if (!HttpListener.IsSupported)
        {
            _console.AppendLine($"{GAME_DEV_FOR_BEGINNERS}, HttpListener is not supported on this platform.");
            Debug.LogError($"{GAME_DEV_FOR_BEGINNERS}, HttpListener is not supported on this platform.");
            return;
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add(uriPrefix);
        _isRunning = true;
        _listener.Start();
        _console.AppendLine($"{GAME_DEV_FOR_BEGINNERS}, HTTP Server started at {uriPrefix}");

        _listenerThread = new Thread(() =>
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    ProcessRequest(context);
                }
                catch (Exception e)
                {
                    _console.AppendLine($"{GAME_DEV_FOR_BEGINNERS}, HTTP Listener Exception: " + e.Message);
                    Debug.LogWarning($"{GAME_DEV_FOR_BEGINNERS}, HTTP Listener Exception: " + e.Message);
                }
            }
        });
        _listenerThread.Start();
    }

    public void StopHttpServer()
    {
        _isRunning = false;
        if (_listener != null)
        {
            _listener.Stop();
            _listener.Close();
            _listener = null;
        }
        
        if (_listenerThread != null)
        {
            _listenerThread.Join();
            _listenerThread = null;
        }
        
        _console.AppendLine($"{GAME_DEV_FOR_BEGINNERS}, HTTP Server stopped.");
        Debug.Log($"{GAME_DEV_FOR_BEGINNERS}, HTTP Server stopped.");
    }

    private ActionType ParseActionType(string actionType)
    {
        string parsedString = actionType.ToLower();
        switch (parsedString)
        {
            case "loadscene":
                return ActionType.loadScene;
            case "loadscript":
                return ActionType.loadScript;
            case "message":
                return ActionType.message;
            default:
                return ActionType.unknown;
        }
    }

    private void SendResponse(HttpListenerContext context, string responseText)
    {
        var response = context.Response;
        string responseString = $"{GAME_DEV_FOR_BEGINNERS}, {responseText}";
        // Prepare the response
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;

        // Write the response
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);

        // Close the response
        output.Close();
    }
    
    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        
        // Read query parameters
        string query = request.Url.Query;
        if(string.IsNullOrEmpty(query))
            return;
        
        // Parse query parameters
        string actionTypeString = GetQueryParameter(query, "actionType");
        string valueString = GetQueryParameter(query, "value");
        if (!string.IsNullOrEmpty(actionTypeString))
        {
            ActionType actionType = ParseActionType(actionTypeString);
            _concurrentQueue.Enqueue((actionType, valueString));
            SendResponse(context, $"success: {actionType}, {valueString}");
        }
        else
        {
            _console.AppendLine($"invalid query: {query}");
            SendResponse(context, $"invalid query!");
        }
    }

    private void ParseQueryString(string query, Encoding encoding, NameValueCollection result)
    {
        if (query.Length == 0)
            return;

        var decodedLength = query.Length;
        var namePos = 0;
        var first = true;

        while (namePos <= decodedLength)
        {
            int valuePos = -1, valueEnd = -1;
            for (var q = namePos; q < decodedLength; q++)
            {
                if ((valuePos == -1) && (query[q] == '='))
                {
                    valuePos = q + 1;
                }
                else if (query[q] == '&')
                {
                    valueEnd = q;
                    break;
                }
            }

            if (first)
            {
                first = false;
                if (query[namePos] == '?')
                    namePos++;
            }

            string name;
            if (valuePos == -1)
            {
                name = null;
                valuePos = namePos;
            }
            else
            {
                name = UnityWebRequest.UnEscapeURL(query.Substring(namePos, valuePos - namePos - 1), encoding);
            }
            if (valueEnd < 0)
            {
                namePos = -1;
                valueEnd = query.Length;
            }
            else
            {
                namePos = valueEnd + 1;
            }
            var value = UnityWebRequest.UnEscapeURL(query.Substring(valuePos, valueEnd - valuePos), encoding);

            result.Add(name, value);
            if (namePos == -1)
                break;
        }
    }
    
    private string GetQueryParameter(string query, string paramName)
    {
        NameValueCollection nameValueCollection = new NameValueCollection();
        ParseQueryString(query, Encoding.UTF8, nameValueCollection);
        return nameValueCollection[paramName];
    }
}
