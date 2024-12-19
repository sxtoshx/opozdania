using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace LateStudentsTracker
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<LateStudent> LateStudents { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LateStudents = new ObservableCollection<LateStudent>();
            LoadData();
            LateStudentsDataGrid.ItemsSource = LateStudents;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StudentNameTextBox.Text) || DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Введите имя студента и выберите дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LateStudents.Add(new LateStudent
            {
                Name = StudentNameTextBox.Text,
                Date = DatePicker.SelectedDate.Value
            });

            StudentNameTextBox.Clear();
            DatePicker.SelectedDate = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "LateStudents.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var student in LateStudents)
                    {
                        writer.WriteLine($"{student.Name};{student.Date:yyyy-MM-dd}");
                    }
                }
                MessageBox.Show("Данные сохранены в файл LateStudents.txt.", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            string filePath = "LateStudents.txt";
            if (File.Exists(filePath))
            {
                try
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        var parts = line.Split(';');
                        if (parts.Length == 2 && DateTime.TryParse(parts[1], out var date))
                        {
                            LateStudents.Add(new LateStudent { Name = parts[0], Date = date });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class LateStudent
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
