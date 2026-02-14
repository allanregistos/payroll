namespace PayrollWeb.Models;

public class DepartmentDto
{
    public int DepartmentId { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PositionDto
{
    public int PositionId { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
}
