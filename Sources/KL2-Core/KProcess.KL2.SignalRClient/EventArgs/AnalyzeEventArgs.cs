using KProcess.Ksmed.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace KProcess.KL2.SignalRClient
{
    public enum TargetAnalyze
    {
        None = 0,
        Project = 1,
        ProjectDir = 2,
        Process = 3,
        Scenario = 4
    }

    public class AnalyzeEventArgs : EventArgs
    {
        public string Messages { get; }

        public object Data { get; }

        public ProjectDir TargetProjectDir { get; }
        public Project TargetProject { get; }
        public Procedure TargetProcess { get; }
        public Scenario TargetScenario { get; }

        public TargetAnalyze TargetAnalyze { get; }

        public AnalyzeEventArgs(string messages, object data = null, TargetAnalyze targetAnalyze = TargetAnalyze.None)
        {
            Messages = messages;
            Data = data;
            TargetAnalyze = targetAnalyze;

            if (!(data is JObject) || targetAnalyze == TargetAnalyze.None)
                return;

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            switch (targetAnalyze)
            {
                case TargetAnalyze.ProjectDir:
                    TargetProjectDir = JsonConvert.DeserializeObject<ProjectDir>(data.ToString(), settings);
                    break;
                case TargetAnalyze.Process:
                    TargetProcess = JsonConvert.DeserializeObject<Procedure>(data.ToString(), settings);
                    break;
                case TargetAnalyze.Project:
                    TargetProject = JsonConvert.DeserializeObject<Project>(data.ToString(), settings);
                    break;
                case TargetAnalyze.Scenario:
                    // Scenario can't be deserialized because of Resource deserialization
                    //TargetScenario = JsonConvert.DeserializeObject<Scenario>(data.ToString(), settings);
                    break;
            }
        }
    }
}
