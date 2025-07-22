using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace content_management_system.Models
{
    public class Obrenovic
    {
        public string Name { get; set; }
        public int DateOfBirth { get; set; }
        public string ImgPath { get; set; }
        public string RtfPath { get; set; }
        public string DateAdded { get; set; }
        public bool IsSelected { get; set; }

        public Obrenovic()
        {
            Name = string.Empty;
            DateOfBirth = 0;
            ImgPath = string.Empty;
            RtfPath = string.Empty;
            DateAdded = DateTimeOffset.Now.ToString("dd-MM-yyyy");
        }

        public Obrenovic(string name, int dateOfBirth, string imgPath, string rtfPath)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            ImgPath = imgPath;
            RtfPath = rtfPath;
            DateAdded = DateTimeOffset.Now.ToString("dd-MM-yyyy");
        }

        // ovaj property je dodat jer mi nije prikazivalo sliku kad sam koristio relativnu putanju
        public BitmapImage ImageSource
        {
            get
            {
                if (string.IsNullOrEmpty(ImgPath)) 
                    return null;
                
                try
                {
                    string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImgPath);
                    return new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
