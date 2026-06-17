namespace BetterCRM.Core.Extensions
{
    public enum PayrollStatus
    {
        Calculated,
        Approved,
        Paid
    }

    public enum ShiftStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }

    public enum BreakType
    {
        Lunch,
        Rest,
        Custom
    }

    public enum TicketStatus
    {
        Draft,
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }
}