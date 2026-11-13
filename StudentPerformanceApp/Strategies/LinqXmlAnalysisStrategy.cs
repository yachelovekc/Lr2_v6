using System.Xml.Linq;
using StudentPerformanceApp.Models;

namespace StudentPerformanceApp.Strategies;

/// <summary>
/// Стратегія аналізу XML за допомогою LINQ to XML (XDocument).
/// Використовує декларативні LINQ-запити з динамічною фільтрацією.
/// </summary>
public class LinqXmlAnalysisStrategy : IXmlAnalysisStrategy
{
    public string StrategyName => "LINQ to XML (XDocument)";

    public List<StudentRecord> Search(string xmlFilePath, StudentSearchCriteria criteria)
    {
        XDocument xdoc = XDocument.Load(xmlFilePath);

        IEnumerable<XElement> query = xdoc.Root?.Elements("Student") ?? Enumerable.Empty<XElement>();

        // Динамічна побудова ланцюжка запитів LINQ
        if (!string.IsNullOrWhiteSpace(criteria.Faculty))
            query = query.Where(el => string.Equals((string?)el.Attribute("Faculty"), criteria.Faculty, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.Department))
            query = query.Where(el => string.Equals((string?)el.Attribute("Department"), criteria.Department, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.Course))
            query = query.Where(el => string.Equals((string?)el.Attribute("Course"), criteria.Course, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.Group))
            query = query.Where(el => string.Equals((string?)el.Attribute("Group"), criteria.Group, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.StudyForm))
            query = query.Where(el => string.Equals((string?)el.Attribute("StudyForm"), criteria.StudyForm, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.Scholarship))
            query = query.Where(el => string.Equals((string?)el.Attribute("Scholarship"), criteria.Scholarship, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(criteria.DisciplineName))
            query = query.Where(el => el.Elements("Discipline").Any(d =>
                string.Equals((string?)d.Attribute("Name"), criteria.DisciplineName, StringComparison.OrdinalIgnoreCase)));

        if (!string.IsNullOrWhiteSpace(criteria.NationalGrade))
            query = query.Where(el => el.Elements("Discipline").Any(d =>
                string.Equals((string?)d.Attribute("NationalGrade"), criteria.NationalGrade, StringComparison.OrdinalIgnoreCase)));

        if (criteria.MinGrade.HasValue && criteria.MinGrade.Value > 0)
            query = query.Where(el => el.Elements("Discipline").Any(d =>
                int.TryParse((string?)d.Attribute("Grade"), out int g) && g >= criteria.MinGrade.Value));

        var students = query.Select(el => new StudentRecord
        {
            AttributeCount = el.Attributes().Count(),
            Id = (string?)el.Attribute("Id") ?? string.Empty,
            FullName = (string?)el.Attribute("FullName") ?? string.Empty,
            Faculty = (string?)el.Attribute("Faculty") ?? string.Empty,
            Department = (string?)el.Attribute("Department") ?? string.Empty,
            Course = (string?)el.Attribute("Course") ?? string.Empty,
            Group = (string?)el.Attribute("Group") ?? string.Empty,
            Specialty = (string?)el.Attribute("Specialty"),
            StudyForm = (string?)el.Attribute("StudyForm"),
            Scholarship = (string?)el.Attribute("Scholarship"),
            Disciplines = el.Elements("Discipline").Select(d =>
            {
                int.TryParse((string?)d.Attribute("Grade"), out int grade);
                return new DisciplineRecord
                {
                    Name = (string?)d.Attribute("Name") ?? string.Empty,
                    Semester = (string?)d.Attribute("Semester") ?? string.Empty,
                    Credits = (string?)d.Attribute("Credits") ?? string.Empty,
                    Grade = grade,
                    NationalGrade = (string?)d.Attribute("NationalGrade") ?? string.Empty,
                    ExamType = (string?)d.Attribute("ExamType") ?? string.Empty,
                    Lecturer = (string?)d.Attribute("Lecturer") ?? string.Empty
                };
            }).ToList()
        }).Where(s => criteria.Matches(s)).ToList();

        return students;
    }
}
