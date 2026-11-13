using StudentPerformanceApp.Models;

namespace StudentPerformanceApp.Strategies;

/// <summary>
/// Інтерфейс шаблону проєктування «Стратегія» (Strategy) для аналізу XML-документа.
/// </summary>
public interface IXmlAnalysisStrategy
{
    string StrategyName { get; }
    List<StudentRecord> Search(string xmlFilePath, StudentSearchCriteria criteria);
}
