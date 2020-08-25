using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass]
    public class ActionsTimingsMoveManagement_DeleteTests
    {

        public TestContext TestContext { get; set; }

        private class DeleteTestCase
        {
            public string Label { get; set; }
            public string Input { get; set; }
            public string[] Delete { get; set; }
            public Dictionary<string, string> Expected { get; set; }
        }

        [TestMethod]
        [DeploymentItem(@"Resources\ActionsTimingsMoveManagement_DeleteTestCases.json")]
        public void Delete_From_Input_TestCases()
        {

            var jsonObject = JObject.Parse(File.ReadAllText("ActionsTimingsMoveManagement_DeleteTestCases.json"));


            JArray cases = (JArray)jsonObject["delete"];

            var testCases = JsonConvert.DeserializeObject<DeleteTestCase[]>(cases.ToString());

            foreach (var tc in testCases)
            {
                TestContext.WriteLine("---------------------------------");
                TestContext.WriteLine("Analyzing Test Case {0}", tc.Label);

                var inputTasks = new List<KAction>();

                var wbses = tc.Input.Split(' ');

                foreach (var wbs in wbses)
                {
                    var task = new KAction
                    {
                        Label = "T" + wbs,
                        WBS = wbs,
                    };
                    inputTasks.Add(task);

                }

                TestContext.WriteLine("Input :", tc.Label);
                WriteTasksWBS(inputTasks);

                TestContext.WriteLine("Delete : {0}", tc.Delete);

                var tasksToDelete = inputTasks.Where(t => tc.Delete.Any(wbs => wbs == t.WBS)).ToArray();

                var outputTasks = inputTasks.ToList();
                foreach (var task in tasksToDelete)
                {
                    TestContext.WriteLine("Deleting : {0}", tc.Delete);
                    ActionsTimingsMoveManagement.DeleteUpdateWBS(outputTasks.ToArray(), task);

                    outputTasks.Remove(task);
                    WriteTasksWBS(outputTasks);

                    ActionsTimingsMoveManagement.DebugCheckAllWBS(outputTasks);
                }

                TestContext.WriteLine("Output :", tc.Label);
                WriteTasksWBS(outputTasks);

                TestContext.WriteLine("Expected :", tc.Label);
                WriteTasksWBS(tc.Expected);

                int i = 0;
                foreach (var expectedWbs in tc.Expected.Keys)
                {
                    var expectedLabel = tc.Expected[expectedWbs];

                    if (outputTasks.Count - 1 < i)
                        Assert.Fail("The actual has less tasks than expected");

                    var task = outputTasks[i];
                    var actualWbs = task.WBS;
                    var actuallabel = task.Label;


                    Assert.AreEqual(expectedWbs, task.WBS, string.Format("Expected {0}: {1}, Found {2}: {3}", expectedWbs, expectedLabel, actualWbs, actuallabel));
                    Assert.AreEqual(expectedLabel, actuallabel, string.Format("Expected {0}: {1}, Found {2}: {3}", expectedWbs, expectedLabel, actualWbs, actuallabel));


                    i++;
                }

                if (i != outputTasks.Count)
                    Assert.Fail("The actual has more tasks than expected");
            }


        }

        [TestMethod]
        public void Delete_Random_Consistency()
        {
            var random = new Random();
            var testEnd = DateTime.Now.AddSeconds(30);

            while (DateTime.Now < testEnd)
            {

                var tasks = new List<KAction>();
                var rootTasksCount = random.Next(10);

                var currentWbsParts = new Stack<int>();
                currentWbsParts.Push(0);

                PopulateRandom(random, tasks, currentWbsParts, 5, false);

                WriteTasksWBS(tasks);
                ActionsTimingsMoveManagement.DebugCheckAllWBS(tasks);
                if (CheckWBS(tasks) != null)
                    throw new InvalidOperationException("Wrong generation");

                while (tasks.Count > 0)
                {
                    var index = random.Next(tasks.Count);
                    var countToDelete = Math.Min(random.Next(1, 10), tasks.Count);
                    var toDelete = tasks[index];

                    var currentStatus = Clone(tasks).ToArray();

                    ActionsTimingsMoveManagement.DeleteUpdateWBS(tasks.ToArray(), toDelete);
                    tasks.Remove(toDelete);

                    TestContext.WriteLine("Deleted: {0} {1}.", toDelete.WBS, toDelete.Label);

                    var wrongWbs = CheckWBS(tasks);
                    if (wrongWbs != null)
                    {
                        // Rappeler l'état d'origine et l'élément supprimé
                        TestContext.WriteLine("Fail!");
                        TestContext.WriteLine("--- Before deletion, indented format:");
                        WriteTasksWBS(currentStatus);
                        TestContext.WriteLine("--- Before deletion, appended format: {0}", string.Join(" ", currentStatus.Select(t => t.WBS)));
                        TestContext.WriteLine("--- Deleted element {0} {1}", toDelete.WBS, toDelete.Label);
                        TestContext.WriteLine("--- After deletion, indented format:");
                        WriteTasksWBS(tasks);

                        Assert.Fail("The WBS {0} is invalid", wrongWbs);
                    }
                    ActionsTimingsMoveManagement.DebugCheckAllWBS(tasks);
                }

            }
        }

        private void PopulateRandom(Random random, List<KAction> tasks, Stack<int> currentWbsparts, int depth, bool allowNoChild)
        {
            var childrenCount = random.Next(allowNoChild ? 0 : 1, 5);

            for (int i = 0; i < childrenCount; i++)
            {
                currentWbsparts.Push(currentWbsparts.Pop() + 1);

                tasks.Add(CreateTask(currentWbsparts));

                if (depth > 0)
                {
                    currentWbsparts.Push(0);
                    PopulateRandom(random, tasks, currentWbsparts, depth - 1, true);
                    currentWbsparts.Pop();
                }
            }
        }

        private KAction CreateTask(IEnumerable<int> wbsParts)
        {
            var wbs = string.Join(".", wbsParts.Reverse());

            return new KAction
            {
                Label = wbs,
                WBS = wbs,
            };
        }

        private void WriteTasksWBS(IEnumerable<KAction> actions)
        {
            WriteTasksWBS(actions.Select(a => new KeyValuePair<string, string>(a.WBS, a.Label)));
        }

        private void WriteTasksWBS(IEnumerable<KeyValuePair<string, string>> actions)
        {
            foreach (var kvp in actions)
            {
                TestContext.WriteLine("{0}{1}: {2}", new string(' ', WBSHelper.IndentationFromWBS(kvp.Key)), kvp.Key, kvp.Value);
            }
        }

        private IEnumerable<KAction> Clone(IEnumerable<KAction> tasks)
        {
            foreach (var task in tasks)
            {
                yield return new KAction
                {
                    WBS = task.WBS,
                    Label = task.Label,
                };
            }
        }

        private string CheckWBS(IEnumerable<KAction> tasks)
        {
            return CheckWBS(tasks.Select(t => t.WBS));
        }

        private string CheckWBS(IEnumerable<string> wbses)
        {
            if (!wbses.Any())
                return null;

            var maxIndentation = wbses.Select(w => WBSHelper.IndentationFromWBS(w)).Max();

            var currentLevels = new int[maxIndentation];

            string previous = null;
            foreach (var wbs in wbses)
            {
                if (previous != null && !WBSHelper.CanBeSuccessive(previous, wbs))
                    return wbs;

                if (WBSHelper.GetParts(wbs).Any(p => p == 0))
                    return wbs;

                previous = wbs;
            }

            return null;
        }
    }
}
