# Azure Data Factory Controller (ADF Controller)

ADF Controller is console application to take input from user and perform activity on azure data factory data pipeline. It perform below operation

- Run new data pipeline
- Get status of existing pipeline

### Run new data pipeline

This operation is used to run new data pipeline on cloud.

- Input: pipeline name
- Output: Create new pipeline run and return run id

### Get status of existing pipeline

This operation is used to get status of existing pipeline run

- Input: Pipeline run id
- Output: Pipeline name, status, run start and run end

# Prerequisites

- Azure subscription
- Azure data factory pipeline/s
- Azure AD application with contributor rights to ADF resource
- Azure application insights
- .Net 6.0 framwork

# Framework & Technology

.Net 6.0 Console application

# Configuration

For azure connection, ADF Console application require below configuration in ./Configurations/appsettings.json file

### ADF Connection configuration

- ResourceGroup: Name of resoure group which hold ADF resources
- DataFactoryName: Name of data factory resource
- TenantId: Azure Tenant id
- ApplicationId: Azure AD'd application which has access to ADF resource
- AuthenticationKey: Azure AD's application authentication key
- SubscriptionId: Azure subscription id

### Application Insight configuration

- ConnectionString : Connection string of azure application insight

### Log configuration

- Console:LogLevel: Set logging level as required for console logs, Acceptable values are "Trace, Debug, Information, Warning, Error, Critical, None".
  more information about log level https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-8.0#log-level

# Artifacts

### Build project

```
dotnet build --configuration Release
```

### Publish project

```
dotnet publish --configuration Release
```

### Logging

Console logs are generated as per logging configuration.

### Application Insights

- It generates and register events on azure application insights.
- To view events please log into azure and navigate to application insights resource.

# Run Project

\*Please make sure all configuration is updated before running

- Go to publish folder from command prompt '.\AdfController\bin\Release\net6.0\publish\'
- Run below command
  ```
  .\AdfController.exe
  ```
