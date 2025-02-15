# Introduction
The goal of this project is synchronize a set of **AzureDevOps** workitems (*User Stories, Task, Areas, Iterations, Features, Bugs*) from multiple Azure DevOps projects in to TableStorage to enable centralized analysis of multiple projects using PowerBI dashboards.

## Architecture

### Basic Flow Diagram
![Azure](/AzureDevOpsToPoweBI/Diagrams/Overview.jpg)

## Application settings (Before Run)
There are a set of basic parameters that need to be supplied to the application for it to work.
In the **app.config** you can set the following settings

### Connection Settings Section

| Property | Description |
|----------|------------ |
|TfsUri | The azure devops url. |
|PersonalAccessToken| The azure devops personal access token with only basic permissions |
|AzureStorageConnectionString| The azure table connection string. |
|WorkItemSyncDate| Date from which you want to synchronize the “User Stories and Task” workitems.|

### Project Settings Section

| Property | Description |
|----------|------------ |
|ProjectName | The project name. You can get this information you project url. |
|AreaPath| The work itens area path. You can get this from the settings in you project |
|TeamName| The team name. You can get this from the security in you project  |


## Supported Work Itens

This project currently allows you to synchronize Areas, Iterations, Tasks and User Stories. Check the class diagram for more details.

![Class Diagram](/AzureDevOpsToPoweBI/Diagrams/EntityClassDiagram.png)