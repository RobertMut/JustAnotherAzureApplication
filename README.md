ASP.NET 6 API to store and create miniatures images and share them between users and groups.

## Technologies
- ASP.NET Core
- EntityFramework Core
- MediatR
- AutoMapper
- Moq
- Specflow
- Azure Functions
- Azure Storage
- NUnit
- FluentAssertions
- FluentValidation

## Azure run
1. Clone the repository
2. Create Azure Storage
3. Create Azure Database
4. Create Azure Functions
5. Create Azure Web App
6. Create KeyVault
7. Publish Function as Azure Function
8. Publish API as Web App
9. Add ConnectionStrings to environmental variables and/or to KeyVault
10. Run WebApp

## Local run
1. Clone the repository
2. Install Node
3. [Install Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio)
4. [Install Microsoft Sql Server](https://learn.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server?view=sql-server-ver16) it can be [Developer or Express version](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
5. [Install Azure Functions Core Tool](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=windows%2Cazure-cli#install-the-azure-functions-core-tools)
6. Fill connection strings in appsettings
    ```json
        {
          "KeyVault": "KEYVAULTADDRESS",
          "ConnectionStrings": {
            "JAAADatabase": "DATABASECONNECTIONSTRING"
          },
          "AzureWebJobsStorage": "STORAGECONNECTIONSTRING"
        }
    ```
7. Create [local.settings.json](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-local#local-settings-file) file under Src/Functions
8. Restore packages
    ```
    dotnet restore
    ```
9. Build solution
    ```
    dotnet build   
    ```
10. Start solution
    ```
    dotnet run
    ```
11. Navigate to Src/Functions and run function
    ```
    func start
    ```

## Default data
User: \
&emsp;Username: Default\
&emsp;Password: 123456\
Group: Everyone

## Architecture
![](https://i.imgur.com/0dVtvs2.png)
