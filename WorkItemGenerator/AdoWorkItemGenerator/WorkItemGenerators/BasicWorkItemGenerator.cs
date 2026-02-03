using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoWorkItemGenerator.Generators
{
    public class BasicWorkItemGenerator : BaseWorkItemGenerator
    {
        public BasicWorkItemGenerator(Configuration config) : base(config) { }

        public override string GetProcessTemplateName() => "Basic";
        public override string GetBacklogItemTypeName() => "Issue";
        public override string GetEffortFieldName() => "Microsoft.VSTS.Scheduling.Size";

        protected override string[] GetValidEpicStates() => new[] { "To Do", "Doing", "Done" };
        protected override string[] GetValidFeatureStates() => new[] { "To Do", "Doing", "Done" };
        protected override string[] GetValidBacklogItemStates() => new[] { "To Do", "Doing", "Done" };
        protected override string[] GetValidTaskStates() => new[] { "To Do", "Doing", "Done" };

        public override List<EpicData> GetEpicsForTeam(string teamName)
        {
            if (teamName == "Frontend")
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "Website Redesign", Description = "Complete redesign of company website", State = "Doing" },
                    new EpicData { Title = "Mobile App Launch", Description = "Launch new mobile application", State = "To Do" }
                };
            }
            else
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "API Development", Description = "Build RESTful API for platform", State = "Doing" },
                    new EpicData { Title = "Infrastructure Setup", Description = "Set up cloud infrastructure", State = "To Do" }
                };
            }
        }

        public override List<FeatureData> GetFeaturesForEpic(string teamName, string epicTitle)
        {
            if (epicTitle.Contains("Website Redesign"))
            {
                return new List<FeatureData>
                {
                    new FeatureData { Title = "Homepage Redesign", Description = "Update homepage with new design", State = "Doing" },
                    new FeatureData { Title = "Navigation Update", Description = "Improve site navigation", State = "To Do" },
                    new FeatureData { Title = "Footer Refresh", Description = "Update footer section", State = "Done" }
                };
            }
            else if (epicTitle.Contains("API Development"))
            {
                return new List<FeatureData>
                {
                    new FeatureData { Title = "User Endpoints", Description = "Create user management endpoints", State = "Doing" },
                    new FeatureData { Title = "Product Endpoints", Description = "Create product management endpoints", State = "To Do" }
                };
            }
            
            return new List<FeatureData>
            {
                new FeatureData { Title = $"{epicTitle} - Phase 1", Description = "Initial phase", State = "To Do" },
                new FeatureData { Title = $"{epicTitle} - Phase 2", Description = "Second phase", State = "To Do" }
            };
        }

        public override List<BacklogItemData> GetBacklogItemsForFeature(string teamName, string featureTitle)
        {
            if (featureTitle.Contains("Homepage"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "Update hero section", Description = "Redesign hero section with new branding", StoryPoints = 5, State = "Done" },
                    new BacklogItemData { Title = "Add featured products section", Description = "Display featured products on homepage", StoryPoints = 8, State = "Doing" },
                    new BacklogItemData { Title = "Implement testimonials carousel", Description = "Add customer testimonials", StoryPoints = 5, State = "To Do" }
                };
            }
            
            return GenerateDefaultBacklogItems(featureTitle, 2);
        }

        public override List<TaskData> GetTasksForBacklogItem(string backlogItemTitle)
        {
            var tasks = new List<TaskData>();
            var taskCount = _random.Next(2, 4);
            var taskTemplates = new[]
            {
                ("Setup", "Initial setup and configuration"),
                ("Implementation", "Core implementation work"),
                ("Testing", "Test the implementation"),
                ("Documentation", "Update documentation")
            };

            var selectedTemplates = taskTemplates.OrderBy(x => _random.Next()).Take(taskCount).ToList();

            foreach (var (title, description) in selectedTemplates)
            {
                var state = GetValidTaskStates()[_random.Next(GetValidTaskStates().Length)];
                var remainingWork = state == "Done" ? 0 : _random.Next(1, 9);

                tasks.Add(new TaskData
                {
                    Title = title,
                    Description = description,
                    RemainingWork = remainingWork,
                    State = state
                });
            }

            return tasks;
        }
    }
}
