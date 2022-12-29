using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using System.Diagnostics;

namespace Utility
{
    public static class InputFormUtil
    {
        
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        private static ModelData modelData
        {
            get
            {
                return ModelData.Instance;
            }
        }
        private static FormData formData
        {
            get
            {
                return FormData.Instance;
            }
        }
        public static void Run(this Model.Entity.ActiveView ettActiveView)
        {
            var ettColumnsInViewRVL = ettActiveView.EntityElements;

            string test = null;
            //ettColumnsInViewRVL.ForEach(x => x.CreateDimColumnGrid());
            foreach (Model.Entity.Element item in ettColumnsInViewRVL)
            {
                
                item.CreateDimColumnGrid();
            }

        }
        public static void RunPileCapDim(this List<Autodesk.Revit.DB.Element> pileCaps)
        {
            List<Model.Entity.Element> ettPileCaps = new List<Model.Entity.Element>();
            foreach (Autodesk.Revit.DB.Element item in pileCaps)
            {
                ettPileCaps.Add(new Model.Entity.Element { RevitElement = item });
            }
            foreach (Model.Entity.Element item1 in ettPileCaps)
            {
                item1.CreateDimPileCapGrid();
            }
        }
        public static void RunSpunPileDim (this List<Autodesk.Revit.DB.Element> pileCaps)
        {
            List<Model.Entity.Element> ettPileCaps = new List<Model.Entity.Element>();
            foreach (Autodesk.Revit.DB.Element item in pileCaps)
            {
                ettPileCaps.Add(new Model.Entity.Element { RevitElement = item });
            }
            foreach (Model.Entity.Element item1 in ettPileCaps)
            {
                item1.CreateDimSpunPileGrid();
            }
        }
        public static void SelectRevitLink()
        {
            var form = FormData.Instance.InputForm;
            form.Hide();
            var sel = revitData.Selection;
            try
            {
                formData.SettingView.Setting.RevitLink = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element).GetRevitElement() as Autodesk.Revit.DB.RevitLinkInstance;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            form.ShowDialog();
        }
        public static void SelectPileCap()
        {
            Func<Autodesk.Revit.DB.Element, bool> filterPileCap = x =>
            {
                var cate = x.Category.Id.IntegerValue;
                if (cate == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation) return true;
                return false;
            };
            var form = formData.InputForm;
            form.Hide();
            var sel = revitData.Selection;
            try
            {
                sel.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new SelectionUtil(filterPileCap), "Select PileCaps").ToList()
                    .ForEach(x=>formData.SelectedPileCaps.Add(x.GetRevitElement()));
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            form.ShowDialog();
        }
    }
}
