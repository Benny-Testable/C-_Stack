namespace NineBlock.Api.ComplianceSamples;

/// <summary>
/// Consistent PascalCase/camelCase naming, no unused variables, no unreachable
/// code, consistent indentation and bracing — zero lint findings of any severity.
/// Satisfies the Lint / Rule Violations metric group in full.
/// </summary>
public class EmployeeSummary
{
    public string DisplayName { get; }
    public string Department { get; }

    public EmployeeSummary(string displayName, string department)
    {
        DisplayName = displayName;
        Department = department;
    }

    public override string ToString() => $"{DisplayName} ({Department})";
}
