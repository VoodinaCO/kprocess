using System;

namespace KProcess.Ksmed.Models.Interfaces
{
    public interface INode
    {
        string Label { get; }

        void StartMonitorNodesChanged();

        bool IsSelected { get; set; }

        bool IsExpanded { get; set; }

        bool AllowDrop { get; set; }

        BulkObservableCollection<INode> Nodes { get; }

        int? NodeProjectId { get; set; }

        int? NodeScenarioId { get; set; }
    }

    public static class TreeViewHelper
    {
        public static ProjectDir FindFolder(INode node, int folderId)
        {
            ProjectDir resultFolder = null;
            if (node is ProjectDir folder)
            {
                if (folder.Id == folderId)
                    return folder;
                foreach (ProjectDir subfolder in folder.Childs)
                {
                    resultFolder = FindFolder(subfolder, folderId);
                    if (resultFolder != null)
                        return resultFolder;
                }
            }
            return resultFolder;
        }

        public static Procedure FindProcess(INode node, int processId)
        {
            Procedure resultProcess = null;
            if (node is Procedure process)
            {
                if (process.ProcessId == processId)
                    return process;
            }
            else if (node is ProjectDir folder)
            {
                foreach (Procedure processInFolder in folder.Processes)
                {
                    resultProcess = FindProcess(processInFolder, processId);
                    if (resultProcess != null)
                        return resultProcess;
                }
                foreach (ProjectDir subFolder in folder.Childs)
                {
                    resultProcess = FindProcess(subFolder, processId);
                    if (resultProcess != null)
                        return resultProcess;
                }
            }
            return resultProcess;
        }

        public static Project FindProject(INode node, int projectId)
        {
            Project resultProject = null;
            if (node is Project project)
            {
                if (project.ProjectId == projectId)
                    return project;
            }
            else if (node is Procedure process)
            {
                foreach (Project projectInProcess in process.Projects)
                {
                    resultProject = FindProject(projectInProcess, projectId);
                    if (resultProject != null)
                        return resultProject;
                }
            }
            else if (node is ProjectDir folder)
            {
                foreach (Procedure processInFolder in folder.Processes)
                {
                    resultProject = FindProject(processInFolder, projectId);
                    if (resultProject != null)
                        return resultProject;
                }
                foreach (ProjectDir subFolder in folder.Childs)
                {
                    resultProject = FindProject(subFolder, projectId);
                    if (resultProject != null)
                        return resultProject;
                }
            }
            return resultProject;
        }

        public static void ExpandFolder(ProjectDir folder)
        {
            if (folder == null)
                return;
            folder.IsExpanded = true;
            ProjectDir parentFolder = folder.Parent;
            while (parentFolder != null)
            {
                parentFolder.IsExpanded = true;
                parentFolder = parentFolder.Parent;
            }
        }

        public static void KeepOnly(ProjectDir folder, Predicate<Procedure> predicate)
        {
            folder.Processes.RemoveWhere(p => !predicate(p));
            foreach (ProjectDir subFolder in folder.Childs)
                KeepOnly(subFolder, predicate);
        }
    }
}
