using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement {
        public int Year;
        public int Month;
        public int Day;

        public ArticleUpdateRequirement(int year = 2021, int month = 6, int day = 30)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}