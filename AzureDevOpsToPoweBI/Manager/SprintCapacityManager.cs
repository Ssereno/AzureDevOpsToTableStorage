using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsToPowerBI
{
    internal static class SprintCapacityManager
    {
        internal static async Task GetCapacity(string projectName, string teamName)
        {
            try
            {
                var connection = new VssConnection(new Uri(AppSettings.TfsUri), new VssBasicCredential(string.Empty, AppSettings.PersonalAccessToken));
                var workClient = connection.GetClient<WorkHttpClient>();

                // Get all the team's iteration paths
                var teamContext = new TeamContext(projectName, teamName);
                var iterations = await workClient.GetTeamIterationsAsync(teamContext);

                foreach (var iteration in iterations)
                {
                    Console.WriteLine($"Iteration: {iteration.Name} {iteration.Id}");
                    var capacities = await workClient.GetCapacitiesWithIdentityRefAsync(teamContext, iteration.Id);

                    double totalCapacity = 0;

                    foreach (var member in capacities)
                    {
                        var teamSettings = await workClient.GetTeamSettingsAsync(teamContext);
                        double memberCapacity = member.Activities.Sum(a => a.CapacityPerDay) * 5;
                        totalCapacity += memberCapacity;
                        Console.WriteLine($"{member.TeamMember.DisplayName}: {memberCapacity} hours");
                    }

                    Console.WriteLine($"Total Capacity for {iteration.Name}: {totalCapacity} hours\n");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        internal static async Task<double> GetSprintCapacityById(string projectName, string teamName, string iterationId)
        {
            try
            {
                var connection = new VssConnection(new Uri(AppSettings.TfsUri), new VssBasicCredential(string.Empty, AppSettings.PersonalAccessToken));
                var workClient = connection.GetClient<WorkHttpClient>();

                // Set up team context
                var teamContext = new TeamContext(projectName, teamName);

                // Get the iteration by ID
                var iteration = await workClient.GetTeamIterationAsync(teamContext, new Guid(iterationId));

                if (iteration != null)
                {
                    Console.WriteLine($"Iteration: {iteration.Name}");
                    var capacities = await workClient.GetCapacitiesWithIdentityRefAsync(teamContext, iteration.Id);

                    double totalCapacity = 0;

                    foreach (var member in capacities)
                    {
                        var teamSettings = await workClient.GetTeamSettingsAsync(teamContext);
                        double memberCapacity = member.Activities.Sum(a => a.CapacityPerDay) * 5;
                        totalCapacity += memberCapacity;
                        Console.WriteLine($"{member.TeamMember.DisplayName}: {memberCapacity} hours");
                    }

                    Console.WriteLine($"Total Capacity for {iteration.Name}: {totalCapacity} hours\n");
                    return totalCapacity;

                }
                else
                {
                    Console.WriteLine("Iteration not found.");
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return 0;
            }

        }
    }   
}
