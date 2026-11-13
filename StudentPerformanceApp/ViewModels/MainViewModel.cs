using System.Text;
using StudentPerformanceApp.Models;
using StudentPerformanceApp.Services;
using StudentPerformanceApp.Strategies;

namespace StudentPerformanceApp.ViewModels;

/// <summary>
/// Клас MainViewModel відповідно до Class Diagram та Sequence Diagram.
/// Інкапсулює взаємодію між MainPage (MAUI UI) та XmlProcessingService (Context шаблону Strategy).
/// </summary>
public class MainViewModel
{
    private readonly XmlProcessingService _xmlService;
    private List<StudentRecord> _lastResults = new();

    public string XmlFilePath { get; set; } = string.Empty;
    public string XslFilePath { get; set; } = string.Empty;
    public string CurrentStrategyName => _xmlService.CurrentStrategyName;
    public IReadOnlyList<StudentRecord> LastResults => _lastResults;

    public MainViewModel()
    {
        _xmlService = new XmlProcessingService(new LinqXmlAnalysisStrategy());
        InitializeDefaultFiles();
    }

    public XmlMetadataValues LoadMetadata()
    {
        return _xmlService.ExtractAttributeValues(XmlFilePath);
    }

    public void SelectStrategy(string strategyKey)
    {
        IXmlAnalysisStrategy strategy = strategyKey switch
        {
            "SAX" => new SaxXmlAnalysisStrategy(),
            "DOM" => new DomXmlAnalysisStrategy(),
            _ => new LinqXmlAnalysisStrategy()
        };
        _xmlService.SetStrategy(strategy);
    }

    public string ExecuteSearch(StudentSearchCriteria criteria)
    {
        _lastResults = _xmlService.Analyze(XmlFilePath, criteria);
        return FormatResults(_lastResults, criteria);
    }

    public string TransformCurrentToHtml(bool onlyFiltered, out string outputHtmlPath)
    {
        string targetDir = Path.GetDirectoryName(XmlFilePath) ?? AppContext.BaseDirectory;
        outputHtmlPath = Path.Combine(targetDir, "students_report.html");

        if (onlyFiltered)
        {
            string tempXmlPath = Path.Combine(targetDir, "filtered_students_temp.xml");
            _xmlService.SaveFilteredXml(_lastResults, tempXmlPath);
            _xmlService.TransformToHtml(tempXmlPath, XslFilePath, outputHtmlPath);
        }
        else
        {
            _xmlService.TransformToHtml(XmlFilePath, XslFilePath, outputHtmlPath);
        }

        return File.ReadAllText(outputHtmlPath, Encoding.UTF8);
    }

    private void InitializeDefaultFiles()
    {
        string baseDir = AppContext.BaseDirectory;
        string defaultXml = Path.Combine(baseDir, "SampleData", "students_performance.xml");
        string defaultXsl = Path.Combine(baseDir, "SampleData", "students_performance.xsl");

        if (File.Exists(defaultXml))
            XmlFilePath = defaultXml;

        if (File.Exists(defaultXsl))
            XslFilePath = defaultXsl;
    }

    private string FormatResults(List<StudentRecord> students, StudentSearchCriteria criteria)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== АНАЛІЗ XML-ДОКУМЕНТА: ІС «УСПІШНІСТЬ СТУДЕНТІВ» (ВАРІАНТ 6) ===");
        sb.AppendLine($"Обрана стратегія (Strategy): {CurrentStrategyName}");
        sb.AppendLine($"Динамічний XPath-запит: {DomXmlAnalysisStrategy.BuildDynamicXPath(criteria)}");
        sb.AppendLine($"Знайдено студентів: {students.Count}");
        sb.AppendLine(new string('=', 72));

        if (students.Count == 0)
        {
            sb.AppendLine("За заданими критеріями пошуку жодного запису не знайдено.");
            return sb.ToString();
        }

        int index = 1;
        foreach (var st in students)
        {
            sb.AppendLine($"[{index++}] П.І.П.: {st.FullName} (ID: {st.Id}) | Атрибутів у вузлі: {st.AttributeCount}");
            sb.AppendLine($"    Факультет: {st.Faculty} | Кафедра: {st.Department}");
            sb.Append($"    Курс: {st.Course} | Група: {st.Group}");

            if (!string.IsNullOrWhiteSpace(st.Specialty))
                sb.Append($" | Спеціальність: {st.Specialty}");
            if (!string.IsNullOrWhiteSpace(st.StudyForm))
                sb.Append($" | Форма: {st.StudyForm}");
            if (!string.IsNullOrWhiteSpace(st.Scholarship))
                sb.Append($" | Стипендія: {st.Scholarship}");
            sb.AppendLine();
            sb.AppendLine($"    Середній бал: {st.AverageGrade:F2}");
            sb.AppendLine("    Навчальні дисципліни та успішність:");

            foreach (var d in st.Disciplines)
            {
                sb.AppendLine($"      • {d.Name} (Сем. {d.Semester}, {d.Credits} кред., {d.ExamType}) — {d.Grade} балів ({d.NationalGrade}), викл. {d.Lecturer}");
            }
            sb.AppendLine(new string('-', 72));
        }

        return sb.ToString();
    }
}
