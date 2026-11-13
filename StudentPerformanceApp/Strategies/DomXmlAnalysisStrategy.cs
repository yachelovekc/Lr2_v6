using System.Xml;
using StudentPerformanceApp.Models;

namespace StudentPerformanceApp.Strategies;

/// <summary>
/// Стратегія аналізу XML за допомогою DOM API (XmlDocument) та динамічної генерації XPath-запитів.
/// </summary>
public class DomXmlAnalysisStrategy : IXmlAnalysisStrategy
{
    public string StrategyName => "DOM API (XmlDocument)";

    public List<StudentRecord> Search(string xmlFilePath, StudentSearchCriteria criteria)
    {
        var results = new List<StudentRecord>();
        var doc = new XmlDocument();
        doc.Load(xmlFilePath);

        string xpath = BuildDynamicXPath(criteria);
        XmlNodeList? studentNodes = doc.SelectNodes(xpath);

        if (studentNodes == null)
            return results;

        foreach (XmlNode node in studentNodes)
        {
            if (node.Attributes == null)
                continue;

            var student = new StudentRecord
            {
                AttributeCount = node.Attributes.Count,
                Id = node.Attributes["Id"]?.Value ?? string.Empty,
                FullName = node.Attributes["FullName"]?.Value ?? string.Empty,
                Faculty = node.Attributes["Faculty"]?.Value ?? string.Empty,
                Department = node.Attributes["Department"]?.Value ?? string.Empty,
                Course = node.Attributes["Course"]?.Value ?? string.Empty,
                Group = node.Attributes["Group"]?.Value ?? string.Empty,
                Specialty = node.Attributes["Specialty"]?.Value,
                StudyForm = node.Attributes["StudyForm"]?.Value,
                Scholarship = node.Attributes["Scholarship"]?.Value
            };

            XmlNodeList? disciplineNodes = node.SelectNodes("Discipline");
            if (disciplineNodes != null)
            {
                foreach (XmlNode dNode in disciplineNodes)
                {
                    if (dNode.Attributes == null)
                        continue;

                    int.TryParse(dNode.Attributes["Grade"]?.Value, out int grade);
                    student.Disciplines.Add(new DisciplineRecord
                    {
                        Name = dNode.Attributes["Name"]?.Value ?? string.Empty,
                        Semester = dNode.Attributes["Semester"]?.Value ?? string.Empty,
                        Credits = dNode.Attributes["Credits"]?.Value ?? string.Empty,
                        Grade = grade,
                        NationalGrade = dNode.Attributes["NationalGrade"]?.Value ?? string.Empty,
                        ExamType = dNode.Attributes["ExamType"]?.Value ?? string.Empty,
                        Lecturer = dNode.Attributes["Lecturer"]?.Value ?? string.Empty
                    });
                }
            }

            // Додаткова перевірка ключових слів без урахування регістру
            if (criteria.Matches(student))
            {
                results.Add(student);
            }
        }

        return results;
    }

    /// <summary>
    /// Динамічна генерація XPath-запиту на основі непорожніх параметрів пошуку.
    /// </summary>
    public static string BuildDynamicXPath(StudentSearchCriteria criteria)
    {
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(criteria.Faculty))
            conditions.Add($"@Faculty = {EscapeXPathLiteral(criteria.Faculty)}");

        if (!string.IsNullOrWhiteSpace(criteria.Department))
            conditions.Add($"@Department = {EscapeXPathLiteral(criteria.Department)}");

        if (!string.IsNullOrWhiteSpace(criteria.Course))
            conditions.Add($"@Course = {EscapeXPathLiteral(criteria.Course)}");

        if (!string.IsNullOrWhiteSpace(criteria.Group))
            conditions.Add($"@Group = {EscapeXPathLiteral(criteria.Group)}");

        if (!string.IsNullOrWhiteSpace(criteria.StudyForm))
            conditions.Add($"@StudyForm = {EscapeXPathLiteral(criteria.StudyForm)}");

        if (!string.IsNullOrWhiteSpace(criteria.Scholarship))
            conditions.Add($"@Scholarship = {EscapeXPathLiteral(criteria.Scholarship)}");

        if (!string.IsNullOrWhiteSpace(criteria.DisciplineName))
            conditions.Add($"Discipline[@Name = {EscapeXPathLiteral(criteria.DisciplineName)}]");

        if (!string.IsNullOrWhiteSpace(criteria.NationalGrade))
            conditions.Add($"Discipline[@NationalGrade = {EscapeXPathLiteral(criteria.NationalGrade)}]");

        if (criteria.MinGrade.HasValue && criteria.MinGrade.Value > 0)
            conditions.Add($"Discipline[number(@Grade) >= {criteria.MinGrade.Value}]");

        if (conditions.Count == 0)
            return "/StudentPerformanceSystem/Student";

        return $"/StudentPerformanceSystem/Student[{string.Join(" and ", conditions)}]";
    }

    private static string EscapeXPathLiteral(string value)
    {
        if (!value.Contains('\''))
            return $"'{value}'";
        if (!value.Contains('"'))
            return $"\"{value}\"";
        return "concat('" + value.Replace("'", "', \"'\", '") + "')";
    }
}
