namespace StudentPerformanceApp.Models;

/// <summary>
/// Модель даних про навчальну дисципліну та успішність студента (2-й рівень XML).
/// </summary>
public class DisciplineRecord
{
    public string Name { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public string Credits { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string NationalGrade { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public string Lecturer { get; set; } = string.Empty;
}
