namespace StudentPerformanceApp.Models;

/// <summary>
/// Модель даних студента (1-й рівень XML).
/// Підтримує змінну кількість атрибутів у головному вузлі Student.
/// </summary>
public class StudentRecord
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;

    // Необов'язкові атрибути (забезпечують різну кількість атрибутів у вузлів Student)
    public string? Specialty { get; set; }
    public string? StudyForm { get; set; }
    public string? Scholarship { get; set; }

    public int AttributeCount { get; set; }

    public List<DisciplineRecord> Disciplines { get; set; } = new();

    public double AverageGrade => Disciplines.Count > 0
        ? Math.Round(Disciplines.Average(d => d.Grade), 2)
        : 0.0;
}
