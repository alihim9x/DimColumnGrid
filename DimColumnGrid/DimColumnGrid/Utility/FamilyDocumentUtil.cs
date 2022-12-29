using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace Utility
{
    public static class FamilyDocumentUtil
    {
        private static RevitData revitData = RevitData.Instance;
        public static Model.Entity.FamilyDocument Create(string name, Action<Autodesk.Revit.DB.Document> action)
        {
            var famDoc = new Model.Entity.FamilyDocument();
            var doc = revitData.Document;
            var app = revitData.Application;
            var revitFamDoc = app.NewFamilyDocument(famDoc.TemplatePath);
            famDoc.Document = revitFamDoc;
            famDoc.Name = name;

            var famTx = new Autodesk.Revit.DB.Transaction(revitFamDoc, "Modify Document");
            famTx.Start();

            action(famDoc.Document);

            famTx.Commit();

            revitFamDoc.SaveAs(famDoc.TemporaryPath);

            return famDoc;
        }
    }
}
