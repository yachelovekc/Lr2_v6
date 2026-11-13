using StudentPerformanceApp.Models;
using StudentPerformanceApp.ViewModels;

namespace StudentPerformanceApp;

/// <summary>
/// Головна сторінка застосунку .NET MAUI (MainPage).
/// Повністю відповідає спеціфікації класу MainPage на Class Diagram та взаємодіям на Sequence Diagram.
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private static readonly string AllItemsOption = "-- Усі --";

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        UpdateFileLabels();
        PopulateMetadataPickers();
    }

    public async Task<bool> ConfirmExitAsync()
    {
        bool confirm = await DisplayAlertAsync(
            "Підтвердження виходу",
            "Чи дійсно ви хочете завершити роботу з програмою?",
            "Так",
            "Ні");

        return confirm;
    }

    private void UpdateFileLabels()
    {
        XmlPathLabel.Text = string.IsNullOrWhiteSpace(_viewModel.XmlFilePath)
            ? "XML: не обрано"
            : $"XML: {_viewModel.XmlFilePath}";

        XslPathLabel.Text = string.IsNullOrWhiteSpace(_viewModel.XslFilePath)
            ? "XSL: не обрано"
            : $"XSL: {_viewModel.XslFilePath}";
    }

    private void PopulateMetadataPickers()
    {
        try
        {
            var meta = _viewModel.LoadMetadata();

            BindPicker(FacultyPicker, meta.Faculties);
            BindPicker(DepartmentPicker, meta.Departments);
            BindPicker(CoursePicker, meta.Courses);
            BindPicker(GroupPicker, meta.Groups);
            BindPicker(StudyFormPicker, meta.StudyForms);
            BindPicker(ScholarshipPicker, meta.Scholarships);
            BindPicker(DisciplinePicker, meta.Disciplines);
            BindPicker(NationalGradePicker, meta.NationalGrades);

            StatusLabel.Text = $"Підвантажено атрибути з файла «{Path.GetFileName(_viewModel.XmlFilePath)}». Доступно факультетів: {meta.Faculties.Count}, кафедр: {meta.Departments.Count}, дисциплін: {meta.Disciplines.Count}.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Помилка підвантаження атрибутів: {ex.Message}";
        }
    }

    private static void BindPicker(Picker picker, List<string> values)
    {
        var items = new List<string> { AllItemsOption };
        items.AddRange(values);
        picker.ItemsSource = items;
        picker.SelectedIndex = 0;
    }

    private static string? GetPickerSelectedValue(Picker picker)
    {
        if (picker.SelectedIndex <= 0 || picker.SelectedItem is not string val || val == AllItemsOption)
            return null;
        return val;
    }

    private void OnStrategyChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || sender is not RadioButton rb || rb.Value is not string strategyKey || _viewModel == null)
            return;

        _viewModel.SelectStrategy(strategyKey);
        StatusLabel.Text = $"Активовано стратегію аналізу XML: {_viewModel.CurrentStrategyName}";
    }

    private async void OnSelectXmlClicked(object? sender, EventArgs e)
    {
        try
        {
            var customXmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xml" } },
                { DevicePlatform.macOS, new[] { "xml" } },
                { DevicePlatform.Android, new[] { "text/xml", "application/xml" } },
                { DevicePlatform.iOS, new[] { "public.xml" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть вхідний XML-файл із даними про успішність студентів",
                FileTypes = customXmlFileType
            });

            if (result != null)
            {
                _viewModel.XmlFilePath = result.FullPath;
                UpdateFileLabels();
                PopulateMetadataPickers();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка вибору XML-файлу", ex.Message, "OK");
        }
    }

    private async void OnSelectXslClicked(object? sender, EventArgs e)
    {
        try
        {
            var customXslFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xsl", ".xslt" } },
                { DevicePlatform.macOS, new[] { "xsl", "xslt" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть XSL-файл стилів трансформації",
                FileTypes = customXslFileType
            });

            if (result != null)
            {
                _viewModel.XslFilePath = result.FullPath;
                UpdateFileLabels();
                StatusLabel.Text = $"Обрано XSL-шаблон: {Path.GetFileName(result.FullPath)}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка вибору XSL-файлу", ex.Message, "OK");
        }
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        try
        {
            int? minGrade = null;
            if (!string.IsNullOrWhiteSpace(MinGradeEntry.Text))
            {
                if (int.TryParse(MinGradeEntry.Text.Trim(), out int parsedGrade) && parsedGrade >= 0 && parsedGrade <= 100)
                {
                    minGrade = parsedGrade;
                }
                else
                {
                    await DisplayAlertAsync("Некоректні дані", "Мінімальний бал має бути цілим числом від 0 до 100.", "OK");
                    return;
                }
            }

            var criteria = new StudentSearchCriteria
            {
                Keyword = KeywordEntry.Text,
                Faculty = GetPickerSelectedValue(FacultyPicker),
                Department = GetPickerSelectedValue(DepartmentPicker),
                Course = GetPickerSelectedValue(CoursePicker),
                Group = GetPickerSelectedValue(GroupPicker),
                StudyForm = GetPickerSelectedValue(StudyFormPicker),
                Scholarship = GetPickerSelectedValue(ScholarshipPicker),
                DisciplineName = GetPickerSelectedValue(DisciplinePicker),
                NationalGrade = GetPickerSelectedValue(NationalGradePicker),
                MinGrade = minGrade
            };

            string report = _viewModel.ExecuteSearch(criteria);
            OutputEditor.Text = report;
            StatusLabel.Text = $"Пошук виконано ({_viewModel.CurrentStrategyName}). Знайдено студентів: {_viewModel.LastResults.Count}.";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка під час аналізу XML", ex.Message, "OK");
        }
    }

    private async void OnTransformClicked(object? sender, EventArgs e)
    {
        try
        {
            bool onlyFiltered = TransformFilteredCheckBox.IsChecked;
            string htmlContent = _viewModel.TransformCurrentToHtml(onlyFiltered, out string outputHtmlPath);

            HtmlPreviewWebView.Source = new HtmlWebViewSource { Html = htmlContent };
            StatusLabel.Text = $"XSLT-трансформацію успішно виконано. HTML збережено у: {outputHtmlPath}";

            bool copyPath = await DisplayAlertAsync(
                "Трансформація в HTML",
                $"Документ успішно трансформовано у HTML-файл:\n{outputHtmlPath}",
                "📋 Скопіювати шлях",
                "Закрити");

            if (copyPath)
            {
                await Clipboard.Default.SetTextAsync(outputHtmlPath);
                StatusLabel.Text = $"Шлях скопійовано в буфер обміну: {outputHtmlPath}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Помилка XSLT-трансформації", ex.Message, "OK");
        }
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        KeywordEntry.Text = string.Empty;
        MinGradeEntry.Text = string.Empty;

        FacultyPicker.SelectedIndex = FacultyPicker.Items.Count > 0 ? 0 : -1;
        DepartmentPicker.SelectedIndex = DepartmentPicker.Items.Count > 0 ? 0 : -1;
        CoursePicker.SelectedIndex = CoursePicker.Items.Count > 0 ? 0 : -1;
        GroupPicker.SelectedIndex = GroupPicker.Items.Count > 0 ? 0 : -1;
        StudyFormPicker.SelectedIndex = StudyFormPicker.Items.Count > 0 ? 0 : -1;
        ScholarshipPicker.SelectedIndex = ScholarshipPicker.Items.Count > 0 ? 0 : -1;
        DisciplinePicker.SelectedIndex = DisciplinePicker.Items.Count > 0 ? 0 : -1;
        NationalGradePicker.SelectedIndex = NationalGradePicker.Items.Count > 0 ? 0 : -1;

        TransformFilteredCheckBox.IsChecked = false;
        OutputEditor.Text = string.Empty;
        HtmlPreviewWebView.Source = new HtmlWebViewSource
        {
            Html = "<html><body style='font-family:sans-serif;color:#6b7280;padding:16px;'>Поле попереднього перегляду HTML очищено.</body></html>"
        };

        StatusLabel.Text = "Поле виводу та всі параметри пошуку очищено (Clear).";
    }

    private async void OnHelpClicked(object? sender, EventArgs e)
    {
        string helpMessage =
            "1. Обирайте вхідний файл XML та шаблон XSL кнопками «Обрати XML» та «Обрати XSL» (за замовчуванням підвантажено тестовий набір варіанту №6).\n\n" +
            "2. При виборі XML-файлу програма автоматично зчитує всі наявні атрибути (факультети, кафедри, курси, групи, дисципліни, оцінки) у списки вибору.\n\n" +
            "3. Оберіть один із 3 методів аналізу за шаблоном «Стратегія» (Strategy): SAX API, DOM API або LINQ to XML.\n\n" +
            "4. Задайте параметри фільтрації або ключове слово та натисніть «Пошук (Аналіз)».\n\n" +
            "5. Для генерації звіту натисніть «Трансформація в HTML». Щоб скинути всі фільтри і очистити поле виводу, натисніть «Clear (Очистити)».";

        await DisplayAlertAsync("Допомога користувачу", helpMessage, "Зрозуміло");
    }

    private async void OnAboutClicked(object? sender, EventArgs e)
    {
        string aboutMessage =
            "Лабораторна робота №2 (без зірочки)\n" +
            "Дисципліна: Об'єктно-орієнтоване програмування\n" +
            "Варіант №6: Інформаційна система «Успішність студентів»\n\n" +
            "Функціональні можливості:\n" +
            "• Аналіз XML трьома способами (SAX API, DOM API, LINQ to XML) із використанням шаблону проєктування «Стратегія» (Strategy);\n" +
            "• Динамічна генерація запитів та підвантаження атрибутів із файла;\n" +
            "• Трансформація XML у HTML за допомогою XSLT (*.xsl).";

        await DisplayAlertAsync("Про програму", aboutMessage, "OK");
    }

    private async void OnExitClicked(object? sender, EventArgs e)
    {
        if (await ConfirmExitAsync())
        {
            Application.Current?.Quit();
        }
    }
}
