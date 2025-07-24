using content_management_system.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace content_management_system.Pages
{
    /// <summary>
    /// Interaction logic for AddEditObrenovicPage.xaml
    /// </summary>
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
                btnAdd.Content = "Change";
            }
            else
            {
                btnAdd.Content = "Add";
            }

            cbFontFamily.ItemsSource = Fonts.SystemFontFamilies;
            cbFontFamily.SelectedItem = new FontFamily("Times New Roman");

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

        private void TxtBirthYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtBirthYear.Text, out int birthYear))
            {
                if (birthYear < 1700 || birthYear > DateTime.Now.Year)
                {
                    MessageBox.Show("Unesite validnu godinu rodjenja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (!string.IsNullOrWhiteSpace(txtBirthYear.Text))
            {
                MessageBox.Show("Unesite samo brojeve.", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtBirthYear.Text, out int birthYear))
            {
                MessageBox.Show("Molimo popunite sva obavezna polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = txtName.Text.Trim();

            string rtfDirectory = "TextFiles";
            Directory.CreateDirectory(rtfDirectory);
            string formattedName = name.ToLower().Replace(" ", "_");
            string rtfFileName = $"{formattedName}.rtf";
            string rtfFullPath = System.IO.Path.Combine(rtfDirectory, rtfFileName);

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
            txtBirthYear.Text = o.DateOfBirth.ToString();

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
