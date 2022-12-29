using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.DB;

namespace Utility
{
    public static class ActiveViewUtil
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
        
        public static List<Model.Entity.Element> GetEntityElements(this Model.Entity.ActiveView ettActiveView)
        {
            var setting = ModelData.Instance.Setting;
            List<Model.Entity.Element> ettElements = new List<Model.Entity.Element>();
            var rvLink = setting.RevitLink;
            
            //var rvLFramingInStancesInView = revitData.FramingInstancesInViewRvL?.ToList().FirstOrDefault();
            var rvLColumnInstancesInView = revitData.ColumnInstancesInViewRvL.ToList();
            var rvLWallInstancesInView = revitData.WallInstancesInViewRvL?.ToList();
            //var framingInstanceInView = revitData.FramingInstancesInView?.ToList().FirstOrDefault();
            var columnInstanceInView = revitData.ColumnInstancesInView?.ToList();
            
            if (rvLink == null)
            {
                //if (framingInstanceInView != null)
                //{
                //    ettElements.Add(new Model.Entity.Element { RevitElement = framingInstanceInView });
                //}
               
                if (columnInstanceInView != null)
                {
                    columnInstanceInView.ForEach(x => ettElements.Add(new Model.Entity.Element { RevitElement = x }));
                }
                
            }
            else
            {
                //if (rvLFramingInStancesInView != null)
                //{
                //    ettElements.Add(new Model.Entity.Element { RevitElement = rvLFramingInStancesInView });
                //}
                if (rvLColumnInstancesInView != null)
                {
                    rvLColumnInstancesInView.ForEach(x => ettElements.Add(new Model.Entity.Element { RevitElement = x }));
                }
                if(rvLWallInstancesInView != null)
                {
                    rvLWallInstancesInView.ForEach(x => ettElements.Add(new Model.Entity.Element { RevitElement = x }));
                }
                //if (rvLWallInstancesInView != null)
                //{
                //    rvLWallInstancesInView.ForEach(x => ettElements.Add(new Model.Entity.Element { RevitElement = x }));
                //}
               
            }
            
            return ettElements;
        }
        public static List<Model.Entity.Element> GetEntityElements1(this Model.Entity.ActiveView ettActiveView, Autodesk.Revit.DB.RevitLinkInstance rvL)
        {
            var setting = ModelData.Instance.Setting;
            List<Model.Entity.Element> ettElements = new List<Model.Entity.Element>();
            var rvLink = rvL;

            var rvLFramingInStancesInView = revitData.FramingInstancesInViewRvL?.ToList();
            var rvLFloorInstancesInView = revitData.FloorInstancesInViewRvL?.ToList();
            var framingInstanceInView = revitData.FramingInstancesInView?.ToList().FirstOrDefault();
            var floorInstanceInView = revitData.FloorInstancesInview?.ToList();
            var columnInstanceInView = revitData.ColumnInstancesInView?.ToList().FirstOrDefault();
            if (rvLink == null)
            {
                if (framingInstanceInView != null)
                {
                    ettElements.Add(new Model.Entity.Element { RevitElement = framingInstanceInView });
                }
                if (floorInstanceInView != null)
                {
                    foreach (var item in floorInstanceInView)
                    {
                        if (ElementUtil.IsIntersectWithActiveView(item))
                        {
                            ettElements.Add(new Model.Entity.Element { RevitElement = item });
                        }
                    }
                    //ettElements.Add(new Model.Entity.Element { RevitElement = floorInstanceInView });
                }
                if (columnInstanceInView != null)
                {
                    ettElements.Add(new Model.Entity.Element { RevitElement = columnInstanceInView });
                }
            }
            else
            {
                if (rvLFramingInStancesInView != null)
                {
                    //ettElements.Add(new Model.Entity.Element { RevitElement = rvLFramingInStancesInView });
                    rvLFramingInStancesInView.ForEach(x => ettElements.Add(new Model.Entity.Element { RevitElement = x }));
                }
                if (rvLFloorInstancesInView != null)
                {
                    foreach (var item in rvLFloorInstancesInView)
                    {

                        ettElements.Add(new Model.Entity.Element { RevitElement = item });
                    }
                    //ettElements.Add(new Model.Entity.Element { RevitElement = floorInstanceInView });
                }
            }

            return ettElements;
        }
        //public static List<Model.Entity.Rebar> GetEntityRebars (this Model.Entity.ActiveView ettActiveView)
        //{
        //    var setting = ModelData.Instance.Setting;
        //    var rvLink = setting.RevitLink;
        //    List<Model.Entity.Rebar> ettRebars = new List<Model.Entity.Rebar>();
        //    var curDocRebarsInView = revitData.RebarsInView.ToList();
        //    var rvLinkRebarsInView = revitData.RebarInstancesInViewRvL.ToList();
        //    if(rvLink == null)
        //    {
        //        curDocRebarsInView.ForEach(x => ettRebars.Add(new Model.Entity.Rebar(x)));
        //    }
        //    else
        //    {
        //        rvLinkRebarsInView.ForEach(x => ettRebars.Add(new Model.Entity.Rebar(x)));
        //    }
        //    return ettRebars;
        //}
        public static Autodesk.Revit.DB.Solid CreateCutPlaneSolid (this Autodesk.Revit.DB.View planView)
        {
            Autodesk.Revit.DB.BoundingBoxXYZ bbActiveView = planView.get_BoundingBox(null);
            Autodesk.Revit.DB.Plane planePlanView = planView.SketchPlane.GetPlane();
            Autodesk.Revit.DB.PlanViewRange viewRange = (planView as Autodesk.Revit.DB.ViewPlan).GetViewRange();
            double cutPlaneHeight = viewRange.GetOffset(Autodesk.Revit.DB.PlanViewPlane.CutPlane);

            Autodesk.Revit.DB.XYZ pt0 = new Autodesk.Revit.DB.XYZ(bbActiveView.Min.X, bbActiveView.Min.Y, bbActiveView.Min.Z);
            Autodesk.Revit.DB.XYZ pt1 = new Autodesk.Revit.DB.XYZ(bbActiveView.Max.X, bbActiveView.Min.Y, bbActiveView.Min.Z);
            Autodesk.Revit.DB.XYZ pt2 = new Autodesk.Revit.DB.XYZ(bbActiveView.Max.X, bbActiveView.Max.Y, bbActiveView.Min.Z);
            Autodesk.Revit.DB.XYZ pt3 = new Autodesk.Revit.DB.XYZ(bbActiveView.Min.X, bbActiveView.Max.Y, bbActiveView.Min.Z);

            Autodesk.Revit.DB.XYZ pt00 = PlaneUtil.ProjectOnto(planePlanView, pt0);
            Autodesk.Revit.DB.XYZ pt11 = PlaneUtil.ProjectOnto(planePlanView, pt1);
            Autodesk.Revit.DB.XYZ pt22 = PlaneUtil.ProjectOnto(planePlanView, pt2);
            Autodesk.Revit.DB.XYZ pt33 = PlaneUtil.ProjectOnto(planePlanView, pt3);

            Autodesk.Revit.DB.Line edge00 = Autodesk.Revit.DB.Line.CreateBound(pt00, pt11);
            Autodesk.Revit.DB.Line edge11 = Autodesk.Revit.DB.Line.CreateBound(pt11, pt22);
            Autodesk.Revit.DB.Line edge22 = Autodesk.Revit.DB.Line.CreateBound(pt22, pt33);
            Autodesk.Revit.DB.Line edge33 = Autodesk.Revit.DB.Line.CreateBound(pt33, pt00);

            List<Autodesk.Revit.DB.Curve> edges0 = new List<Autodesk.Revit.DB.Curve>();
            edges0.Add(edge00);
            edges0.Add(edge11);
            edges0.Add(edge22);
            edges0.Add(edge33);

            Autodesk.Revit.DB.CurveLoop baseLoop0 = Autodesk.Revit.DB.CurveLoop.Create(edges0);
            List<Autodesk.Revit.DB.CurveLoop> loopList0 = new List<Autodesk.Revit.DB.CurveLoop>();
            loopList0.Add(baseLoop0);
            Autodesk.Revit.DB.Solid preTransformSolid = Autodesk.Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(loopList0, Autodesk.Revit.DB.XYZ.BasisZ, cutPlaneHeight);
            Autodesk.Revit.DB.Solid transformSolid = Autodesk.Revit.DB.SolidUtils.CreateTransformed(preTransformSolid, bbActiveView.Transform);

            return transformSolid;
            //return preTransformSolid;
            //Autodesk.Revit.DB.BoundingBoxXYZ inputBb = planView.get_BoundingBox(null);
            //Autodesk.Revit.DB.BoundingBoxXYZ bbActiveView = planView.CropBox;
            //Autodesk.Revit.DB.Plane planePlanView = planView.SketchPlane.GetPlane();
            //Autodesk.Revit.DB.PlanViewRange viewRange = (planView as Autodesk.Revit.DB.ViewPlan).GetViewRange();

            ////edges in BBox coords

            ////create loop, still in BBox coords
            ////List<Curve> edges = new List<Curve>();

            ////Double height = viewRange.GetOffset(Autodesk.Revit.DB.PlanViewPlane.CutPlane); 
            ////CurveLoop baseLoop1 = planView.GetCropRegionShapeManager().GetAnnotationCropShape();
            ////List<CurveLoop> loopList = planView.GetCropRegionShapeManager().GetCropShape().ToList();
            //////loopList.Add(baseLoop1);
            ////Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);
            ////Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, inputBb.Transform);

            ////return preTransformBox;
            //return transformBox;


        }
    }
}
