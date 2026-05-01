namespace BetterCRM.Business.Policies
{
    public static class RoundingPolicy
    {
        public static decimal RoundDownHour(decimal hours) => hours < 0 ? 0 : Math.Floor(hours);
    }
}
