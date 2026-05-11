#if UNITY_EDITOR
using System;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MLAgentsInferenceAutomation
{
    private const string CommandPath = "Temp/MLAgentsAutomation/inference_build_command.json";
    private const string RunningCommandPath = "Temp/MLAgentsAutomation/inference_build_command.running.json";
    private const string ResultPath = "Temp/MLAgentsAutomation/inference_build_result.json";
    private static double nextPollTime;

    [Serializable]
    private class InferenceBuildCommand
    {
        public string scenePath;
        public string outputPath;
        public string modelPath;
    }

    [Serializable]
    private class InferenceBuildResult
    {
        public bool success;
        public string scenePath;
        public string outputPath;
        public string message;
    }

    static MLAgentsInferenceAutomation()
    {
        EditorApplication.update += PollCommandFile;
    }

    public static void BuildFinalPassLevel01()
    {
        BuildInferenceScene("Assets/Scenes/LEVELTEST01.unity", "Builds/MLInference/LEVELTEST01_FinalPass/SpotlightTraining.exe", "Assets/ML-Agents/Models/My Behavior_LEVELTEST01_Final.onnx");
    }

    public static void BuildFinalPassLevel02()
    {
        BuildInferenceScene("Assets/Scenes/LEVELTEST02.unity", "Builds/MLInference/LEVELTEST02_FinalPass/SpotlightTraining.exe", "Assets/ML-Agents/Models/My Behavior_LEVELTEST02_Final.onnx");
    }

    public static void BuildFinalPassLevel03()
    {
        BuildInferenceScene("Assets/Scenes/LEVELTEST03.unity", "Builds/MLInference/LEVELTEST03_FinalPass/SpotlightTraining.exe", "Assets/ML-Agents/Models/My Behavior_LEVELTEST03_Final.onnx");
    }

    public static void BuildFinalPassAllLevels()
    {
        BuildFinalPassLevel01();
        BuildFinalPassLevel02();
        BuildFinalPassLevel03();
    }

    public static void BuildFromCommandFile()
    {
        try
        {
            if (!File.Exists(CommandPath))
            {
                WriteResult(ResultPath, false, null, null, $"Missing command file: {CommandPath}");
                return;
            }

            var command = JsonUtility.FromJson<InferenceBuildCommand>(File.ReadAllText(CommandPath));
            BuildInferenceScene(command.scenePath, command.outputPath, command.modelPath);
            WriteResult(ResultPath, true, command.scenePath, command.outputPath, "Build succeeded.");
        }
        catch (Exception ex)
        {
            WriteResult(ResultPath, false, null, null, ex.ToString());
            throw;
        }
    }

    private static void PollCommandFile()
    {
        if (EditorApplication.timeSinceStartup < nextPollTime)
        {
            return;
        }

        nextPollTime = EditorApplication.timeSinceStartup + 2;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            return;
        }

        if (!File.Exists(CommandPath))
        {
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(RunningCommandPath));
            if (File.Exists(RunningCommandPath))
            {
                File.Delete(RunningCommandPath);
            }

            File.Move(CommandPath, RunningCommandPath);
            var command = JsonUtility.FromJson<InferenceBuildCommand>(File.ReadAllText(RunningCommandPath));
            BuildInferenceScene(command.scenePath, command.outputPath, command.modelPath);
            WriteResult(ResultPath, true, command.scenePath, command.outputPath, "Build succeeded.");
            File.Delete(RunningCommandPath);
        }
        catch (Exception ex)
        {
            WriteResult(ResultPath, false, null, null, ex.ToString());
            Debug.LogException(ex);
        }
    }

    private static void BuildInferenceScene(string scenePath, string outputPath, string modelPath)
    {
        if (string.IsNullOrWhiteSpace(scenePath))
        {
            throw new ArgumentException("Scene path is empty.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path is empty.");
        }

        if (string.IsNullOrWhiteSpace(modelPath))
        {
            throw new ArgumentException("Model path is empty.");
        }

        if (!File.Exists(scenePath))
        {
            throw new FileNotFoundException("Scene file not found.", scenePath);
        }

        var outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var scene = EditorSceneManager.OpenScene(scenePath);
        var model = AssetDatabase.LoadAssetAtPath<Unity.Barracuda.NNModel>(modelPath);
        if (model == null)
        {
            throw new FileNotFoundException("NN model asset not found.", modelPath);
        }

        ConfigureInferenceScene(model);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"MLAGENTS_INFERENCE_BUILD scene={scenePath} model={model.name} output={outputPath}");

        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new InvalidOperationException($"Build failed: {report.summary.result}");
        }
    }

    private static void ConfigureInferenceScene(Unity.Barracuda.NNModel model)
    {
        foreach (var behavior in UnityEngine.Object.FindObjectsOfType<BehaviorParameters>(true))
        {
            var serializedBehavior = new SerializedObject(behavior);
            serializedBehavior.FindProperty("m_BehaviorName").stringValue = "My Behavior";
            serializedBehavior.FindProperty("m_BehaviorType").intValue = (int)BehaviorType.InferenceOnly;
            serializedBehavior.FindProperty("m_Model").objectReferenceValue = model;
            serializedBehavior.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(behavior);
        }

        foreach (var decisionRequester in UnityEngine.Object.FindObjectsOfType<DecisionRequester>(true))
        {
            decisionRequester.DecisionPeriod = 1;
            decisionRequester.TakeActionsBetweenDecisions = true;
            EditorUtility.SetDirty(decisionRequester);
        }

        foreach (var agent in UnityEngine.Object.FindObjectsOfType<Agent>(true))
        {
            agent.MaxStep = 12000;
            var serializedAgent = new SerializedObject(agent);
            var maxStepProperty = serializedAgent.FindProperty("MaxStep");
            if (maxStepProperty != null)
            {
                maxStepProperty.intValue = 12000;
                serializedAgent.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorUtility.SetDirty(agent);
        }
    }

    private static void WriteResult(string path, bool success, string scenePath, string outputPath, string message)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var result = new InferenceBuildResult
        {
            success = success,
            scenePath = scenePath,
            outputPath = outputPath,
            message = message
        };
        File.WriteAllText(path, JsonUtility.ToJson(result, true));
    }
}
#endif
