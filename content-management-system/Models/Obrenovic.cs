using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace content_management_system.Models
{
    public class Obrenovic
    {
        public string Name { get; set; }

        public int DateOfBirth { get; set; }

        public string ImgPath { get; set; }

        public string RtfPath { get; set; }

        public string DateAdded { get; set; }

        public Obrenovic()
        {
            Name = string.Empty;
            DateOfBirth = 0;
            ImgPath = string.Empty;
            RtfPath = string.Empty;
            DateAdded = DateTimeOffset.Now.ToString("dd-MM-yyyy");
        }

        public Obrenovic(string name, int dateOfBirth, string imgPath, string rtfPath, string timeAdded)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            ImgPath = imgPath;
            RtfPath = rtfPath;
            DateAdded = DateTimeOffset.Now.ToString("dd-MM-yyyy");
        }
    }
}
