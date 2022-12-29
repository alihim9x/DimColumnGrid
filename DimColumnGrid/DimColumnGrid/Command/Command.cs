using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using SingleData;
using Autodesk.Revit.UI.Selection;
using Utility;
using Autodesk.Revit.DB.Structure;
using System.Diagnostics;
using Model.Entity;

namespace DimColumnsGrid
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Initial
            var singleTon = Singleton.Instance = new Singleton();
            var revitData = singleTon.RevitData;
            revitData.UIApplication = commandData.Application;
            var sel = revitData.Selection;
            var doc = revitData.Document;
            var activeView = revitData.ActiveView;
            var tx = revitData.Transaction;
            var uidoc = revitData.UIDocument;
            var app = revitData.Application;
            tx.Start();
            #endregion

            var form = FormData.Instance.InputForm;
            form.ShowDialog();




            //try
            //{
            //RevitLinkInstance rvL = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element).GetRevitElement() as Autodesk.Revit.DB.RevitLinkInstance;
            //}
            //catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            //{

            //}


            //var rvL = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element).GetRevitElement() as Autodesk.Revit.DB.RevitLinkInstance;
            //ModelData.Instance.Setting.RevitLink = rvL;
            ////var bbActivewView = activeView.get_BoundingBox(null);
            //var preTransformSolid = activeView.CreateSolidFromBBox1();
            //Model.Entity.ActiveView ettActive = ModelData.Instance.ActiveView;
            ////List<Model.Entity.Element> ettColumnsInView = ettActive.EntityElements;
            ////Action<Document> differenceAction8 = ((x) =>
            ////{
            ////    var translateSolid8 = preTransformSolid;
            ////    FreeFormElement.Create(x, translateSolid8);

            ////});
            //DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            //ds.ApplicationId = "Test";
            //ds.ApplicationDataId = "testBox";
            //List<GeometryObject> geoList = new List<GeometryObject>();
            //geoList.Add(preTransformSolid);
            //ds.SetShape(geoList);
            //ds.SetName("ID_testBox");

            //var columnFamily8 = FamilyUtil.Create($"Solid_{Guid.NewGuid()}", differenceAction8);
            //columnFamily8.Insert(activeView.get_BoundingBox(null).Transform.Origin);



            //List<FamilyInstance> columnsInstancesInPlanViewRvL1 = revitData.InstanceElementsInViewRvL.Where(x => x.Id.IntegerValue
            //== (int)BuiltInCategory.OST_StructuralColumns).Cast<FamilyInstance>().ToList();
            //List<FamilyInstance> columnsInstancesInPlanViewRvL = revitData.ColumnInstancesInViewRvL.Cast<FamilyInstance>().ToList();


            //var ettColumnInViewRvL = new Model.Entity.Element { RevitElement = columnsInstancesInPlanViewRvL.FirstOrDefault() };




            ////var ettColumnInView = ettColumnsInView.FirstOrDefault();
            //var dimType = revitData.DimensionTypes.ToList().Single(x => x.Name == "NTK_Dim");
            ////var ettColumnInView = new Model.Entity.Element { RevitElement = sel.PickObject(ObjectType.Element).GetRevitElement() };

            //var faceEttColumninView = ettColumnInViewRvL.Faces;
            //var gridsInView = revitData.GridsInView.ToList();
            //List<Grid> gridsParallel = new List<Grid>();
            //Grid gridToDim = null;
            ////var grid = sel.PickObject(ObjectType.Element).GetRevitElement() as Grid;
            //ReferenceArray yFaceRefArray = new ReferenceArray();
            //ReferenceArray xFaceRefArray = new ReferenceArray();
            ////yFaceRefArray.Append(new Reference(grid));
            //var geoElem = ettColumnInViewRvL.RevitElement.get_Geometry(new Options());
            //var geoSolid = geoElem.Where(x => x is Solid).FirstOrDefault();
            //foreach (PlanarFace item in faceEttColumninView.YFace)
            //{
            //    // code a Tài: Cột ko join thì dùng cách này để lấy ref
            //    //var rfString = item.Reference.ConvertToStableRepresentation(doc);
            //    //var editRFString = $"{ettColumnInView.RevitElement.UniqueId}:0:INSTANCE:{rfString}";
            //    //var rf = Reference.ParseFromStableRepresentation(doc,editRFString);
            //    //yFaceRefArray.Append(rf);

            //    // code Cột file Link
            //    //{UniqueIdRV}L:0:RVLINK:{ElementId}:0:INSTANCE:{Reference.ElementId}{PlanarFace.Reference.ConvertStable...Remove(0,elementSymbol.UniqueId.Length)}
            //    var rvlUniqueId = rvL.UniqueId;
            //    var elemIdColumnInViewRvL = ettColumnInViewRvL.RevitElement.Id.ToString();
            //    var refElemId = item.Reference.ElementId;
            //    var uniqueIdFamSym = (ettColumnInViewRvL.RevitElement as FamilyInstance).UniqueId;
            //    var refStable = item.Reference.ConvertToStableRepresentation(rvL.GetLinkDocument());
            //    var lastStrNotJoin = refStable.Remove(0,uniqueIdFamSym.Length);
            //    var lastStrJoined = refStable.Remove(0, uniqueIdFamSym.Length);
            //    var editRFStringNotJoin = $"{rvlUniqueId}:0:RVTLINK:{elemIdColumnInViewRvL}:0:INSTANCE:{refElemId}{lastStrNotJoin}";
            //    var editRFStringJoined = $"{rvlUniqueId}:0:RVTLINK:{elemIdColumnInViewRvL}{lastStrJoined}";
            //    var rfNotJoin = Reference.ParseFromStableRepresentation(doc, editRFStringNotJoin);
            //    var rfJoined = Reference.ParseFromStableRepresentation(doc, editRFStringJoined);
            //    //yFaceRefArray.Append(rfNotJoin);              
            //    //yFaceRefArray.Append(rfJoined);

            //    if(geoSolid == null)
            //    {
            //        yFaceRefArray.Append(rfNotJoin);
            //    }
            //    else
            //    {
            //        yFaceRefArray.Append(rfJoined);
            //    }


            //    // cột join thì dùng truyền thống
            //    //yFaceRefArray.Append(item.Reference);


            //    //925238e1-a793-467c-9494-f81d953d50dd-0005ce73:0:INSTANCE:dd0c29da-1047-4e69-8e05-cf87557e95ff-0005d5c5:20:SURFACE

            //    //doc.Create.NewDetailCurve(activeView,Line.CreateBound(item.Origin, item.Origin + 2000.0.milimeter2Feet()*activeView.UpDirection))
            //    //yFaceRefArray.Append((item.Reference.CreateLinkReference(rvL) as Reference).MakeLinkedRef4Dim());
            //}
            //gridsParallel = gridsInView.Where(x => (x.Curve as Line).Direction
            //.IsPerpendicularDirection(faceEttColumninView.YFace.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            //var dimOriginYFace = ettColumnInViewRvL.Geometry.Origin + (ettColumnInViewRvL.Geometry.LengthY/2 
            //    + 200.0.milimeter2Feet()) * ettColumnInViewRvL.Geometry.BasisY;
            //var dimLineYFace = Line.CreateBound(dimOriginYFace, dimOriginYFace + ettColumnInViewRvL.Geometry.BasisX);
            //gridToDim = gridsParallel.Where(x => (x.Curve as Line).Distance(faceEttColumninView.YFace.FirstOrDefault().Origin) 
            //== (gridsParallel.Min(y=>(y.Curve as Line).Distance(faceEttColumninView.YFace.FirstOrDefault().Origin)))).FirstOrDefault();
            //yFaceRefArray.Append(new Reference(gridToDim));
            //doc.Create.NewDimension(activeView, dimLineYFace, yFaceRefArray, dimType);

            //TaskDialog.Show("Revit", $"{faceEttColumninView.XFace.First().Area.FeetSq2MeterSq()}");
            //List<FamilyInstance> columnsInstancesInPlanViewRvL = revitData.ColumnInstancesInViewRvL.Cast<FamilyInstance>().ToList();
            //columnsInstancesInPlanViewRvL.ForEach(x => TaskDialog.Show("Revit", $"{x.AsValue("Mark").ValueText}"));


            //doc.Create.NewDetailCurve(activeView, Line.CreateBound(faceEttColumninView.YFace.First().Origin, faceEttColumninView.YFace.First().Origin
            //    + 2000.0.milimeter2Feet() * ettColumnInViewRvL.Geometry.BasisY));
            //doc.Create.NewDetailCurve(activeView, Line.CreateBound(faceEttColumninView.YFace.Last().Origin, faceEttColumninView.YFace.Last().Origin
            //    + 2000.0.milimeter2Feet() * ettColumnInViewRvL.Geometry.BasisY));

            //doc.Create.NewDetailCurve(activeView,Line.CreateBound(dimOriginYFace, dimOriginYFace + 2000.0.milimeter2Feet() * activeView.UpDirection));










            tx.Commit();


            return Result.Succeeded;       
        }
    }
}
