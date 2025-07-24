using content_management_system.Helpers;
using content_management_system.Models;
using Microsoft.Win32;
using Notification.Wpf;
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
        private readonly Obrenovic _obrenovicEdit;
        private readonly bool _isEditMode;
        private readonly MainWindow mainWindow;
        bool isInitializing = true;

        private string _selectedImagePath = string.Empty;

        public AddEditObrenovicPage(TablePage tablePage, Obrenovic obrenovicEdit = null)
        {
            InitializeComponent();
            _tablePage = tablePage;
            mainWindow = (MainWindow)Application.Current.MainWindow;

            if (obrenovicEdit != null)
            {
                _isEditMode = true;
                _obrenovicEdit = obrenovicEdit;
                PopuniFormu(obrenovicEdit);
                btnAdd.Content = "Change";
            }
            else
            {
                btnAdd.Content = "Add";
            }

            cbFontFamily.ItemsSource = Fonts.SystemFontFamilies;
            cbFontFamily.SelectedItem = new FontFamily("Times New Roman");
            cbFontSize.SelectedItem = cbFontSize.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == "14");
            colorPickerText.SelectedColor = Colors.Black;

            isInitializing = false;

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
            if (isInitializing) return;

            if (colorPickerText.SelectedColor.HasValue)
            {
                var selection = rtbDescription.Selection;

                if (!selection.IsEmpty)
                {
                    selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(colorPickerText.SelectedColor.Value));
                    mainWindow.ShowToastNotification(new ToastNotification("Success", "You have successfully changed the selected text color.", NotificationType.Success));
                }
                else
                {
                    mainWindow.ShowToastNotification(new ToastNotification("Information", "Please select some text before applying color.", NotificationType.Information));
                }
            }
        }

        private void CbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitializing) return;

            if (cbFontFamily.SelectedItem != null)
            {
                FontFamily selectedFont = cbFontFamily.SelectedItem as FontFamily;
                if (selectedFont != null)
                {
                    TextRange range = new TextRange(rtbDescription.Selection.Start, rtbDescription.Selection.End);
                    range.ApplyPropertyValue(TextElement.FontFamilyProperty, selectedFont);
                    mainWindow.ShowToastNotification(new ToastNotification("Success", $"Selected font: {selectedFont.Source} has been applied.", NotificationType.Success));
                }
            }
        }

        private void CbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitializing) return;

            if (cbFontSize.SelectedItem is ComboBoxItem item && double.TryParse(item.Content.ToString(), out double size))
            {
                TextRange range = new TextRange(rtbDescription.Selection.Start, rtbDescription.Selection.End);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                mainWindow.ShowToastNotification(new ToastNotification("Success", $"Font size set to {size}.", NotificationType.Success));
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowToastNotification(new ToastNotification("Success", "Bold applied to selected text.", NotificationType.Success));
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowToastNotification(new ToastNotification("Success", "Italic applied to selected text.", NotificationType.Success));
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowToastNotification(new ToastNotification("Success", "Underline applied to selected text.", NotificationType.Success));
        }

        private void RtbDescription_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateWordCount();
        }

        private void UpdateWordCount()
        {
            TextRange range = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
            string text = range.Text.Trim();
            int wordCount = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            lblWordCount.Content = $"Number of Words: {wordCount}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NameError.Text = "";
            YearOfBirthError.Text = "";

            bool isValid = true;

            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                NameError.Text = "Name is required.";
                isValid = false;
            }

            string birthYearText = txtBirthYear.Text.Trim();
            int birthYear = 0;

            if (string.IsNullOrWhiteSpace(birthYearText))
            {
                YearOfBirthError.Text = "Year of birth is required.";
                isValid = false;
            }
            else if (!int.TryParse(birthYearText, out birthYear))
            {
                YearOfBirthError.Text = "Year must be a number.";
                isValid = false;
            }
            else if (birthYear < 1700 || birthYear > DateTime.Now.Year)
            {
                YearOfBirthError.Text = $"Enter a valid year between 1700 and {DateTime.Now.Year}.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                mainWindow.ShowToastNotification(new ToastNotification("Error", "Please select an image.", NotificationType.Error));
                isValid = false;
            }

            TextRange descriptionRange = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
            string descriptionText = descriptionRange.Text.Trim();

            if (string.IsNullOrWhiteSpace(descriptionText))
            {
                DescriptionError.Text = "Description is required.";
                isValid = false;
            }
            else
            {
                DescriptionError.Text = "";
            }

            if (!isValid)
                return;

            string rtfDirectory = "../../../TextFiles";
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
                _obrenovicEdit.Name = name;
                _obrenovicEdit.DateOfBirth = birthYear;
                _obrenovicEdit.RtfPath = rtfFullPath;
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    _obrenovicEdit.ImgPath = _selectedImagePath;
                }
                else
                {
                    mainWindow.ShowToastNotification(new ToastNotification("Warning", "Please select an image.", NotificationType.Warning));
                    return;
                }

                mainWindow.ShowToastNotification(new ToastNotification("Success", "Member successfully updated.", NotificationType.Success));
                _tablePage.SaveDataToXml();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    mainWindow.ShowToastNotification(new ToastNotification("Warning", "Please select an image.", NotificationType.Warning));
                    return;
                }

                Obrenovic newO = new Obrenovic
                {
                    Name = name,
                    DateOfBirth = birthYear,
                    ImgPath = _selectedImagePath,
                    RtfPath = rtfFullPath
                };

                 mainWindow.ShowToastNotification(new ToastNotification("Success", "New member successfully added.", NotificationType.Success));
                _tablePage.Obrenovici.Add(newO);
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
