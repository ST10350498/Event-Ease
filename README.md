CLDV6211 – Portfolio of Evidence (POE)

Student Name: Kgopotso Sereme
Student Number: ST10350498
Module: Cloud Development A (CLDV6211)
Date: 31 July 2026


GitHub Repository Link

URL: https://github.com/ST10350498/Event-Ease

Note: Code will continue to be pushed after submission. This link was submitted on time.


YouTube Video Link


Live Web App URL (Part 3)
<img width="656" height="711" alt="Screenshot 2026-08-05 110649" src="https://github.com/user-attachments/assets/f06a759d-66ce-4e8b-8118-170008beefa4" />

Deployment in progress. Screenshots attached below show current state.


Part 3A – Advanced Filtering

EventType Lookup Table

SQL script executed to create EventTypes table with categories:
- Conference
- Wedding
- Concert
- Corporate Event
- Private Party
- Exhibition
- Workshop
- Networking

Filters Implemented
- Search by Booking ID or Event Name
- Filter by Event Type (dropdown from lookup table)
- Filter by Date Range (Start Date – End Date)
- Filter by Venue

Evidence


Part 3B – Migration to Cloud

Azure SQL Database
- Database created
- Schema and data migration in progress

Azure Storage Account
- Storage account created
- Image migration from Azurite pending

Configuration Changes
- Connection strings updated for Azure


Part 3C – Live Deployment

Azure App Service
- Web App created
- Deployment pending

Dropped Resources
- Resources will be deleted after screenshots taken
- Proof attached


Part 3D – Reflective Report

Features Implemented
- Venue CRUD with image upload (Azurite → Azure Storage)
- Event CRUD
- Booking CRUD with double booking prevention
- Consolidated Booking View with search
- Advanced filtering by Event Type, Date Range, Venue (Part 3)

Azure Services Used

| Service | Purpose | Why chosen |
|---------|---------|------------|
| Azure App Service | Host web application | PaaS, no server management, easy deployment |
| Azure SQL Database | Store venue, event, booking data | Relational, ACID compliance, familiar SQL |
| Azure Storage Account | Store venue images | Scalable, cost-effective, CDN integration |

Migration Experience

Challenges faced:
1. Connection string configuration between local and Azure
2. Firewall rules for Azure SQL access
3. Image URL updates after moving to Azure Storage

Environment Separation Importance
- Development = local (LocalDB, Azurite)
- Production = cloud (Azure SQL, Azure Storage)
- Allows testing without affecting live data

Technologies Used

| Technology | Purpose |
|------------|---------|
| ASP.NET Core MVC | Web framework |
| Entity Framework Core | Database ORM |
| Bootstrap 5 | UI styling |
| Azure App Service | Cloud hosting |
| Azure SQL Database | Cloud database |
| Azure Storage | Cloud image storage |


Part 3E – Theory

Question 1: How does Cosmos DB differ from traditional databases?

| Aspect | Traditional (SQL) | Cosmos DB |
|--------|-------------------|-----------|
| Schema | Fixed, predefined | Flexible, schema-agnostic |
| Scaling | Vertical (scale up) | Horizontal (scale out) |
| Consistency | Strong ACID | Multiple levels (strong, bounded staleness, session, consistent prefix, eventual) |
| Data model | Relational (tables) | Multi-model (document, graph, key-value, column-family) |
| Global distribution | Complex, manual | Native, turn-key replication |

Cosmos DB is best for globally distributed applications with flexible schemas. Traditional SQL is better for complex transactions and reporting (Mrzyglod, 2022, Chapter 10).

Question 2: Key considerations when designing Logic Apps that handle sensitive data

1. Authentication: Use Managed Identities instead of connection strings
2. Encryption: Ensure data in transit uses HTTPS/TLS
3. Access control: Restrict who can trigger and modify Logic Apps
4. Auditing: Enable diagnostic logging to track all executions
5. Data retention: Set appropriate retention policies for logs
6. Integration environment: Use Integration Service Environment (ISE) for VNet isolation
7. Compliance: Ensure Logic Apps meet POPIA requirements for South African data

Question 3: How combining Event Grid with other services creates robust workflows

Event Grid enables reactive, event-driven architectures. Example workflow:

1. Event source: New booking created in SQL Database
2. Event Grid: Captures the "BookingCreated" event
3. Event handler: Azure Function triggered
4. Action: Send confirmation email via SendGrid, update reporting database, notify staff via Teams

Benefits:
- Decoupling: Event source unaware of handlers
- Reliability: Built-in retry and dead-lettering
- Scalability: Handles millions of events per second
- Filtering: Subscribe only to relevant events


Code Attribution

All code written for this assignment is original work by Kgopotso Sereme (ST10350498).

Sources referenced:
- Microsoft Learn – Azure Blob Storage, App Service, SQL Database documentation
- Entity Framework Core documentation
- Bootstrap 5 – UI components
- SweetAlert2 – confirmation dialogs
- Font Awesome – icons

AI assistance (The Orchestrator) was used for debugging, code structure guidance, and error resolution.


Reference List

Mrzyglod, K., 2022. Azure for Developers. 2nd ed. Birmingham: Packt Publishing.

Microsoft, 2024. Azure App Service Documentation. [online] Available at: <https://docs.microsoft.com/en-us/azure/app-service/> [Accessed 4 June 2026].

Microsoft, 2024. Azure SQL Database Documentation. [online] Available at: <https://docs.microsoft.com/en-us/azure/azure-sql/> [Accessed 4 June 2026].

Microsoft, 2024. Azure Storage Documentation. [online] Available at: <https://docs.microsoft.com/en-us/azure/storage/> [Accessed 4 June 2026].

Satzinger, J.W., Jackson, R.B. and Burd, S.D., 2016. Systems Analysis and Design in a Changing World. 7th ed. Boston: Cengage Learning.


Declaration

I declare that this assignment is my own original work. All sources used have been appropriately cited and referenced using the Harvard Anglia referencing style. This work has not been submitted for any other course or assessment.

Signature: Kgopotso Sereme
Date: 4 June 2026
