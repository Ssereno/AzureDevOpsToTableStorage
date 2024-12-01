# Introduction
The goal of this project is synchronize a set of **AzureDevOps** workitems (*User Stories, Task, Areas, Iterations, Features, Bugs*) from multiple Azure DevOps projects in to TableStorage to enable centralized analysis of multiple projects using PowerBI dashboards.

## Architecture

### Basic Flow Diagram
![Azure](/AzureDevOpsToPoweBI/Diagrams/Overview.jpg)

### Application settings
There are a set of basic parameters that need to be supplied to the application for it to work:

```C#
internal static string TfsUri { 
    get {return "Azure DevOps URL";}
}

public static string PersonalAccessToken { 
    get {return "PAT from Azure DevOps";}
}

public static string AzureStorageConnectionString { 
    get {return "Azure Table connection string"; }
}
```

### Project Metadata
To specify the projects to be synchronized it is necessary (*at least for now*) to indicate the Project Name; Area Path and Team Name. 

```C#
 /// <summary>
/// Main Class.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Start Sincronization....");

        // List of projects to get data.
        // Project Name; Area Path, Team Name
        var appsProjects = new List<Tuple<string,string,string>>{
                Tuple.Create("Project_A","Project_A\\Apollo","Project A Team"),
        };
```
