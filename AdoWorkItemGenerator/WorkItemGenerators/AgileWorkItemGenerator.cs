using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoWorkItemGenerator.Generators
{
    public class AgileWorkItemGenerator : BaseWorkItemGenerator
    {
        public AgileWorkItemGenerator(Configuration config) : base(config) { }

        public override string GetProcessTemplateName() => "Agile";
        public override string GetBacklogItemTypeName() => "User Story";
        public override string GetEffortFieldName() => "Microsoft.VSTS.Scheduling.StoryPoints";

        protected override string[] GetValidEpicStates() => new[] { "New", "Active", "Resolved", "Closed" };
        protected override string[] GetValidFeatureStates() => new[] { "New", "Active", "Resolved", "Closed" };
        protected override string[] GetValidBacklogItemStates() => new[] { "New", "Active", "Resolved", "Closed", "Removed" };
        protected override string[] GetValidTaskStates() => new[] { "To Do", "In Progress", "Done" };

        public override List<EpicData> GetEpicsForTeam(string teamName)
        {
            if (teamName == "Frontend")
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "Customer Experience Enhancement", Description = "Improve overall customer experience across web and mobile platforms", State = "Active" },
                    new EpicData { Title = "Mobile First Initiative", Description = "Redesign for mobile-first approach", State = "New" },
                    new EpicData { Title = "Performance Optimization", Description = "Optimize application performance and loading times", State = "Active" }
                };
            }
            else
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "Cloud Migration", Description = "Migrate infrastructure to cloud-native services", State = "Active" },
                    new EpicData { Title = "Security Hardening", Description = "Enhance security across all services", State = "New" },
                    new EpicData { Title = "Scalability Improvements", Description = "Improve system scalability and reliability", State = "Active" }
                };
            }
        }

        public override List<FeatureData> GetFeaturesForEpic(string teamName, string epicTitle)
        {
            if (teamName == "Frontend")
            {
                if (epicTitle.Contains("Customer Experience"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Personalized Homepage", Description = "Create personalized homepage based on user preferences", State = "Active" },
                        new FeatureData { Title = "Enhanced Search", Description = "Improve search with filters and suggestions", State = "New" },
                        new FeatureData { Title = "Streamlined Checkout", Description = "Simplify checkout process", State = "Active" }
                    };
                }
                else if (epicTitle.Contains("Mobile First"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Responsive Design System", Description = "Build mobile-first design system", State = "New" },
                        new FeatureData { Title = "Touch Optimizations", Description = "Optimize UI for touch interactions", State = "New" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Code Splitting", Description = "Implement code splitting for faster load", State = "Active" },
                        new FeatureData { Title = "Image Optimization", Description = "Optimize and lazy-load images", State = "Active" }
                    };
                }
            }
            else
            {
                if (epicTitle.Contains("Cloud Migration"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Containerization", Description = "Containerize all services", State = "Active" },
                        new FeatureData { Title = "Kubernetes Deployment", Description = "Deploy to Kubernetes cluster", State = "New" }
                    };
                }
                else if (epicTitle.Contains("Security"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Authentication Upgrade", Description = "Implement modern auth standards", State = "New" },
                        new FeatureData { Title = "Data Encryption", Description = "Encrypt sensitive data at rest", State = "New" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Auto-Scaling", Description = "Implement auto-scaling for services", State = "Active" },
                        new FeatureData { Title = "Load Balancing", Description = "Improve load balancing strategy", State = "Active" }
                    };
                }
            }
        }

        public override List<BacklogItemData> GetBacklogItemsForFeature(string teamName, string featureTitle)
        {
            if (featureTitle.Contains("Personalized Homepage"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "Display recommended products", Description = "As a user, I want to see products recommended for me", StoryPoints = 8, State = "Active" },
                    new BacklogItemData { Title = "Show recent activity", Description = "As a user, I want to see my recent browsing history", StoryPoints = 5, State = "Resolved" },
                    new BacklogItemData { Title = "Customize layout preferences", Description = "As a user, I want to customize my homepage layout", StoryPoints = 13, State = "New" }
                };
            }
            
            return GenerateDefaultBacklogItems(featureTitle, 3);
        }

        public override List<TaskData> GetTasksForBacklogItem(string backlogItemTitle)
        {
            var tasks = new List<TaskData>();
            var taskCount = _random.Next(2, 5);
            var taskTemplates = new[]
            {
                ("Design UI mockups", "Create wireframes and mockups"),
                ("Implement frontend", "Build user interface components"),
                ("Develop backend API", "Create supporting backend services"),
                ("Write tests", "Create unit and integration tests"),
                ("Review and refine", "Code review and improvements")
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
