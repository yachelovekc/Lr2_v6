namespace StudentPerformanceApp.Models;

/// <summary>
/// Критерії динамічної генерації запиту та пошуку за ключовими словами.
/// </summary>
public class StudentSearchCriteria
{
    /// <summary>
    /// Пошук за ключовим словом у П.І.П., назві дисципліни або викладачі.
    /// </summary>
    public string? Keyword { get; set; }

    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public string? Course { get; set; }
    public string? Group { get; set; }
    public string? StudyForm { get; set; }
    public string? Scholarship { get; set; }
    public string? DisciplineName { get; set; }
    public string? NationalGrade { get; set; }
    public int? MinGrade { get; set; }

    /// <summary>
    /// Перевіряє, чи відповідає запис студента заданим критеріям пошуку.
    /// </summary>
    public bool Matches(StudentRecord student)
    {
        if (!string.IsNullOrWhiteSpace(Faculty) &&
            !string.Equals(student.Faculty, Faculty, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(Department) &&
            !string.Equals(student.Department, Department, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(Course) &&
            !string.Equals(student.Course, Course, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(Group) &&
            !string.Equals(student.Group, Group, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(StudyForm) &&
            !string.Equals(student.StudyForm ?? string.Empty, StudyForm, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(Scholarship) &&
            !string.Equals(student.Scholarship ?? string.Empty, Scholarship, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(DisciplineName) &&
            !student.Disciplines.Any(d => string.Equals(d.Name, DisciplineName, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (!string.IsNullOrWhiteSpace(NationalGrade) &&
            !student.Disciplines.Any(d => string.Equals(d.NationalGrade, NationalGrade, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (MinGrade.HasValue && MinGrade.Value > 0 &&
            !student.Disciplines.Any(d => d.Grade >= MinGrade.Value))
            return false;

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            string kw = Keyword.Trim();
            bool inStudent =
                student.FullName.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                student.Id.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                student.Faculty.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                student.Department.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                (student.Specialty?.Contains(kw, StringComparison.OrdinalIgnoreCase) ?? false);

            bool inDisciplines = student.Disciplines.Any(d =>
                d.Name.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                d.Lecturer.Contains(kw, StringComparison.OrdinalIgnoreCase));

            if (!inStudent && !inDisciplines)
                return false;
        }

        return true;
    }
}
