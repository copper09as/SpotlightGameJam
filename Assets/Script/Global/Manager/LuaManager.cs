using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance { get; private set; }
    public LuaEnv _luaEnv;
    private readonly Dictionary<string, LuaTable> _loadedScripts = new();
    public const string LUA_SCRIPTS_PATH = "LuaScripts/";

    [Header("调试设置")]
    public bool enableLog = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLuaEnv();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// 初始化 Lua 环境
    private void InitializeLuaEnv()
    {
        _luaEnv = new LuaEnv();
        _luaEnv.AddLoader(CustomLuaLoader);
        Log("Lua 环境初始化完成");
    }

    /// 自定义 Lua 加载器
    private byte[] CustomLuaLoader(ref string filePath)
    {
        string fullPath = LUA_SCRIPTS_PATH + filePath;

        // 1️⃣ 尝试 StreamingAssets
        string streamingPath = Path.Combine(Application.streamingAssetsPath, fullPath + ".txt");
        if (File.Exists(streamingPath))
        {
            Debug.Log($"从 StreamingAssets 加载 Lua 文件: {streamingPath}");
            return File.ReadAllBytes(streamingPath);
        }
        else
        {
            Debug.LogWarning($"StreamingAssets Lua 文件不存在: {streamingPath}, 尝试从 Resources 加载...");
        }

        // 2️⃣ 尝试 Resources（去掉扩展名）
        string resourcePath = Path.ChangeExtension(fullPath, null);
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset != null)
        {
            Debug.Log($"从 Resources 加载 Lua 文件: {resourcePath}");
            return textAsset.bytes;
        }

        Debug.LogError($"Lua 文件在 StreamingAssets 和 Resources 中都不存在: {filePath}");
        return null;
    }

    /// 加载 Lua 脚本
    public LuaTable LoadScript(string scriptName)
    {
        if (_loadedScripts.TryGetValue(scriptName, out var existing))
            return existing;
        try
        {
            LuaTable scriptEnv = _luaEnv.NewTable();
            LuaTable meta = _luaEnv.NewTable();
            meta.Set("__index", _luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            _luaEnv.DoString($"require '{scriptName}'", scriptName, scriptEnv);
            _loadedScripts[scriptName] = scriptEnv;

            Log($"加载脚本成功: {scriptName}");
            return scriptEnv;
        }
        catch (Exception e)
        {
            LogError($"加载脚本失败 {scriptName}: {e.Message}");
            return null;
        }
    }


    /// 调用 Lua 函数
    public object[] CallFunction(string scriptName, string functionName, params object[] args)
    {
        if (!_loadedScripts.TryGetValue(scriptName, out var scriptEnv))
            scriptEnv = LoadScript(scriptName);

        if (scriptEnv == null) return null;

        var function = scriptEnv.Get<LuaFunction>(functionName);
        if (function == null)
        {
            LogError($"函数未找到: {functionName} in {scriptName}");
            return null;
        }

        return function.Call(args);
    }

    /// 卸载 Lua 脚本
    public void UnloadScript(string scriptName)
    {
        if (_loadedScripts.TryGetValue(scriptName, out var scriptEnv))
        {
            scriptEnv.Dispose();
            _loadedScripts.Remove(scriptName);
            Log($"卸载脚本: {scriptName}");
        }
    }

    /// 执行 Lua 代码片段
    public object[] DoString(string luaCode, string chunkName = "chunk")
    {
        try
        {
            return _luaEnv.DoString(luaCode, chunkName);
        }
        catch (Exception e)
        {
            LogError($"执行 Lua 代码失败: {e.Message}");
            return null;
        }
    }

    /// 获取 / 设置 Lua 全局变量
    public T GetGlobal<T>(string name) => _luaEnv.Global.Get<T>(name);
    public void SetGlobal(string name, object value) => _luaEnv.Global.Set(name, value);

    void Update()
    {
        _luaEnv?.Tick();
    }

    void OnDestroy()
    {
        foreach (var script in _loadedScripts.Values)
            script.Dispose();

        _loadedScripts.Clear();
        _luaEnv?.Dispose();
        _luaEnv = null;
    }

    #region 日志
    private void Log(string msg)
    {
        if (enableLog) Debug.Log($"[LuaManager] {msg}");
    }
    private void LogError(string msg)
    {
        if (enableLog) Debug.LogError($"[LuaManager] {msg}");
    }
    #endregion
}
