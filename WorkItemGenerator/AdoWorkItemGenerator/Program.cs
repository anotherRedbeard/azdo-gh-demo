using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdoWorkItemGenerator.Generators;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Azure DevOps Work Item Generator for Demo");
        Console.WriteLine("==========================================\n");

        // Load configuration
        var config = LoadConfiguration();
        
        if (config == null)
        {
            Console.WriteLine("Failed to load configuration. Please check appsettings.json");
            return;
        }

        // Validate configuration
        if (string.IsNullOrEmpty(config.PersonalAccessToken) || config.PersonalAccessToken == "YOUR_PAT_TOKEN")
        {
            Console.WriteLine("Please update appsettings.json with your Azure DevOps PAT token");
            return;
        }

        // Create connection
        var credentials = new VssBasicCredential(string.Empty, config.PersonalAccessToken);
        var connection = new VssConnection(new Uri(config.OrganizationUrl), credentials);
        var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        Console.WriteLine($"Connected to: {config.OrganizationUrl}");
        Console.WriteLine($"Project: {config.ProjectName}\n");

        // Check for command line arguments
        if (args.Length > 0 && args[0].ToLower() == "cleanup")
        {
            await CleanupDemoWorkItems(witClient, config);
            Console.WriteLine("\nCleanup complete! Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Show menu
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Create sample work item (single epic/feature/pbi/tasks)");
        Console.WriteLine("2. Generate full backlog (100+ work items)");
        Console.WriteLine("3. Delete all demo work items");
        Console.WriteLine("4. Exit");
        Console.Write("\nEnter your choice (1-4): ");
        
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await CreateSampleWorkItem(witClient, config);
                break;
            case "2":
                // Create sprints if using sprint history
                if (config.UseSprintHistory)
                {
                    await CreateSprintsWithHistory(connection, config);
                    Console.WriteLine();
                }
                await GenerateFullBacklog(witClient, config);
                break;
            case "3":
                await CleanupDemoWorkItems(witClient, config);
                break;
            case "4":
                Console.WriteLine("Exiting...");
                return;
            default:
                Console.WriteLine("Invalid choice");
                break;
        }

        Console.WriteLine("\nDone! Press any key to exit...");
        Console.ReadKey();
    }

    static Configuration? LoadConfiguration()
    {
        try
        {
            var json = File.ReadAllText("appsettings.json");
            var jObject = JObject.Parse(json);
            
            var config = new Configuration
            {
                OrganizationUrl = jObject["AzureDevOps"]?["OrganizationUrl"]?.ToString() ?? "",
                ProjectName = jObject["AzureDevOps"]?["ProjectName"]?.ToString() ?? "",
                PersonalAccessToken = jObject["AzureDevOps"]?["PersonalAccessToken"]?.ToString() ?? "",
                ProcessTemplate = jObject["AzureDevOps"]?["ProcessTemplate"]?.ToString() ?? "Scrum",
                UseSprintHistory = jObject["AzureDevOps"]?["UseSprintHistory"]?.ToObject<bool>() ?? false,
                Teams = jObject["Teams"]?.ToObject<List<TeamConfig>>() ?? new List<TeamConfig>()
            };

            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            return null;
        }
    }

    static async Task CreateSampleWorkItem(WorkItemTrackingHttpClient witClient, Configuration config)
    {
        Console.WriteLine("Creating sample work item...\n");

        BaseWorkItemGenerator workItemGenerator = config.ProcessTemplate.ToLower() switch
        {
            "scrum" => new ScrumWorkItemGenerator(config),
            "agile" => new AgileWorkItemGenerator(config),
            "basic" => new BasicWorkItemGenerator(config),
            "cmmi" => new CmmiWorkItemGenerator(config),
            _ => throw new Exception($"Unsupported process template: {config.ProcessTemplate}")
        };
        
        var team = config.Teams.First(t => t.Name == "Frontend");
        
        // Create an Epic
        var epic = await CreateWorkItem(witClient, config.ProjectName, "Epic", 
            "E-Commerce Platform Modernization",
            "Modernize the entire e-commerce platform with a new frontend architecture using React and improved backend APIs",
            team.AreaPath,
            null,
            "New",
            null,
            null);

        Console.WriteLine($"Created Epic #{epic.Id}: {epic.Fields["System.Title"]}");

        // Create a Feature under the Epic
        var feature = await CreateWorkItem(witClient, config.ProjectName, "Feature",
            "Product Catalog UI Redesign",
            "Redesign the product catalog interface with improved search, filtering, and responsive design",
            team.AreaPath,
            epic.Id,
            "New",
            null,
            null);

        Console.WriteLine($"Created Feature #{feature.Id}: {feature.Fields["System.Title"]}");

        // Create a Product Backlog Item (PBI) under the Feature
        var pbi = await CreateWorkItem(witClient, config.ProjectName, workItemGenerator.GetBacklogItemTypeName(),
            "Implement Product Card Component",
            "As a customer, I want to see product information in an attractive card format so that I can quickly browse products.\n\nAcceptance Criteria:\n- Card displays product image, name, price, and rating\n- Card is responsive and works on mobile devices\n- Card has hover effects for better UX\n- Card links to product detail page",
            team.AreaPath,
            feature.Id,
            "New",
            8,
            workItemGenerator.GetEffortFieldName()); // Story Points/Effort

        Console.WriteLine($"Created {workItemGenerator.GetBacklogItemTypeName()} #{pbi.Id}: {pbi.Fields["System.Title"]}");

        // Create Tasks under the PBI
        var task1 = await CreateTask(witClient, config.ProjectName,
            "Create ProductCard component structure",
            "Set up the basic React component with props interface",
            team.AreaPath,
            pbi.Id!.Value,
            4); // Remaining Work

        Console.WriteLine($"Created Task #{task1.Id}: {task1.Fields["System.Title"]}");

        var task2 = await CreateTask(witClient, config.ProjectName,
            "Implement responsive styling",
            "Add CSS/styled-components for responsive layout",
            team.AreaPath,
            pbi.Id!.Value,
            3);

        Console.WriteLine($"Created Task #{task2.Id}: {task2.Fields["System.Title"]}");
    }

    static async Task<WorkItem> CreateWorkItem(
        WorkItemTrackingHttpClient witClient,
        string projectName,
        string workItemType,
        string title,
        string description,
        string areaPath,
        int? parentId,
        string state,
        int? effort = null,
        string? effortFieldName = null,
        string? iterationPath = null,
        bool useSprintHistory = false)
    {
        // Check if work item with same title already exists in the same area
        if (await WorkItemExists(witClient, projectName, title, areaPath))
        {
            Console.WriteLine($"  ⚠️  Skipping '{title}' - already exists");
            // Return a placeholder (we'll need to handle this in calling code)
            return null!;
        }

        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Title",
                Value = title
            },
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Description",
                Value = description
            },
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.AreaPath",
                Value = areaPath
            }
        };

        if (effort.HasValue && !string.IsNullOrEmpty(effortFieldName))
        {
            patchDocument.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = $"/fields/{effortFieldName}",
                Value = effort.Value
            });
        }

        var workItem = await witClient.CreateWorkItemAsync(patchDocument, projectName, workItemType);

        // Update state if it's not the default "New" state
        var defaultStates = new[] { "New", "To Do", "Proposed" };
        if (!defaultStates.Contains(state))
        {
            await UpdateWorkItemState(witClient, workItem.Id!.Value, state);
        }

        // Assign to sprint if sprint history is enabled
        if (useSprintHistory && !string.IsNullOrEmpty(iterationPath))
        {
            var random = new Random();
            var sprint = GetSprintForState(projectName, state, random);
            await AssignToSprint(witClient, workItem.Id!.Value, sprint, state);
        }

        // Link to parent if specified
        if (parentId.HasValue)
        {
            await LinkWorkItems(witClient, workItem.Id!.Value, parentId.Value);
        }

        return workItem;
    }

    static async Task<WorkItem> CreateTask(
        WorkItemTrackingHttpClient witClient,
        string projectName,
        string title,
        string description,
        string areaPath,
        int parentId,
        int remainingWork,
        string? iterationPath = null,
        bool useSprintHistory = false,
        string state = "To Do")
    {
        // Check if task with same title already exists in the same area
        if (await WorkItemExists(witClient, projectName, title, areaPath))
        {
            Console.WriteLine($"        ⚠️  Skipping task '{title}' - already exists");
            return null!;
        }

        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Title",
                Value = title
            },
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Description",
                Value = description
            },
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.AreaPath",
                Value = areaPath
            },
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork",
                Value = remainingWork
            }
        };

        var task = await witClient.CreateWorkItemAsync(patchDocument, projectName, "Task");

        // Update state if not default
        if (state != "To Do")
        {
            await UpdateWorkItemState(witClient, task.Id!.Value, state);
        }

        // Assign to sprint if sprint history is enabled
        if (useSprintHistory && !string.IsNullOrEmpty(iterationPath))
        {
            var random = new Random();
            var sprint = GetSprintForState(projectName, state, random);
            await AssignToSprint(witClient, task.Id!.Value, sprint, state);
        }

        // Link to parent
        await LinkWorkItems(witClient, task.Id!.Value, parentId);

        return task;
    }

    static async Task LinkWorkItems(WorkItemTrackingHttpClient witClient, int childId, int parentId)
    {
        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/relations/-",
                Value = new
                {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = $"https://dev.azure.com/_apis/wit/workItems/{parentId}",
                    attributes = new
                    {
                        comment = "Linking work items"
                    }
                }
            }
        };

        await witClient.UpdateWorkItemAsync(patchDocument, childId);
    }

    static async Task GenerateFullBacklog(WorkItemTrackingHttpClient witClient, Configuration config)
    {
        Console.WriteLine("\n\nGenerating full backlog...\n");

        var random = new Random();
        
        // Create the appropriate generator based on process template
        BaseWorkItemGenerator workItemGenerator = config.ProcessTemplate.ToLower() switch
        {
            "scrum" => new ScrumWorkItemGenerator(config),
            "agile" => new AgileWorkItemGenerator(config),
            "basic" => new BasicWorkItemGenerator(config),
            "cmmi" => new CmmiWorkItemGenerator(config),
            _ => throw new Exception($"Unsupported process template: {config.ProcessTemplate}. Supported: Scrum, Agile, Basic, CMMI")
        };

        Console.WriteLine($"Using {workItemGenerator.GetProcessTemplateName()} process template");
        Console.WriteLine($"Backlog item type: {workItemGenerator.GetBacklogItemTypeName()}\n");

        // Create sprints with dates if using sprint history
        if (config.UseSprintHistory)
        {
            var credentials = new VssBasicCredential(string.Empty, config.PersonalAccessToken);
            var connection = new VssConnection(new Uri(config.OrganizationUrl), credentials);
            await CreateSprintsWithDates(connection, config.ProjectName);
            
            Console.WriteLine("\n⚠️  IMPORTANT: Configure Team Iterations");
            Console.WriteLine("To see work items on sprint boards, you must configure each team's iterations:");
            Console.WriteLine($"1. Go to Project Settings → Teams → Select your team (Frontend or Backend)");
            Console.WriteLine($"2. Click on 'Iterations and areas'");
            Console.WriteLine($"3. Click 'Select iterations' and check: Sprint 1, Sprint 2, Sprint 3, Sprint 4, Sprint 5");
            Console.WriteLine($"4. Set Sprint 4 as the default iteration");
            Console.WriteLine($"5. Repeat for each team\n");
        }

        foreach (var team in config.Teams)
        {
            Console.WriteLine($"\nGenerating work items for {team.Name} team...");

            var epics = workItemGenerator.GetEpicsForTeam(team.Name);

            foreach (var epicData in epics)
            {
                var epic = await CreateWorkItem(witClient, config.ProjectName, "Epic",
                    epicData.Title, epicData.Description, team.AreaPath, null, epicData.State, null, null, team.IterationPath, config.UseSprintHistory);

                if (epic == null) continue; // Skip if already exists

                Console.WriteLine($"  Created Epic #{epic.Id}: {epicData.Title}");

                // Create features for each epic
                var features = workItemGenerator.GetFeaturesForEpic(team.Name, epicData.Title);

                foreach (var featureData in features)
                {
                    var feature = await CreateWorkItem(witClient, config.ProjectName, "Feature",
                        featureData.Title, featureData.Description, team.AreaPath, epic.Id, featureData.State, null, null, team.IterationPath, config.UseSprintHistory);

                    if (feature == null) continue; // Skip if already exists

                    Console.WriteLine($"    Created Feature #{feature.Id}: {featureData.Title}");

                    // Create backlog items for each feature
                    var backlogItems = workItemGenerator.GetBacklogItemsForFeature(team.Name, featureData.Title);

                    foreach (var backlogItemData in backlogItems)
                    {
                        var backlogItem = await CreateWorkItem(witClient, config.ProjectName, workItemGenerator.GetBacklogItemTypeName(),
                            backlogItemData.Title, backlogItemData.Description, team.AreaPath, feature.Id, backlogItemData.State, backlogItemData.StoryPoints, workItemGenerator.GetEffortFieldName(), team.IterationPath, config.UseSprintHistory);

                        if (backlogItem == null) continue; // Skip if already exists

                        Console.WriteLine($"      Created {workItemGenerator.GetBacklogItemTypeName()} #{backlogItem.Id}: {backlogItemData.Title}");

                        // Create tasks - always create for Done items, sometimes for others
                        bool shouldCreateTasks = backlogItemData.State == "Done" || 
                                                backlogItemData.State == "Closed" || 
                                                backlogItemData.State == "Resolved" ||
                                                (backlogItemData.State != "New" && backlogItemData.State != "To Do" && backlogItemData.State != "Proposed" && random.Next(100) < 50);
                        
                        if (shouldCreateTasks)
                        {
                            var tasks = workItemGenerator.GetTasksForBacklogItem(backlogItemData.Title);

                            foreach (var taskData in tasks)
                            {
                                // If parent is Done, all tasks should be Done too
                                var taskState = taskData.State;
                                var taskRemaining = taskData.RemainingWork;
                                
                                if (backlogItemData.State == "Done" || backlogItemData.State == "Closed" || backlogItemData.State == "Resolved")
                                {
                                    taskState = "Done";
                                    taskRemaining = 0;
                                }

                                // Make task title unique by prepending parent backlog item title
                                var uniqueTaskTitle = $"{backlogItemData.Title}: {taskData.Title}";
                                
                                var task = await CreateTask(witClient, config.ProjectName,
                                    uniqueTaskTitle, taskData.Description, team.AreaPath, backlogItem.Id!.Value, taskRemaining, team.IterationPath, config.UseSprintHistory, taskState);

                                if (task == null) continue; // Skip if already exists

                                Console.WriteLine($"        Created Task #{task.Id}: {taskData.Title}");
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine("\nBacklog generation complete!");
    }

    static async Task CleanupDemoWorkItems(WorkItemTrackingHttpClient witClient, Configuration config)
    {
        Console.WriteLine("\nSearching for demo work items to delete...\n");

        try
        {
            // Build a query that checks each team's area path
            var areaConditions = string.Join(" OR ", 
                config.Teams.Select(t => $"[System.AreaPath] UNDER '{t.AreaPath}'"));

            // WIQL query to find all work items in the configured team areas
            var wiql = new Wiql()
            {
                Query = $@"
                    SELECT [System.Id], [System.Title], [System.WorkItemType]
                    FROM WorkItems
                    WHERE [System.TeamProject] = '{config.ProjectName}'
                    AND ({areaConditions})
                    ORDER BY [System.Id] DESC"
            };

            Console.WriteLine($"Searching in areas: {string.Join(", ", config.Teams.Select(t => t.AreaPath))}\n");
            
            var result = await witClient.QueryByWiqlAsync(wiql);
            
            if (result.WorkItems == null || !result.WorkItems.Any())
            {
                Console.WriteLine("No work items found to delete.");
                Console.WriteLine("\nTip: Make sure the area paths in appsettings.json match your Azure DevOps areas.");
                return;
            }

            var workItemIds = result.WorkItems.Select(w => w.Id).ToList();
            
            Console.WriteLine($"Found {workItemIds.Count} work items to process...\n");
            
            // Fetch work items in batches of 200 (Azure DevOps API limit)
            const int batchSize = 200;
            var allWorkItems = new List<WorkItem>();
            
            for (int i = 0; i < workItemIds.Count; i += batchSize)
            {
                var batch = workItemIds.Skip(i).Take(batchSize).ToList();
                var batchItems = await witClient.GetWorkItemsAsync(batch, expand: WorkItemExpand.None);
                allWorkItems.AddRange(batchItems);
                Console.WriteLine($"Loaded batch {(i / batchSize) + 1}: {batchItems.Count} items");
            }

            Console.WriteLine($"\nTotal found: {allWorkItems.Count} work items:");
            
            // Group by type for summary
            var grouped = allWorkItems.GroupBy(w => w.Fields["System.WorkItemType"].ToString());
            foreach (var group in grouped.OrderBy(g => g.Key))
            {
                Console.WriteLine($"  {group.Count()} {group.Key}(s)");
            }

            Console.WriteLine("\nWARNING: This will DELETE all work items listed above!");
            Console.Write("Are you sure you want to continue? (y/n): ");
            
            var confirmation = Console.ReadLine()?.ToLower();
            if (confirmation != "y" && confirmation != "yes")
            {
                Console.WriteLine("Cleanup cancelled.");
                return;
            }

            Console.WriteLine("\nDeleting work items...");
            int deleted = 0;
            int failed = 0;

            // Delete in reverse order (tasks first, then PBIs, then features, then epics)
            var orderedItems = allWorkItems.OrderByDescending(w => 
            {
                var type = w.Fields["System.WorkItemType"].ToString();
                return type == "Task" ? 4 : 
                       type == "Product Backlog Item" ? 3 : 
                       type == "Feature" ? 2 : 1;
            });

            foreach (var item in orderedItems)
            {
                try
                {
                    await witClient.DeleteWorkItemAsync(item.Id!.Value, destroy: false);
                    deleted++;
                    var type = item.Fields["System.WorkItemType"];
                    var title = item.Fields["System.Title"];
                    Console.WriteLine($"✓ Deleted {type} #{item.Id}: {title}");
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine($"✗ Failed to delete work item #{item.Id}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n{'=',-50}");
            Console.WriteLine($"Deleted: {deleted} work items");
            if (failed > 0)
            {
                Console.WriteLine($"Failed:  {failed} work items");
            }
            Console.WriteLine("\nNote: Work items are moved to Recycle Bin and can be restored if needed.");
            Console.WriteLine("To restore: Project Settings → Boards → Process → Recycle Bin");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cleanup: {ex.Message}");
        }
    }

    static async Task<bool> WorkItemExists(WorkItemTrackingHttpClient witClient, string projectName, string title, string areaPath)
    {
        var wiql = new Wiql()
        {
            Query = $@"
                SELECT [System.Id]
                FROM WorkItems
                WHERE [System.TeamProject] = '{projectName}'
                AND [System.Title] = '{title.Replace("'", "''")}'
                AND [System.AreaPath] = '{areaPath}'"
        };

        var result = await witClient.QueryByWiqlAsync(wiql);
        return result.WorkItems.Any();
    }

    static async Task UpdateWorkItemState(WorkItemTrackingHttpClient witClient, int workItemId, string state)
    {
        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.State",
                Value = state
            }
        };

        await witClient.UpdateWorkItemAsync(patchDocument, workItemId);
    }

    static async Task CreateSprintsWithHistory(VssConnection connection, Configuration config)
    {
        Console.WriteLine("Creating sprints with historical dates...");
        Console.WriteLine("Note: Sprints should be created in Azure DevOps Project Settings → Iterations");
        Console.WriteLine("      The tool will assign work items to these sprint paths:\n");

        // Define sprint dates (2-week sprints)
        var today = DateTime.Now;

        var sprints = new[]
        {
            new { Name = "Sprint 1", Start = today.AddDays(-84), End = today.AddDays(-71), Status = "Past (Completed)" },
            new { Name = "Sprint 2", Start = today.AddDays(-70), End = today.AddDays(-57), Status = "Past (Completed)" },
            new { Name = "Sprint 3", Start = today.AddDays(-56), End = today.AddDays(-43), Status = "Past (Completed)" },
            new { Name = "Sprint 4", Start = today.AddDays(-7), End = today.AddDays(7), Status = "Current (Active)" },
            new { Name = "Sprint 5", Start = today.AddDays(8), End = today.AddDays(21), Status = "Future (Planned)" }
        };

        foreach (var sprint in sprints)
        {
            Console.WriteLine($"  {sprint.Name}: {sprint.Start:MM/dd/yyyy} - {sprint.End:MM/dd/yyyy} ({sprint.Status})");
        }

        Console.WriteLine($"\nTo create these in Azure DevOps:");
        Console.WriteLine($"1. Go to Project Settings → Project configuration → Iterations");
        Console.WriteLine($"2. Click 'New child' under '{config.ProjectName}'");
        Console.WriteLine($"3. Add each sprint with the dates shown above\n");
    }

    static async Task AssignToSprint(WorkItemTrackingHttpClient witClient, int workItemId, string iterationPath, string state)
    {
        var patchDocument = new JsonPatchDocument
        {
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.IterationPath",
                Value = iterationPath
            }
        };

        await witClient.UpdateWorkItemAsync(patchDocument, workItemId);
    }

    static async Task CreateSprintsWithDates(VssConnection connection, string projectName)
    {
        try
        {
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            
            Console.WriteLine("Setting up sprints with historical dates...");
            
            var today = DateTime.Now;
            
            var sprints = new[]
            {
                new { Name = "Sprint 1", StartDate = today.AddDays(-84), EndDate = today.AddDays(-71) },  // 12 weeks ago
                new { Name = "Sprint 2", StartDate = today.AddDays(-70), EndDate = today.AddDays(-57) },  // 10 weeks ago
                new { Name = "Sprint 3", StartDate = today.AddDays(-56), EndDate = today.AddDays(-43) },  // 8 weeks ago
                new { Name = "Sprint 4", StartDate = today.AddDays(-7), EndDate = today.AddDays(7) },     // Current sprint
                new { Name = "Sprint 5", StartDate = today.AddDays(8), EndDate = today.AddDays(22) }      // Future sprint
            };
            
            foreach (var sprint in sprints)
            {
                try
                {
                    // First, try to get the existing iteration
                    WorkItemClassificationNode? existingIteration = null;
                    try
                    {
                        existingIteration = await witClient.GetClassificationNodeAsync(projectName, TreeStructureGroup.Iterations, sprint.Name);
                    }
                    catch
                    {
                        // Doesn't exist yet, we'll create it
                    }

                    var iteration = new WorkItemClassificationNode
                    {
                        Name = sprint.Name,
                        StructureType = TreeNodeStructureType.Iteration,
                        Attributes = new Dictionary<string, object>
                        {
                            { "startDate", sprint.StartDate },
                            { "finishDate", sprint.EndDate }
                        }
                    };
                    
                    if (existingIteration == null)
                    {
                        // Create new iteration
                        await witClient.CreateOrUpdateClassificationNodeAsync(iteration, projectName, TreeStructureGroup.Iterations);
                        // Now update it with dates
                        await witClient.UpdateClassificationNodeAsync(iteration, projectName, TreeStructureGroup.Iterations, sprint.Name);
                        Console.WriteLine($"  ✓ Created {sprint.Name}: {sprint.StartDate:MMM dd} - {sprint.EndDate:MMM dd, yyyy}");
                    }
                    else
                    {
                        // Update existing iteration with dates
                        await witClient.UpdateClassificationNodeAsync(iteration, projectName, TreeStructureGroup.Iterations, sprint.Name);
                        Console.WriteLine($"  ✓ Updated {sprint.Name}: {sprint.StartDate:MMM dd} - {sprint.EndDate:MMM dd, yyyy}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ! Could not create/update {sprint.Name}: {ex.Message}");
                }
            }
            
            Console.WriteLine("Sprint setup complete!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not create sprints (may need Project Collection Administrator permissions): {ex.Message}\n");
            Console.WriteLine("Continuing with work item creation...\n");
        }
    }

    static string GetSprintForState(string projectName, string state, Random random)
    {
        // Assign to sprints based on state to simulate project history
        // Sprint naming: ProjectName\Sprint X
        
        if (state == "Done" || state == "Closed" || state == "Resolved")
        {
            // Completed items go to old sprints
            var sprintNum = random.Next(1, 4); // Sprint 1-3
            return $"{projectName}\\Sprint {sprintNum}";
        }
        else if (state == "Committed" || state == "Active" || state == "In Progress" || state == "Doing")
        {
            // In progress items go to current sprint
            return $"{projectName}\\Sprint 4";
        }
        else
        {
            // New/backlog items have no sprint (return base iteration)
            return projectName;
        }
    }
}

public class Configuration
{
    public string OrganizationUrl { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public string PersonalAccessToken { get; set; } = "";
    public string ProcessTemplate { get; set; } = "Scrum"; // Default to Scrum
    public bool UseSprintHistory { get; set; } = false;
    public List<TeamConfig> Teams { get; set; } = new();
}

public class TeamConfig
{
    public string Name { get; set; } = "";
    public string AreaPath { get; set; } = "";
    public string IterationPath { get; set; } = "";
}
