using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoWorkItemGenerator.Generators
{
    public class ScrumWorkItemGenerator : BaseWorkItemGenerator
    {
        public ScrumWorkItemGenerator(Configuration config) : base(config) { }

        public override string GetProcessTemplateName() => "Scrum";
        public override string GetBacklogItemTypeName() => "Product Backlog Item";
        public override string GetEffortFieldName() => "Microsoft.VSTS.Scheduling.Effort";

        protected override string[] GetValidEpicStates() => new[] { "New", "In Progress", "Done", "Removed" };
        protected override string[] GetValidFeatureStates() => new[] { "New", "In Progress", "Removed" };
        protected override string[] GetValidBacklogItemStates() => new[] { "New", "Approved", "Committed", "Done", "Removed" };
        protected override string[] GetValidTaskStates() => new[] { "To Do", "In Progress", "Done", "Removed" };

        public override List<EpicData> GetEpicsForTeam(string teamName)
        {
            if (teamName == "Frontend")
            {
                return new List<EpicData>
                {
                    new EpicData
                    {
                        Title = "[UI] Legacy System Migration",
                        Description = "Complete migration from legacy PHP system to modern React-based architecture - COMPLETED",
                        State = "Done"
                    },
                    new EpicData
                    {
                        Title = "[UI] E-Commerce Platform Modernization",
                        Description = "Modernize the entire e-commerce platform with a new frontend architecture using React and improved backend APIs",
                        State = "In Progress"
                    },
                    new EpicData
                    {
                        Title = "[UI] Mobile App Development",
                        Description = "Develop native mobile applications for iOS and Android with feature parity to web application",
                        State = "New"
                    },
                    new EpicData
                    {
                        Title = "[UI] Analytics Dashboard",
                        Description = "Build comprehensive analytics and reporting dashboard for business insights",
                        State = "New"
                    }
                };
            }
            else // Backend
            {
                return new List<EpicData>
                {
                    new EpicData
                    {
                        Title = "[API] Infrastructure Modernization",
                        Description = "Migrate from on-premise infrastructure to Azure cloud services - COMPLETED",
                        State = "Done"
                    },
                    new EpicData
                    {
                        Title = "[API] Microservices Architecture Migration",
                        Description = "Migrate monolithic backend to microservices architecture for better scalability and maintainability",
                        State = "In Progress"
                    },
                    new EpicData
                    {
                        Title = "[API] Gateway Implementation",
                        Description = "Implement API Gateway with authentication, rate limiting, and monitoring capabilities",
                        State = "New"
                    },
                    new EpicData
                    {
                        Title = "[API] Data Platform Upgrade",
                        Description = "Upgrade data platform with improved caching, database optimization, and data analytics capabilities",
                        State = "In Progress"
                    }
                };
            }
        }

        public override List<FeatureData> GetFeaturesForEpic(string teamName, string epicTitle)
        {
            if (teamName == "Frontend")
            {
                if (epicTitle.Contains("Legacy System"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[UI] Database Migration", Description = "Migrate from MySQL to PostgreSQL", State = "In Progress" },
                        new FeatureData { Title = "[UI] API Modernization", Description = "Convert REST endpoints to modern standards", State = "In Progress" },
                        new FeatureData { Title = "[UI] Component Library", Description = "Build reusable React component library", State = "In Progress" }
                    };
                }
                else if (epicTitle.Contains("E-Commerce"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[UI] Product Catalog Redesign", Description = "Redesign the product catalog interface with improved search, filtering, and responsive design", State = "In Progress" },
                        new FeatureData { Title = "[UI] Shopping Cart Enhancement", Description = "Enhance shopping cart with real-time updates, saved carts, and guest checkout", State = "New" },
                        new FeatureData { Title = "[UI] Checkout Flow Optimization", Description = "Streamline checkout process to reduce cart abandonment", State = "New" },
                        new FeatureData { Title = "[UI] User Account Dashboard", Description = "Create comprehensive user account management interface", State = "In Progress" }
                    };
                }
                else if (epicTitle.Contains("Mobile App"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[UI] Mobile Navigation System", Description = "Implement bottom tab navigation and drawer menu for mobile apps", State = "New" },
                        new FeatureData { Title = "[UI] Push Notifications", Description = "Enable push notifications for orders, promotions, and updates", State = "New" },
                        new FeatureData { Title = "[UI] Offline Mode Support", Description = "Allow basic browsing and cart management in offline mode", State = "New" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[UI] Sales Analytics Visualization", Description = "Create interactive charts and graphs for sales data", State = "New" },
                        new FeatureData { Title = "[UI] Customer Insights Dashboard", Description = "Build dashboard showing customer behavior and trends", State = "New" }
                    };
                }
            }
            else // Backend
            {
                if (epicTitle.Contains("Infrastructure Modernization"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[API] Azure Migration", Description = "Migrate all services to Azure", State = "In Progress" },
                        new FeatureData { Title = "[API] Monitoring Setup", Description = "Configure Application Insights and monitoring", State = "In Progress" },
                        new FeatureData { Title = "[API] CI/CD Pipeline", Description = "Setup automated deployment pipelines", State = "In Progress" }
                    };
                }
                else if (epicTitle.Contains("Microservices"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[API] Order Service Extraction", Description = "Extract order processing into dedicated microservice", State = "In Progress" },
                        new FeatureData { Title = "[API] Product Catalog Service", Description = "Create independent product catalog microservice with caching", State = "In Progress" },
                        new FeatureData { Title = "[API] User Service Implementation", Description = "Build user management and authentication microservice", State = "New" },
                        new FeatureData { Title = "[API] Payment Service Integration", Description = "Develop payment processing microservice with multiple provider support", State = "New" }
                    };
                }
                else if (epicTitle.Contains("Gateway"))
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[API] Authentication & Authorization", Description = "Implement JWT-based auth with OAuth2 support", State = "New" },
                        new FeatureData { Title = "[API] Rate Limiting & Throttling", Description = "Add configurable rate limiting per client/endpoint", State = "New" },
                        new FeatureData { Title = "[API] Monitoring & Logging", Description = "Implement comprehensive logging and monitoring solution", State = "New" }
                    };
                }
                else
                {
                    return new List<FeatureData>
                    {
                        new FeatureData { Title = "[API] Redis Caching Layer", Description = "Implement distributed caching with Redis", State = "In Progress" },
                        new FeatureData { Title = "[API] Database Performance Optimization", Description = "Optimize queries and add appropriate indexes", State = "In Progress" },
                        new FeatureData { Title = "[API] Data Warehouse Integration", Description = "Set up data pipeline to analytics warehouse", State = "New" }
                    };
                }
            }
        }

        public override List<BacklogItemData> GetBacklogItemsForFeature(string teamName, string featureTitle)
        {
            // Completed epic features
            if (featureTitle.Contains("Database Migration"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[UI] Setup PostgreSQL Database", Description = "Configure PostgreSQL instance in Azure", StoryPoints = 5, State = "Done" },
                    new BacklogItemData { Title = "[UI] Migrate User Tables", Description = "Migrate user and authentication tables", StoryPoints = 8, State = "Done" },
                    new BacklogItemData { Title = "[UI] Migrate Product Data", Description = "Migrate product catalog and inventory", StoryPoints = 13, State = "Done" }
                };
            }
            else if (featureTitle.Contains("API Modernization"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[UI] Standardize Response Format", Description = "Implement consistent JSON response structure", StoryPoints = 5, State = "Done" },
                    new BacklogItemData { Title = "[UI] Add API Versioning", Description = "Implement API versioning strategy", StoryPoints = 8, State = "Done" }
                };
            }
            else if (featureTitle.Contains("Component Library"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[UI] Create Button Components", Description = "Build reusable button components", StoryPoints = 3, State = "Done" },
                    new BacklogItemData { Title = "[UI] Create Form Components", Description = "Build input, select, and form controls", StoryPoints = 8, State = "Done" },
                    new BacklogItemData { Title = "[UI] Create Layout Components", Description = "Build grid, container, and layout utilities", StoryPoints = 5, State = "Done" }
                };
            }
            else if (featureTitle.Contains("Azure Migration"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[API] Provision Azure Resources", Description = "Setup resource groups and services", StoryPoints = 8, State = "Done" },
                    new BacklogItemData { Title = "[API] Migrate Application Servers", Description = "Move app servers to Azure App Service", StoryPoints = 13, State = "Done" },
                    new BacklogItemData { Title = "[API] Configure Networking", Description = "Setup VNets and security groups", StoryPoints = 5, State = "Done" }
                };
            }
            else if (featureTitle.Contains("Monitoring Setup"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[API] Configure Application Insights", Description = "Setup telemetry and logging", StoryPoints = 5, State = "Done" },
                    new BacklogItemData { Title = "[API] Create Dashboards", Description = "Build monitoring dashboards", StoryPoints = 3, State = "Done" }
                };
            }
            else if (featureTitle.Contains("CI/CD Pipeline"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[API] Setup Azure DevOps Pipeline", Description = "Configure build and release pipelines", StoryPoints = 8, State = "Done" },
                    new BacklogItemData { Title = "[API] Implement Automated Testing", Description = "Add automated tests to pipeline", StoryPoints = 5, State = "Done" }
                };
            }
            else if (teamName == "Frontend" && featureTitle.Contains("Product Catalog"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[UI] Implement Product Card Component", Description = "As a customer, I want to see product information in an attractive card format\n\nAcceptance Criteria:\n- Card displays image, name, price, rating\n- Responsive design\n- Hover effects", StoryPoints = 8, State = "Done" },
                    new BacklogItemData { Title = "[UI] Add Advanced Filtering", Description = "As a customer, I want to filter products by multiple criteria", StoryPoints = 13, State = "Committed" },
                    new BacklogItemData { Title = "[UI] Implement Search Autocomplete", Description = "As a customer, I want search suggestions as I type", StoryPoints = 5, State = "Approved" },
                    new BacklogItemData { Title = "[UI] Add Product Comparison Feature", Description = "As a customer, I want to compare multiple products side-by-side", StoryPoints = 8, State = "New" }
                };
            }
            else if (teamName == "Backend" && featureTitle.Contains("Order Service"))
            {
                return new List<BacklogItemData>
                {
                    new BacklogItemData { Title = "[API] Create Order Processing API", Description = "Implement RESTful API for order creation and management", StoryPoints = 13, State = "Committed" },
                    new BacklogItemData { Title = "[API] Order Status Tracking", Description = "Implement order status updates and notifications", StoryPoints = 8, State = "Committed" },
                    new BacklogItemData { Title = "[API] Order Validation Logic", Description = "Add comprehensive validation for order data", StoryPoints = 5, State = "Done" }
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
                ("Create component/service structure", "Set up basic structure and dependencies"),
                ("Implement core functionality", "Develop main business logic"),
                ("Add unit tests", "Write comprehensive unit tests"),
                ("Add integration tests", "Create integration test suite"),
                ("Code review and refactoring", "Review code and refactor as needed"),
                ("Update documentation", "Document API/component usage")
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
