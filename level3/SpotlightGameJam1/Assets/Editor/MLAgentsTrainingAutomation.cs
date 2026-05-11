#if UNITY_EDITOR
using System;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Demonstrations;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MLAgentsTrainingAutomation
{
    private const string CommandPath = "Temp/MLAgentsAutomation/build_command.json";
    private const string RunningCommandPath = "Temp/MLAgentsAutomation/build_command.running.json";
    private const string ResultPath = "Temp/MLAgentsAutomation/build_result.json";
    private static double nextPollTime;

    [Serializable]
    private class BuildCommand
    {
        public string scenePath;
        public string outputPath;
        public bool stompFirst;
    }

    [Serializable]
    private class AutomationBuildResult
    {
        public bool success;
        public string scenePath;
        public string outputPath;
        public string message;
    }

    static MLAgentsTrainingAutomation()
    {
        EditorApplication.update += PollCommandFile;
    }

    public static void BuildLevelTest01()
    {
        BuildScene("Assets/Scenes/LEVELTEST01.unity", "Builds/MLTraining/LEVELTEST01/SpotlightTraining.exe");
    }

    public static void BuildLevelTest02()
    {
        BuildScene("Assets/Scenes/LEVELTEST02.unity", "Builds/MLTraining/LEVELTEST02/SpotlightTraining.exe");
    }

    public static void BuildLevelTest03()
    {
        BuildScene("Assets/Scenes/LEVELTEST03.unity", "Builds/MLTraining/LEVELTEST03/SpotlightTraining.exe");
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

            var command = JsonUtility.FromJson<BuildCommand>(File.ReadAllText(CommandPath));
            BuildScene(command.scenePath, command.outputPath, command.stompFirst);
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
            var command = JsonUtility.FromJson<BuildCommand>(File.ReadAllText(RunningCommandPath));
            BuildScene(command.scenePath, command.outputPath, command.stompFirst);
            WriteResult(ResultPath, true, command.scenePath, command.outputPath, "Build succeeded.");
            File.Delete(RunningCommandPath);
        }
        catch (Exception ex)
        {
            WriteResult(ResultPath, false, null, null, ex.ToString());
            Debug.LogException(ex);
        }
    }

    private static void BuildScene(string scenePath, string outputPath, bool stompFirst = false)
    {
        if (string.IsNullOrWhiteSpace(scenePath))
        {
            throw new ArgumentException("Scene path is empty.");
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path is empty.");
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
        ConfigureTrainingScene(stompFirst);
        EditorSceneManager.SaveScene(scene);

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

    private static void ConfigureTrainingScene(bool stompFirst)
    {
        foreach (var behavior in UnityEngine.Object.FindObjectsOfType<BehaviorParameters>(true))
        {
            behavior.BehaviorName = "My Behavior";
            behavior.BehaviorType = BehaviorType.Default;
            behavior.Model = null;
        }

        foreach (var decisionRequester in UnityEngine.Object.FindObjectsOfType<DecisionRequester>(true))
        {
            decisionRequester.DecisionPeriod = 1;
            decisionRequester.TakeActionsBetweenDecisions = true;
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

        foreach (var agentController in UnityEngine.Object.FindObjectsOfType<AgentController>(true))
        {
            var serializedController = new SerializedObject(agentController);
            var stompFirstProperty = serializedController.FindProperty("stompEnemyBeforeGoal");
            if (stompFirstProperty != null)
            {
                stompFirstProperty.boolValue = stompFirst;
                serializedController.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorUtility.SetDirty(agentController);
        }

        foreach (var recorder in UnityEngine.Object.FindObjectsOfType<DemonstrationRecorder>(true))
        {
            recorder.Record = false;
        }
    }

    private static void WriteResult(string path, bool success, string scenePath, string outputPath, string message)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var result = new AutomationBuildResult
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
