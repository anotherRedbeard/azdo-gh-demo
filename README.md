# Azure DevOps & GitHub Demo

[![Board Status](https://dev.azure.com/FDPO-25-ORG/6b5fa99b-5439-47b8-b67d-dcdf5bf4971e/13eeaf2c-01e0-403f-825d-35f618c1aa04/_apis/work/boardbadge/8550ffc9-6d0c-4c3b-b8f3-4396757f82c8)](https://dev.azure.com/FDPO-25-ORG/6b5fa99b-5439-47b8-b67d-dcdf5bf4971e/_boards/board/t/13eeaf2c-01e0-403f-825d-35f618c1aa04/Microsoft.RequirementCategory)
![GitHub](https://img.shields.io/github/license/anotherRedbeard/azdo-gh-demo)

This repository demonstrates integration between Azure DevOps and GitHub, showcasing work item synchronization and automated workflows.

## Repository Structure

```
azdo-gh-demo/
├── WorkItemGenerator/          # Azure DevOps work item generator tool
│   └── AdoWorkItemGenerator/   # .NET console application
│       ├── WorkItemGenerators/ # Process template generators (Scrum, Agile, CMMI, Basic)
│       ├── Program.cs          # Main application entry point
│       ├── README.md           # Detailed usage instructions
│       ├── GENERATORS.md       # Generator customization guide
│       └── appsettings.example.json  # Configuration template
├── Backend/                    # Backend application (coming soon)
├── Frontend/                   # Frontend application (coming soon)
├── azdo-gh-demo.sln            # Visual Studio solution file
└── LICENSE                     # MIT License
```

## Components

### WorkItemGenerator

A .NET console application that generates realistic demo data for Azure DevOps projects. Supports all process templates: **Scrum**, **Agile**, **Basic**, and **CMMI**.

**Key Features:**
- Generate complete backlogs with epics, features, and work items
- Support for all Azure DevOps process templates
- Configurable team areas and iterations
- Bulk cleanup functionality

**Quick Start:**
```bash
cd WorkItemGenerator/AdoWorkItemGenerator
dotnet run
```

See [WorkItemGenerator/AdoWorkItemGenerator/README.md](WorkItemGenerator/AdoWorkItemGenerator/README.md) for detailed instructions and [GENERATORS.md](WorkItemGenerator/AdoWorkItemGenerator/GENERATORS.md) for customization options.

### Backend

Backend application (coming soon). See [Backend/README.md](Backend/README.md) for updates.

### Frontend

Frontend application (coming soon). See [Frontend/README.md](Frontend/README.md) for updates.

## Development

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (for WorkItemGenerator)
- Visual Studio or VS Code (optional)

### Building the Solution
```bash
dotnet build azdo-gh-demo.sln
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.