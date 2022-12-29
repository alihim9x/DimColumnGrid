using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Utility
{
    public static class ElementUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }

        public static Autodesk.Revit.DB.Element GetRevitElement(this Autodesk.Revit.DB.ElementId elemId)
        {
            return RevitData.Instance.Document.GetElement(elemId);
        }
        public static Autodesk.Revit.DB.Element GetRevitElement(this Autodesk.Revit.DB.Reference reference)
        {
            return RevitData.Instance.Document.GetElement(reference);
        }
        public static Model.Entity.Element GetEntityElement(this Autodesk.Revit.DB.Element elem)
        {
            var ettElem = ModelData.Instance.EttElements.SingleOrDefault(x => x.RevitElement.UniqueId == elem.UniqueId);
            if (ettElem == null)
            {
                ettElem = new Model.Entity.Element()
                {
                    RevitElement = elem
                };
                ModelData.Instance.EttElements.Add(ettElem);
            }
            return ettElem;
        }

        public static Model.Entity.ElementType GetElementType(this Model.Entity.Element ettElem)
        {
            var revitElem = ettElem.RevitElement;
            var cate = revitElem.Category;
            if (revitElem is Autodesk.Revit.DB.Floor)
                return Model.Entity.ElementType.Floor;
            if (revitElem is Autodesk.Revit.DB.Wall && (revitElem.Location as LocationCurve).Curve is Line)
                return Model.Entity.ElementType.Wall;
            if (cate.Id.IntegerValue == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming)
                return Model.Entity.ElementType.Framing;
            if (cate.Id.IntegerValue == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns)
                return Model.Entity.ElementType.Column;
            if (cate.Id.IntegerValue == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation)
                return Model.Entity.ElementType.PileCap;
            return Model.Entity.ElementType.Undefined;
        }
        public static List<Element> GetSubComponent (this Model.Entity.Element ettElem)
        {
            var rvElem = ettElem.RevitElement as FamilyInstance;
            List<ElementId> subComponentIds = rvElem.GetSubComponentIds().ToList();
            List<Element> subComponents = new List<Element>();
            subComponentIds.ForEach(x => subComponents.Add(x.GetRevitElement()));
            return subComponents;
        }
        public static List<PlanarFace> MinimumList(this List<PlanarFace> listPlanarFace)
        {
            List<PlanarFace> returnList = new List<PlanarFace>();
            //try
            //{
                PlanarFace maxFace = listPlanarFace[0];
                int a = 0;
                for (int i = 0; i < listPlanarFace.Count; i++)
                {
                    if (maxFace.Area < listPlanarFace[i].Area)
                    {
                        maxFace = listPlanarFace[i];
                        a = i;
                    }
                }
                returnList.Add(listPlanarFace[a]);
                listPlanarFace.RemoveAt(a);
                returnList.Add(listPlanarFace.OrderByDescending(x => x.Area).FirstOrDefault());
                //for (int i = 0; i < listPlanarFace.Count; i++)
                //{
                //    if (maxFace.Area < listPlanarFace[i].Area)
                //    {
                //        maxFace = listPlanarFace[i];
                //        a = i;
                //    }
                //}
                //returnList.Add(listPlanarFace[a]);
            //}
            //catch (Exception)
            //{


            //}

            return returnList;
        }
        //public static List<PlanarFace> RemoveFaceNull (this List<PlanarFace> planarFace)
        //{

        //    foreach (PlanarFace item in planarFace)
        //    {
        //        if(item == null)
        //        {
        //            planarFace.Remove(item);
        //        }
        //    }
        //    return planarFace;
        //}
        public static Model.Entity.Face GetFaces(this Model.Entity.Element ettElem)
        {
            var typeElem = ettElem.ElementType;
            var revitSolid = ettElem.RevitElement.GetSingleSolidValidRef();
            var ab = ettElem.SubComponents.SingleOrDefault(x=>x.Name.Contains("Pilecap"));
            if(revitSolid == null)
            {
                revitSolid = ab.GetSingleSolidValidRef();
            }
            var faceElem = new Model.Entity.Face();
            List<PlanarFace> yFace = new List<PlanarFace>();
            List<PlanarFace> xFace = new List<PlanarFace>();
            var activeView = revitData.ActiveView;
            string test = null;
            for (int i = 0; i < revitSolid.Faces.Size; i++)
            {

                if (revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsXOrY() && !revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsParallelDirection(XYZ.BasisZ))
                {
                    //test = (revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace).Reference.ConvertToStableRepresentation(revitData.Document);
                    yFace.Add(revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace);
                }
                else if (!revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsXOrY() && !revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsParallelDirection(XYZ.BasisZ))

                {
                    xFace.Add(revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace);
                }
            }
            yFace.RemoveAll(x => x == null);
            //yFace.RemoveAll(x => (x.GraphicsStyleId.GetRevitElement() as GraphicsStyle).Name == "LeanConcrete");
            xFace.RemoveAll(x=>x == null);
            //xFace.RemoveAll(x => (x.GraphicsStyleId.GetRevitElement() as GraphicsStyle).Name == "LeanConcrete");
            if (yFace.Count > 2)
            {
                faceElem.YFace = yFace.MinimumList();
            }
            else
            {
                faceElem.YFace = yFace;
            }
            if (xFace.Count > 2)
            {
                faceElem.XFace = xFace.MinimumList();
            }
            else
            {
                faceElem.XFace = xFace;
            }


            return faceElem;
        }
        public static Autodesk.Revit.DB.XYZ MaxRepeatedItem(this List<Autodesk.Revit.DB.XYZ> listXYZ)
        {
            var maxRepeatedItems = listXYZ.GroupBy(x => x.Z).OrderByDescending(x => x.Count()).First().Select(x => x).First();
            return maxRepeatedItems;

        }

        public static bool IsIntersectWithActiveView(this Autodesk.Revit.DB.Element elem)
        {

            var insElem = revitData.InstanceElements.Where(x =>
            {
                List<BuiltInCategory> bic = new List<BuiltInCategory>
                {
                    (BuiltInCategory)elem.Category.Id.IntegerValue
                };

                var cate = x.Category;
                if (cate == null) return false;
                return bic.Contains((Autodesk.Revit.DB.BuiltInCategory)cate.Id.IntegerValue);
            }).ToList();
            var yes = false;
            var activeView = revitData.ActiveView;
            var bbActiveView = activeView.get_BoundingBox(null);
            var solidOfView = bbActiveView.CreateSolidFromBBox().ScaleSolid(1.001);
            var outline = new Autodesk.Revit.DB.Outline(bbActiveView.Min, bbActiveView.Max);
            var bbIntersecFil = new Autodesk.Revit.DB.BoundingBoxIntersectsFilter(outline);
            /*var solid = elem.GetOriginalSolid().ScaleSolid(1.001);*/ // Để va chạm với các đối tượng chạm vô đối tượng đang xét chứ ko join vô
            var solidIntersecFil = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(solidOfView);
            //return new Autodesk.Revit.DB.FilteredElementCollector(revitData.Document
            //    , insElem.Select(x => x.Id).ToList()).WherePasses(bbIntersecFil).WherePasses(solidIntersecFil);
            IEnumerable<Autodesk.Revit.DB.Element> intersectElems = new Autodesk.Revit.DB.FilteredElementCollector(revitData.Document
                , insElem.Select(x => x.Id).ToList())/*.WherePasses(bbIntersecFil)*/.WherePasses(solidIntersecFil).ToList();
            foreach (var item in intersectElems)
            {
                if (elem.Id == item.Id)
                {
                    yes = true;
                    break;

                }
                else
                {
                    yes = false;
                }
            }
            return yes;

        }
        public static Solid CreateSolidFromBBox(this BoundingBoxXYZ inputBb)
        {
            // corners in BBox coords
            XYZ pt0 = new XYZ(inputBb.Min.X, inputBb.Min.Y, inputBb.Min.Z);
            XYZ pt1 = new XYZ(inputBb.Max.X, inputBb.Min.Y, inputBb.Min.Z);
            XYZ pt2 = new XYZ(inputBb.Max.X, inputBb.Max.Y, inputBb.Min.Z);
            XYZ pt3 = new XYZ(inputBb.Min.X, inputBb.Max.Y, inputBb.Min.Z);
            //edges in BBox coords
            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);
            //create loop, still in BBox coords
            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            Double height = inputBb.Max.Z - inputBb.Min.Z;
            CurveLoop baseLoop = CurveLoop.Create(edges);
            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);
            Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);
            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, inputBb.Transform);

            return transformBox;
        }
        public static Solid CreateSolidFromBBox(this Autodesk.Revit.DB.View planView)
        {
            var inputBb = planView.get_BoundingBox(null);
            Autodesk.Revit.DB.Plane planePlanView = planView.SketchPlane.GetPlane();
            Autodesk.Revit.DB.PlanViewRange viewRange = (planView as Autodesk.Revit.DB.ViewPlan).GetViewRange();
            double cutPlaneHeight = viewRange.GetOffset(Autodesk.Revit.DB.PlanViewPlane.CutPlane);
            double height = 300.0.milimeter2Feet();
            XYZ pt0 = PlaneUtil.ProjectOnto(planePlanView, new XYZ(inputBb.Min.X, inputBb.Min.Y, inputBb.Min.Z));
            XYZ pt1 = PlaneUtil.ProjectOnto(planePlanView, new XYZ(inputBb.Max.X, inputBb.Min.Y, inputBb.Min.Z));
            XYZ pt2 = PlaneUtil.ProjectOnto(planePlanView, new XYZ(inputBb.Max.X, inputBb.Max.Y, inputBb.Min.Z));
            XYZ pt3 = PlaneUtil.ProjectOnto(planePlanView, new XYZ(inputBb.Min.X, inputBb.Max.Y, inputBb.Min.Z));
            
            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);

            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            CurveLoop baseLoop = CurveLoop.Create(edges);
            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);
            Solid preTransformBox = null;
            if(cutPlaneHeight>0)
            {
                preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);
            }
            else if(cutPlaneHeight <0)
            {
                preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, -XYZ.BasisZ, height);

            }

            var rliTf = FormData.Instance.SettingView.Setting.RevitLink.GetTotalTransform().Inverse;
            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox,rliTf * inputBb.Transform);
            var tf = Transform.CreateTranslation((transformBox.ComputeCentroid().Z - planView.SketchPlane.GetPlane().Origin.Z - 200.0.Milimet2Feet()) * -XYZ.BasisZ);
            var transformBox1 = SolidUtils.CreateTransformed(transformBox, tf);
            return transformBox1;
            //return preTransformBox;

        }
        public static Solid CreateSolidFromBBox1(this Autodesk.Revit.DB.View planView)
        {
            var inputBb = planView.get_BoundingBox(null);
            Autodesk.Revit.DB.Plane planePlanView = planView.SketchPlane.GetPlane();
            Autodesk.Revit.DB.PlanViewRange viewRange = (planView as Autodesk.Revit.DB.ViewPlan).GetViewRange();
            double cutPlaneHeight = viewRange.GetOffset(Autodesk.Revit.DB.PlanViewPlane.CutPlane);
            double height = 300.0.milimeter2Feet();
            var viewCropRegion = planView.GetCropRegionShapeManager();
            IList<CurveLoop> crops = viewCropRegion.GetCropShape();
            var curveLoops = crops.First();
            Solid virtualSolid = GeometryCreationUtilities.CreateExtrusionGeometry(new CurveLoop[] { curveLoops}, XYZ.BasisZ, height);
            var rliTf = FormData.Instance.SettingView.Setting.RevitLink.GetTotalTransform().Inverse;
            Solid virtualLinkSolid = SolidUtils.CreateTransformed(virtualSolid, rliTf);
            return virtualLinkSolid;

            //var rliTf = FormData.Instance.SettingView.Setting.RevitLink.GetTotalTransform().Inverse;
            //Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, rliTf * inputBb.Transform);
            //var tf = Transform.CreateTranslation((transformBox.ComputeCentroid().Z - planView.SketchPlane.GetPlane().Origin.Z - 200.0.Milimet2Feet()) * -XYZ.BasisZ);
            //var transformBox1 = SolidUtils.CreateTransformed(transformBox, tf);
            //return transformBox1;
            //return preTransformBox;

        }
    }
}
