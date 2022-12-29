using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class FamilyDocument
    {
        public string TemplatePath { get; set; }
        = @"C:\ProgramData\Autodesk\RVT 2019\Family Templates\English\Metric Generic Model.rft";
        public string Name { get; set; }
        public string TemporaryPath
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), $"{Name}.rfa");
            }
        }
        public Autodesk.Revit.DB.Document Document { get; set; }
        public Action<Autodesk.Revit.DB.Document> ModifyDocument { get; set; }
      
    }
}
