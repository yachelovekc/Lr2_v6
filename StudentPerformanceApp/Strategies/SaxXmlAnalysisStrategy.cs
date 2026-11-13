using System.Xml;
using StudentPerformanceApp.Models;

namespace StudentPerformanceApp.Strategies;

/// <summary>
/// Стратегія аналізу XML за допомогою потокового читання SAX API (XmlReader).
/// Не завантажує весь документ у пам'ять, послідовно обходить вузли.
/// </summary>
public class SaxXmlAnalysisStrategy : IXmlAnalysisStrategy
{
    public string StrategyName => "SAX API (XmlReader)";

    public List<StudentRecord> Search(string xmlFilePath, StudentSearchCriteria criteria)
    {
        var results = new List<StudentRecord>();
        StudentRecord? currentStudent = null;

        var settings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        using var reader = XmlReader.Create(xmlFilePath, settings);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "Student")
                {
                    currentStudent = new StudentRecord
                    {
                        AttributeCount = reader.AttributeCount,
                        Id = reader.GetAttribute("Id") ?? string.Empty,
                        FullName = reader.GetAttribute("FullName") ?? string.Empty,
                        Faculty = reader.GetAttribute("Faculty") ?? string.Empty,
                        Department = reader.GetAttribute("Department") ?? string.Empty,
                        Course = reader.GetAttribute("Course") ?? string.Empty,
                        Group = reader.GetAttribute("Group") ?? string.Empty,
                        Specialty = reader.GetAttribute("Specialty"),
                        StudyForm = reader.GetAttribute("StudyForm"),
                        Scholarship = reader.GetAttribute("Scholarship")
                    };

                    if (reader.IsEmptyElement)
                    {
                        if (criteria.Matches(currentStudent))
                            results.Add(currentStudent);
                        currentStudent = null;
                    }
                }
                else if (reader.Name == "Discipline" && currentStudent != null)
                {
                    int.TryParse(reader.GetAttribute("Grade"), out int grade);
                    var discipline = new DisciplineRecord
                    {
                        Name = reader.GetAttribute("Name") ?? string.Empty,
                        Semester = reader.GetAttribute("Semester") ?? string.Empty,
                        Credits = reader.GetAttribute("Credits") ?? string.Empty,
                        Grade = grade,
                        NationalGrade = reader.GetAttribute("NationalGrade") ?? string.Empty,
                        ExamType = reader.GetAttribute("ExamType") ?? string.Empty,
                        Lecturer = reader.GetAttribute("Lecturer") ?? string.Empty
                    };
                    currentStudent.Disciplines.Add(discipline);
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student")
            {
                if (currentStudent != null && criteria.Matches(currentStudent))
                {
                    results.Add(currentStudent);
                }
                currentStudent = null;
            }
        }

        return results;
    }
}
