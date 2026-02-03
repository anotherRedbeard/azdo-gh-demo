# Azure DevOps Work Item Generator

Generate realistic demo data for Azure DevOps projects. Supports all process templates: **Scrum**, **Agile**, **Basic**, and **CMMI**.

## Quick Start

1. **Get a PAT token** from Azure DevOps (User Settings → Personal Access Tokens)
   - Scope: Work Items (Read, write, & manage)

2. **Edit appsettings.json**:
```json
{
  "AzureDevOps": {
    "OrganizationUrl": "https://dev.azure.com/YourOrg",
    "ProjectName": "YourProject",
    "PersonalAccessToken": "your-pat-here",
    "ProcessTemplate": "Scrum"
  },
  "Teams": [
    {
      "Name": "Frontend",
      "AreaPath": "YourProject\\Frontend"
    }
  ]
}
```

3. **Run**:
```bash
dotnet run
```

## Process Templates

| Template | Backlog Item Type |
|----------|-------------------|
| Scrum | Product Backlog Item |
| Agile | User Story |
| Basic | Issue |
| CMMI | Requirement |

## Menu Options

1. **Create sample** - One epic with features, backlog items, and tasks
2. **Generate full backlog** - 100+ work items for demo
3. **Delete all** - Remove all work items from configured teams
4. **Exit**

## Customization

Edit generators in `WorkItemGenerators/` folder to customize epics, features, and backlog items for each process template.

## Cleanup

```bash
dotnet run cleanup  # or choose option 3 from menu
```

Deleted items go to Recycle Bin (recoverable for 30 days).

## Troubleshooting

- **Authentication errors**: Check PAT token and permissions
- **Area path errors**: Create areas in Project Settings → Areas
- **Wrong process template**: Update `ProcessTemplate` in appsettings.json to match your project
