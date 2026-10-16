namespace CodeChallenge.Models
{
    /// <summary>
    /// Describes an employee and the total number of employees reporting beneath them.
    /// </summary>
    public class ReportingStructure
    {
        public Employee Employee { get; set; }
        public int NumberOfReports { get; set; }
    }
}