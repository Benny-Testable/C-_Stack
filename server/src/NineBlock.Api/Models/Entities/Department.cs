namespace NineBlock.Api.Models.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
