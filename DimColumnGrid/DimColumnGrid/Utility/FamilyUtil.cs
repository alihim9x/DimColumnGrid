using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;


namespace Utility
{
    public static class FamilyUtil
    {
        private static RevitData revitData = RevitData.Instance;
        public static Model.Entity.Family Create(string name,Action<Autodesk.Revit.DB.Document> action)
        {
            var doc = revitData.Document;
            var fam = new Model.Entity.Family();

            var famDoc = FamilyDocumentUtil.Create(name, action);
            fam.FamilyDocument = famDoc;

            Autodesk.Revit.DB.Family revitFam = null;
            doc.LoadFamily(famDoc.TemporaryPath, out revitFam);

            famDoc.Document.Close();
            if(System.IO.File.Exists(famDoc.TemporaryPath))
            {
                System.IO.File.Delete(famDoc.TemporaryPath);
            }
            fam.RevitFamily = revitFam;
            
            return fam;
        }
        public static Autodesk.Revit.DB.FamilyInstance Insert(this Model.Entity.Family family, Autodesk.Revit.DB.XYZ point)
        {
            var doc = revitData.Document;
            return doc.Create.NewFamilyInstance(point, family.DefaultFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
        }
        public static IEnumerable<Autodesk.Revit.DB.FamilySymbol> GetFamilySymbols(this Model.Entity.Family family)
        {
            return family.RevitFamily.GetFamilySymbolIds().Select(x => x.GetRevitElement() as Autodesk.Revit.DB.FamilySymbol);
        }
        
    }
}

