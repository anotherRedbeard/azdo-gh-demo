using System;
using System.Collections.Generic;

namespace AdoWorkItemGenerator.Generators
{
    public abstract class BaseWorkItemGenerator
    {
        protected readonly Configuration _config;
        protected readonly Random _random = new Random();

        protected BaseWorkItemGenerator(Configuration config)
        {
            _config = config;
        }

        // Abstract methods that each process template must implement
        public abstract List<EpicData> GetEpicsForTeam(string teamName);
        public abstract List<FeatureData> GetFeaturesForEpic(string teamName, string epicTitle);
        public abstract List<BacklogItemData> GetBacklogItemsForFeature(string teamName, string featureTitle);
        public abstract List<TaskData> GetTasksForBacklogItem(string backlogItemTitle);

        // Common method to get process template name
        public abstract string GetProcessTemplateName();
        
        // Common method to get the backlog item type name (varies by template)
        public abstract string GetBacklogItemTypeName();
        
        // Common method to get the effort field name (varies by template)
        public abstract string GetEffortFieldName();

        protected List<BacklogItemData> GenerateDefaultBacklogItems(string featureTitle, int count)
        {
            var states = GetValidBacklogItemStates();
            var points = new[] { 3, 5, 8, 13 };
            var items = new List<BacklogItemData>();

            for (int i = 0; i < count; i++)
            {
                items.Add(new BacklogItemData
                {
                    Title = $"{featureTitle} - Implementation Part {i + 1}",
                    Description = $"Implement functionality for {featureTitle}",
                    StoryPoints = points[_random.Next(points.Length)],
                    State = states[_random.Next(states.Length)]
                });
            }

            return items;
        }

        // Each template defines its valid states
        protected abstract string[] GetValidEpicStates();
        protected abstract string[] GetValidFeatureStates();
        protected abstract string[] GetValidBacklogItemStates();
        protected abstract string[] GetValidTaskStates();
    }

    // Common data classes
    public class EpicData
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string State { get; set; } = "New";
    }

    public class FeatureData
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string State { get; set; } = "New";
    }

    public class BacklogItemData
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int StoryPoints { get; set; }
        public string State { get; set; } = "New";
    }

    public class TaskData
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int RemainingWork { get; set; }
        public string State { get; set; } = "To Do";
    }
}
