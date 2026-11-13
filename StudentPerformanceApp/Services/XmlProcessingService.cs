using System.Xml.Linq;
using System.Xml.Xsl;
using StudentPerformanceApp.Models;
using StudentPerformanceApp.Strategies;

namespace StudentPerformanceApp.Services;

/// <summary>
/// Контекст шаблону «Стратегія» (Strategy Context) та сервіс обробки і трансформації XML.
/// Повністю відповідає класу XmlProcessingService на діаграмах Class та Sequence.
/// </summary>
public class XmlProcessingService
{
    private IXmlAnalysisStrategy _strategy;

    public string CurrentStrategyName => _strategy.StrategyName;

    public XmlProcessingService(IXmlAnalysisStrategy initialStrategy)
    {
        _strategy = initialStrategy ?? throw new ArgumentNullException(nameof(initialStrategy));
    }

    public void SetStrategy(IXmlAnalysisStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public List<StudentRecord> Analyze(string xmlFilePath, StudentSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(xmlFilePath) || !File.Exists(xmlFilePath))
            throw new FileNotFoundException("Вхідний XML-файл не знайдено.", xmlFilePath);

        return _strategy.Search(xmlFilePath, criteria);
    }

    /// <summary>
    /// Підвантаження даних (UC1_Inc): зчитує всі наявні унікальні значення атрибутів із вибраного XML-файлу
    /// для заповнення випадаючих списків (Picker) у графічному інтерфейсі.
    /// </summary>
    public XmlMetadataValues ExtractAttributeValues(string xmlFilePath)
    {
        var metadata = new XmlMetadataValues();
        if (string.IsNullOrWhiteSpace(xmlFilePath) || !File.Exists(xmlFilePath))
            return metadata;

        XDocument xdoc = XDocument.Load(xmlFilePath);
        var students = xdoc.Root?.Elements("Student") ?? Enumerable.Empty<XElement>();

        metadata.Faculties = students
            .Select(s => (string?)s.Attribute("Faculty"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.Departments = students
            .Select(s => (string?)s.Attribute("Department"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.Courses = students
            .Select(s => (string?)s.Attribute("Course"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.Groups = students
            .Select(s => (string?)s.Attribute("Group"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.StudyForms = students
            .Select(s => (string?)s.Attribute("StudyForm"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.Scholarships = students
            .Select(s => (string?)s.Attribute("Scholarship"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        var disciplines = students.Elements("Discipline");

        metadata.Disciplines = disciplines
            .Select(d => (string?)d.Attribute("Name"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        metadata.NationalGrades = disciplines
            .Select(d => (string?)d.Attribute("NationalGrade"))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        return metadata;
    }

    /// <summary>
    /// Трансформація XML-документа у файл HTML за допомогою XslCompiledTransform (*.xsl).
    /// </summary>
    public void TransformToHtml(string xmlFilePath, string xslFilePath, string outputHtmlPath)
    {
        if (!File.Exists(xmlFilePath))
            throw new FileNotFoundException("Не знайдено XML-файл для трансформації.", xmlFilePath);
        if (!File.Exists(xslFilePath))
            throw new FileNotFoundException("Не знайдено XSL-файл стилів.", xslFilePath);

        var xslt = new XslCompiledTransform();
        xslt.Load(xslFilePath);
        xslt.Transform(xmlFilePath, outputHtmlPath);
    }

    /// <summary>
    /// Прецедент розширення (UC6_Ext): збереження лише відфільтрованої вибірки у XML-файл.
    /// </summary>
    public void SaveFilteredXml(IEnumerable<StudentRecord> records, string outputXmlPath)
    {
        var root = new XElement("StudentPerformanceSystem",
            new XAttribute("university", "КНУ імені Тараса Шевченка (Відфільтрована вибірка)"));

        foreach (var st in records)
        {
            var stEl = new XElement("Student",
                new XAttribute("Id", st.Id),
                new XAttribute("FullName", st.FullName),
                new XAttribute("Faculty", st.Faculty),
                new XAttribute("Department", st.Department),
                new XAttribute("Course", st.Course),
                new XAttribute("Group", st.Group));

            if (!string.IsNullOrWhiteSpace(st.Specialty))
                stEl.Add(new XAttribute("Specialty", st.Specialty));
            if (!string.IsNullOrWhiteSpace(st.StudyForm))
                stEl.Add(new XAttribute("StudyForm", st.StudyForm));
            if (!string.IsNullOrWhiteSpace(st.Scholarship))
                stEl.Add(new XAttribute("Scholarship", st.Scholarship));

            foreach (var d in st.Disciplines)
            {
                stEl.Add(new XElement("Discipline",
                    new XAttribute("Name", d.Name),
                    new XAttribute("Semester", d.Semester),
                    new XAttribute("Credits", d.Credits),
                    new XAttribute("Grade", d.Grade),
                    new XAttribute("NationalGrade", d.NationalGrade),
                    new XAttribute("ExamType", d.ExamType),
                    new XAttribute("Lecturer", d.Lecturer)));
            }

            root.Add(stEl);
        }

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        doc.Save(outputXmlPath);
    }
}
