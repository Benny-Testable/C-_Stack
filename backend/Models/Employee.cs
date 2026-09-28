namespace NineBlock.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
