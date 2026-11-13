namespace StudentPerformanceApp.Services;

/// <summary>
/// Набір унікальних значень атрибутів, витягнутих із XML-файлу при його підвантаженні (підгрузці даних).
/// Повністю відповідає класу XmlMetadataValues на діаграмі класів UML.
/// </summary>
public class XmlMetadataValues
{
    public List<string> Faculties { get; set; } = new();
    public List<string> Departments { get; set; } = new();
    public List<string> Courses { get; set; } = new();
    public List<string> Groups { get; set; } = new();
    public List<string> StudyForms { get; set; } = new();
    public List<string> Scholarships { get; set; } = new();
    public List<string> Disciplines { get; set; } = new();
    public List<string> NationalGrades { get; set; } = new();
}
