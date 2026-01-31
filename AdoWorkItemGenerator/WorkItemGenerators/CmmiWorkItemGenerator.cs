using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoWorkItemGenerator.Generators
{
    public class CmmiWorkItemGenerator : BaseWorkItemGenerator
    {
        public CmmiWorkItemGenerator(Configuration config) : base(config) { }

        public override string GetProcessTemplateName() => "CMMI";
        public override string GetBacklogItemTypeName() => "Requirement";
        public override string GetEffortFieldName() => "Microsoft.VSTS.Scheduling.Size";

        protected override string[] GetValidEpicStates() => new[] { "Proposed", "Active", "Resolved", "Closed" };
        protected override string[] GetValidFeatureStates() => new[] { "Proposed", "Active", "Resolved", "Closed" };
        protected override string[] GetValidBacklogItemStates() => new[] { "Proposed", "Active", "Resolved", "Closed" };
        protected override string[] GetValidTaskStates() => new[] { "Proposed", "Active", "Resolved", "Closed" };

        public override List<EpicData> GetEpicsForTeam(string teamName)
        {
            if (teamName == "Frontend")
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "Compliance Dashboard Implementation", Description = "Build comprehensive compliance and reporting dashboard", State = "Active" },
                    new EpicData { Title = "Audit Trail System", Description = "Implement complete audit trail for all user actions", State = "Proposed" },
                    new EpicData { Title = "Accessibility Compliance", Description = "Ensure WCAG 2.1 AA compliance across platform", State = "Active" }
                };
            }
            else
            {
                return new List<EpicData>
                {
                    new EpicData { Title = "Regulatory Compliance System", Description = "Implement system for regulatory compliance tracking", State = "Active" },
                    new EpicData { Title = "Quality Assurance Framework", Description = "Build comprehensive QA framework", State = "Proposed" },
                    new EpicData { Title = "Risk Management Platform", Description = "Develop risk assessment and management system", State = "Active" }
                };
            }
        }

        public override List<FeatureData> GetFeaturesForEpic(string teamName, string epicTitle)
        {
            if (teamName == "Frontend")
            {
                if (epicTitle.Contains("Compliance Dashboard"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Compliance Metrics Visualization", Description = "Display compliance metrics and KPIs", State = "Active" },
                        new FeatureData { Title = "Report Generation", Description = "Generate compliance reports", State = "Proposed" },
                        new FeatureData { Title = "Real-time Monitoring", Description = "Monitor compliance status in real-time", State = "Active" }
                    };
                }
                else if (epicTitle.Contains("Audit Trail"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Action Logging", Description = "Log all user actions", State = "Proposed" },
                        new FeatureData { Title = "Audit Search", Description = "Search and filter audit logs", State = "Proposed" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Screen Reader Support", Description = "Full screen reader compatibility", State = "Active" },
                        new FeatureData { Title = "Keyboard Navigation", Description = "Complete keyboard navigation support", State = "Active" }
                    };
                }
            }
            else
            {
                if (epicTitle.Contains("Regulatory"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Compliance Rule Engine", Description = "Build rule engine for compliance checks", State = "Active" },
                        new FeatureData { Title = "Regulatory Reporting", Description = "Automated regulatory reporting", State = "Proposed" }
                    };
                }
                else if (epicTitle.Contains("Quality"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Automated Testing Framework", Description = "Comprehensive test automation", State = "Proposed" },
                        new FeatureData { Title = "Quality Metrics Dashboard", Description = "Track quality metrics", State = "Proposed" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "Risk Assessment Module", Description = "Assess and score risks", State = "Active" },
                        new FeatureData { Title = "Risk Mitigation Tracking", Description = "Track risk mitigation efforts", State = "Active" }
                    };
                }
            }
        }

        public override List<BacklogItemData> GetBacklogItemsForFeature(string teamName, string featureTitle)
        {
            if (featureTitle.Contains("Compliance Metrics"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "REQ-001: Display compliance score", Description = "System shall display overall compliance score based on all metrics", StoryPoints = 8, State = "Active" },
                    new BacklogItemData { Title = "REQ-002: Show trend analysis", Description = "System shall show compliance trends over time", StoryPoints = 5, State = "Resolved" },
                    new BacklogItemData { Title = "REQ-003: Alert on threshold breach", Description = "System shall alert when compliance falls below threshold", StoryPoints = 8, State = "Proposed" }
                };
            }
            else if (featureTitle.Contains("Rule Engine"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "REQ-101: Configure compliance rules", Description = "System shall allow configuration of compliance rules", StoryPoints = 13, State = "Active" },
                    new BacklogItemData { Title = "REQ-102: Execute rule validation", Description = "System shall validate data against configured rules", StoryPoints = 8, State = "Active" },
                    new BacklogItemData { Title = "REQ-103: Log rule violations", Description = "System shall log all rule violations", StoryPoints = 5, State = "Resolved" }
                };
            }
            
            return GenerateDefaultBacklogItems(featureTitle, 3);
        }

        public override List<TaskData> GetTasksForBacklogItem(string backlogItemTitle)
        {
            var tasks = new List<TaskData>();
            var taskCount = _random.Next(3, 6);
            var taskTemplates = new[]
            {
                ("Requirements analysis", "Analyze and document detailed requirements"),
                ("Design specification", "Create technical design specification"),
                ("Implementation", "Develop the solution"),
                ("Unit testing", "Create and execute unit tests"),
                ("Integration testing", "Perform integration testing"),
                ("Documentation", "Create technical and user documentation"),
                ("Code review", "Conduct peer code review"),
                ("Validation", "Validate against requirements")
            };

            var selectedTemplates = taskTemplates.OrderBy(x => _random.Next()).Take(taskCount).ToList();

            foreach (var (title, description) in selectedTemplates)
            {
                var state = GetValidTaskStates()[_random.Next(GetValidTaskStates().Length)];
                var remainingWork = state == "Closed" || state == "Resolved" ? 0 : _random.Next(1, 9);

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
