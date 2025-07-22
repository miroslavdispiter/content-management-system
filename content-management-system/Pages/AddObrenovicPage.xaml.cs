using content_management_system.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace content_management_system.Pages
{
    public partial class AddObrenovicPage : Page
    {
        private readonly TablePage _tablePage;
        private string _selectedImagePath = string.Empty;

        public AddObrenovicPage(TablePage tablePage)
        {
            InitializeComponent();
            _tablePage = tablePage;
            datePickerBirth.SelectedDate = DateTime.Now;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;
                imgPreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void ColorPickerText_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerText.SelectedColor.HasValue)
            {
                TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(colorPickerText.SelectedColor.Value));
            }
        }

        private void RtbDescription_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
            string text = range.Text.Trim();
            int wordCount = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            lblWordCount.Content = $"Broj reči: {wordCount}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !datePickerBirth.SelectedDate.HasValue || string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                MessageBox.Show("Molimo popunite sva polja i izaberite sliku.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = txtName.Text.Trim();
            int birthYear = datePickerBirth.SelectedDate.Value.Year;

            string rtfDirectory = "Data/Descriptions";
            Directory.CreateDirectory(rtfDirectory);
            string rtfFileName = $"{Guid.NewGuid()}.rtf";
            string rtfFullPath = System.IO.Path.Combine(rtfDirectory, rtfFileName);

            using (FileStream fs = new FileStream(rtfFullPath, FileMode.Create))
            {
                TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }

            Obrenovic novi = new Obrenovic
            {
                Name = name,
                DateOfBirth = birthYear,
                ImgPath = _selectedImagePath,
                RtfPath = rtfFullPath
            };

            _tablePage.Obrenovici.Add(novi);
            _tablePage.SaveDataToXml();

            NavigationService?.GoBack();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
