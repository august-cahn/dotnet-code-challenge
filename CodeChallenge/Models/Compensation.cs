using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeChallenge.Models
{
    /// <summary>
    /// Stores the compensation details for one employee.
    /// </summary>
    public class Compensation
    {
        [Required]
        public Employee Employee { get; set; }

        [Key]
        [JsonIgnore]
        public string EmployeeId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        [Required]
        public DateTime? EffectiveDate { get; set; }
    }
}