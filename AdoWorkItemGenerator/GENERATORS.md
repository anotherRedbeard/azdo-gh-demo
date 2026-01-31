# Process Template Generators

This application supports all Azure DevOps process templates with dedicated generators for each.

## Architecture

```
BaseWorkItemGenerator (Abstract Base Class)
├── ScrumWorkItemGenerator
├── AgileWorkItemGenerator
├── BasicWorkItemGenerator
└── CmmiWorkItemGenerator
```

## Base Class

`BaseWorkItemGenerator` provides common functionality:
- Random number generation for variety
- Default backlog item generation
- Abstract methods for template-specific implementations
- State validation arrays

## Generator Classes

### ScrumWorkItemGenerator
- **Work Item Types**: Epic, Feature, Product Backlog Item, Task
- **Scenarios**: E-commerce platform, microservices migration
- **States**: 
  - Epic: New, In Progress, Resolved, Removed
  - Feature: New, In Progress, Removed
  - PBI: New, Approved, Committed, Done, Removed
  - Task: To Do, In Progress, Done, Removed

### AgileWorkItemGenerator
- **Work Item Types**: Epic, Feature, User Story, Task
- **Scenarios**: Customer experience, cloud migration, performance optimization
- **States**:
  - Epic/Feature: New, Active, Resolved, Closed
  - User Story: New, Active, Resolved, Closed, Removed
  - Task: To Do, In Progress, Done

### BasicWorkItemGenerator
- **Work Item Types**: Epic, Issue, Task (simplified)
- **Scenarios**: Website redesign, API development, infrastructure setup
- **States**: All items use To Do, Doing, Done

### CmmiWorkItemGenerator
- **Work Item Types**: Epic, Feature, Requirement, Task
- **Scenarios**: Compliance, quality assurance, risk management, audit trails
- **States**: All items use Proposed, Active, Resolved, Closed
- **Special Features**: 
  - Requirement IDs (REQ-001, REQ-002, etc.)
  - Formal process-oriented descriptions
  - Additional task types (requirements analysis, validation, etc.)

## How to Extend

To add custom scenarios to a generator:

1. Open the appropriate generator file
2. Locate `GetEpicsForTeam()` method
3. Add new epics to the returned list
4. Create corresponding features in `GetFeaturesForEpic()`
5. Add backlog items in `GetBacklogItemsForFeature()`

Example:
```csharp
public override List<EpicData> GetEpicsForTeam(string teamName)
{
    if (teamName == "Frontend")
    {
        return new List<EpicData>
        {
            new EpicData 
            { 
                Title = "Your New Epic",
                Description = "Epic description",
                State = "New" 
            }
        };
    }
}
```

## Automatic Work Item Type Detection

The program automatically uses the correct work item type based on the selected template:
- Scrum → "Product Backlog Item"
- Agile → "User Story"  
- Basic → "Issue"
- CMMI → "Requirement"

This is handled by `GetBacklogItemTypeName()` method in each generator.
