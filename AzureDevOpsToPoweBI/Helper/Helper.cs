namespace AzureDevOpsToPowerBI
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DateTimeHelper
    {
        internal static string GetEffectiveCompletionDate(string completedDateStr, string closedDateStr)
        {
            bool completedParsed = DateTime.TryParse(completedDateStr, out DateTime completedDate);
            bool closedParsed = DateTime.TryParse(closedDateStr, out DateTime closedDate);

            if (completedParsed && closedParsed)
            {
                return (completedDate < closedDate ? completedDate : closedDate).ToString("yyyy-MM-dd");
            }

            if (completedParsed) return completedDate.ToString("yyyy-MM-dd");
            if (closedParsed) return closedDate.ToString("yyyy-MM-dd");

            return string.Empty;
        }

        public static string GetEffectiveResolutionDate(string completedDateStr, string closedDateStr, string resolvedDateStr)
        {
            bool completedParsed = DateTime.TryParse(completedDateStr, out DateTime completedDate);
            bool closedParsed = DateTime.TryParse(closedDateStr, out DateTime closedDate);
            bool resolvedParsed = DateTime.TryParse(resolvedDateStr, out DateTime resolvedDate);

            if (completedParsed && closedParsed && resolvedParsed)
            {
                if (closedDate == resolvedDate && closedDate > completedDate && resolvedDate > completedDate)
                {
                    return completedDate.ToString("yyyy-MM-dd");
                }

                return resolvedDate.ToString("yyyy-MM-dd");
            }

            if (resolvedParsed) return resolvedDate.ToString("yyyy-MM-dd");

            return string.Empty;
        }

        public static string GetEffectiveActivatedDate(string createdDateStr, string activatedDateStr)
        {
            bool createdParsed = DateTime.TryParse(createdDateStr, out DateTime createdDate);
            bool activatedParsed = DateTime.TryParse(activatedDateStr, out DateTime activatedDate);

            if (createdParsed && activatedParsed)
            {
                if (activatedDate > createdDate)
                {
                    return createdDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    return activatedDate.ToString("yyyy-MM-dd");
                }
            }

            if (createdParsed) return createdDate.ToString("yyyy-MM-dd");
            if (activatedParsed) return activatedDate.ToString("yyyy-MM-dd");

            return string.Empty;
        }

    }
}