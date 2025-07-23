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
    public partial class AddEditObrenovicPage : Page
    {
        private readonly TablePage _tablePage;
        private readonly Obrenovic _obrenovicZaIzmenu;
        private readonly bool _isEditMode;

        private string _selectedImagePath = string.Empty;

        public AddEditObrenovicPage(TablePage tablePage, Obrenovic obrenovicZaIzmenu = null)
        {
            InitializeComponent();
            _tablePage = tablePage;

            if (obrenovicZaIzmenu != null)
            {
                _isEditMode = true;
                _obrenovicZaIzmenu = obrenovicZaIzmenu;
                PopuniFormu(obrenovicZaIzmenu);
                btnAdd.Content = "Promeni";
            }
            else
            {
                datePickerBirth.SelectedDate = DateTime.Now;
                btnAdd.Content = "Dodaj";
            }

            cbFontFamily.ItemsSource = Fonts.SystemFontFamilies;
            cbFontFamily.SelectedItem = new FontFamily("Segoe UI");

            UpdateWordCount();
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

        private void CbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFontFamily.SelectedItem != null)
            {
                FontFamily selectedFont = cbFontFamily.SelectedItem as FontFamily;
                if (selectedFont != null)
                {
                    TextRange range = new TextRange(rtbDescription.Selection.Start, rtbDescription.Selection.End);
                    range.ApplyPropertyValue(TextElement.FontFamilyProperty, selectedFont);
                }
            }
        }

        private void CbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFontSize.SelectedItem is ComboBoxItem item && double.TryParse(item.Content.ToString(), out double size))
            {
                TextRange range = new TextRange(rtbDescription.Selection.Start, rtbDescription.Selection.End);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            }
        }

        private void RtbDescription_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            UpdateWordCount();
        }

        private void UpdateWordCount()
        {
            TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
            string text = range.Text.Trim();
            int wordCount = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            lblWordCount.Content = $"Broj reči: {wordCount}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !datePickerBirth.SelectedDate.HasValue)
            {
                MessageBox.Show("Molimo popunite sva obavezna polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = txtName.Text.Trim();
            int birthYear = datePickerBirth.SelectedDate.Value.Year;

            string rtfDirectory = "TextFiles";
            Directory.CreateDirectory(rtfDirectory);
            string formattedName = name.ToLower().Replace(" ", "_");
            string rtfFileName = $"{formattedName}.rtf";
            string rtfFullPath = Path.Combine(rtfDirectory, rtfFileName);

            using (FileStream fs = new FileStream(rtfFullPath, FileMode.Create))
            {
                TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }

            if (_isEditMode)
            {
                _obrenovicZaIzmenu.Name = name;
                _obrenovicZaIzmenu.DateOfBirth = birthYear;
                _obrenovicZaIzmenu.RtfPath = rtfFullPath;
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    _obrenovicZaIzmenu.ImgPath = _selectedImagePath;
                }

                _tablePage.SaveDataToXml();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    MessageBox.Show("Molimo izaberite sliku.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
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
            }

            NavigationService?.GoBack();
        }

        private void PopuniFormu(Obrenovic o)
        {
            txtName.Text = o.Name;
            datePickerBirth.SelectedDate = new DateTime(o.DateOfBirth, 1, 1);

            if (!string.IsNullOrWhiteSpace(o.ImgPath) && File.Exists(o.ImgPath))
            {
                _selectedImagePath = o.ImgPath;
                imgPreview.Source = new BitmapImage(new Uri(_selectedImagePath, UriKind.RelativeOrAbsolute));
            }

            if (!string.IsNullOrWhiteSpace(o.RtfPath) && File.Exists(o.RtfPath))
            {
                using FileStream fs = new FileStream(o.RtfPath, FileMode.Open, FileAccess.Read);
                TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                range.Load(fs, DataFormats.Rtf);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
