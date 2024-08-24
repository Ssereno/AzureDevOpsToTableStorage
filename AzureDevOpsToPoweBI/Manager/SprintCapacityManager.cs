using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsToPowerBI
{
    internal static class SprintCapacityManager
    {
        internal static async Task<List<SprintCapacity>> GetCapacityAsync(string projectName, string teamName)
        {
            try
            {
                var connection = new VssConnection(new Uri(AppSettings.TfsUri), new VssBasicCredential(string.Empty, AppSettings.PersonalAccessToken));
                var workClient = connection.GetClient<WorkHttpClient>();

                // Get all the team's iteration paths
                var teamContext = new TeamContext(projectName, teamName);
                var iterations = await workClient.GetTeamIterationsAsync(teamContext);

                var SprintCapacity = new List<SprintCapacity>();

                foreach (var iteration in iterations)
                {
                    Console.WriteLine($"Iteration: {iteration.Name} {iteration.Id}");

                    // Get the team settings days off (holidays, etc.)
                    var teamDaysOff = await workClient.GetTeamDaysOffAsync(teamContext, iteration.Id);

                    var capacities = await workClient.GetCapacitiesWithIdentityRefAsync(teamContext, iteration.Id);

                    double totalCapacity = 0;

                    foreach (var member in capacities)
                    {
                        // Calculate the number of workdays in the iteration
                        var startDate = iteration.Attributes.StartDate.Value;
                        var endDate = iteration.Attributes.FinishDate.Value;

                        // Consider iteration-level days off (holidays, etc.)
                        var iterationDaysOff = teamDaysOff.DaysOff.Sum(doff =>
                        {
                            var doffStart = doff.Start.Date;
                            var doffEnd = doff.End.Date;

                            if (doffStart > endDate || doffEnd < startDate)
                                return 0;

                            doffStart = doffStart < startDate ? startDate : doffStart;
                            doffEnd = doffEnd > endDate ? endDate : doffEnd;

                            return Enumerable.Range(0, (doffEnd - doffStart).Days + 1)
                                            .Select(d => doffStart.AddDays(d))
                                            .Count(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);
                        });

                        var totalWorkdays = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                                    .Select(d => startDate.AddDays(d))
                                                    .Count(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday) - iterationDaysOff;

                        // Calculate the number of days off within the iteration period for the member
                        var memberDaysOff = member.DaysOff.Sum(doff =>
                        {
                            var doffStart = doff.Start.Date;
                            var doffEnd = doff.End.Date;

                            if (doffStart > endDate || doffEnd < startDate)
                                return 0;

                            doffStart = doffStart < startDate ? startDate : doffStart;
                            doffEnd = doffEnd > endDate ? endDate : doffEnd;

                            return Enumerable.Range(0, (doffEnd - doffStart).Days + 1)
                                            .Select(d => doffStart.AddDays(d))
                                            .Count(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);
                        });

                        // Effective workdays
                        var effectiveWorkdays = totalWorkdays - memberDaysOff;

                        // Calculate member capacity
                        double memberCapacity = member.Activities.Sum(a => a.CapacityPerDay) * effectiveWorkdays;
                        totalCapacity += memberCapacity;

                        Console.WriteLine($"{member.TeamMember.DisplayName}: {memberCapacity} hours (including {memberDaysOff} member days off and {iterationDaysOff} iteration days off)");
                    }

                    SprintCapacity.Add(new SprintCapacity
                    {
                            PartitionKey = projectName,
                            RowKey = iteration.Id.ToString(),
                            ProjectName = projectName,
                            SprintName = iteration.Name,
                            IterationSK = iteration.Id.ToString(),
                            SprintPath = iteration.Path,
                            SprintHoursCapacity = totalCapacity
                    });

                    Console.WriteLine($"Total Capacity for {iteration.Name}: {totalCapacity} hours\n");
                }

                return SprintCapacity;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }
    }  
}
