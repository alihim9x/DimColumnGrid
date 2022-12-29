using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using Autodesk.Revit.DB;
namespace Utility
{
    public static class AnnotationUtil
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
        public static ReferenceArray Ref4Dim (this List<PlanarFace> planarFaces, Model.Entity.Element ettColumnInViewRvl)
        {
            var doc = revitData.Document;
            var setting = modelData.Setting;
            var rvL = setting.RevitLink;
            var ettElemType = ettColumnInViewRvl.ElementType;
            var revitElem = ettColumnInViewRvl.RevitElement;
            var geoElem = ettColumnInViewRvl.RevitElement.get_Geometry(new Options());
            var geoSolid = geoElem.Where(x => x is Solid).FirstOrDefault();
            ReferenceArray refArray = new ReferenceArray();
            foreach (var item in planarFaces)
            {
                try
                {
                    var rvlUniqueId = rvL.UniqueId;
                    var elemIdColumnInViewRvL = ettColumnInViewRvl.RevitElement.Id.ToString();
                    var refElemId = item.Reference.ElementId;
                    string uniqueIdFamSym = null;
                    switch (ettElemType)
                    {
                        case Model.Entity.ElementType.Column:
                            uniqueIdFamSym = (revitElem as FamilyInstance).UniqueId;
                            break;
                        case Model.Entity.ElementType.Wall:
                            uniqueIdFamSym = (revitElem as Autodesk.Revit.DB.Wall).UniqueId;
                            break;
                        default:
                            break;
                    }
                    //var uniqueIdFamSym = (ettColumnInViewRvl.RevitElement as FamilyInstance).UniqueId;
                    var refStable = item.Reference.ConvertToStableRepresentation(rvL.GetLinkDocument());
                    var lastStrNotJoin = refStable.Remove(0, uniqueIdFamSym.Length);
                    var lastStrJoined = refStable.Remove(0, uniqueIdFamSym.Length);
                    var editRFStringNotJoin = $"{rvlUniqueId}:0:RVTLINK:{elemIdColumnInViewRvL}:0:INSTANCE:{refElemId}{lastStrNotJoin}";
                    var editRFStringJoined = $"{rvlUniqueId}:0:RVTLINK:{elemIdColumnInViewRvL}{lastStrJoined}";
                    var rfNotJoin = Reference.ParseFromStableRepresentation(doc, editRFStringNotJoin);
                    var rfJoined = Reference.ParseFromStableRepresentation(doc, editRFStringJoined);
                    if (geoSolid == null)
                    {
                        refArray.Append(rfNotJoin);
                    }
                    else
                    {
                        refArray.Append(rfJoined);
                    }
                }
                catch (Exception)
                {

                    
                }
                
            }
            return refArray;
        }
        public static void CreateDimSpunPileGrid (this Model.Entity.Element ettPileCap)
        {
            var setting = modelData.Setting;
            var dimType = setting.DimensionType;
            var dimSnapDis = dimType.AsValue("Dimension Line Snap Distance").ValueNumber;
            var offsetDim = setting.OffsetDim;
            List<Autodesk.Revit.DB.Element> spunPiles = ettPileCap.SubComponents;
            // đang bí bước tìm origin cho dim 2 phương
            Autodesk.Revit.DB.XYZ originSpunPiles = null;
            Autodesk.Revit.DB.XYZ ooo = new XYZ(0, 0, 0);
            for (int i = 0; i < spunPiles.Count; i++)
            {
                originSpunPiles = ((spunPiles[i] as Autodesk.Revit.DB.FamilyInstance).Location as Autodesk.Revit.DB.LocationPoint).Point;
                for (int j = i+1; j < spunPiles.Count; j++)
                {
                    originSpunPiles = (ooo + ((spunPiles[i] as Autodesk.Revit.DB.FamilyInstance).Location as Autodesk.Revit.DB.LocationPoint).Point);
                }
                break;
                
            }
            ReferenceArray centerFBRefArray = new ReferenceArray();
            ReferenceArray centerLRRefArray = new ReferenceArray();
            List<Reference> centerFBRefs = new List<Reference>();
            List<Reference> centerLRRefs = new List<Reference>();
            List<Plane> planeCFB = new List<Plane>();
            List<Plane> planeCLR = new List<Plane>();
            List<Grid> gridsParallelCLR = new List<Grid>();
            List<Grid> gridsParallelCFB = new List<Grid>();
            Grid gridToDimCLR = null;
            Grid gridToDimCFB = null;
            XYZ centerFBDirection = null;
            XYZ centerLRDirection = null;

            foreach ( var item in spunPiles)
            {
                var centerFB = (item as FamilyInstance).GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();
                var centerLR = (item as FamilyInstance).GetReferences(FamilyInstanceReferenceType.CenterLeftRight).FirstOrDefault();
                centerFBDirection = centerLR.GetReferenceDirection(revitData.Document);
                centerLRDirection = centerFB.GetReferenceDirection(revitData.Document);

                centerFBRefs.Add(centerFB);
                centerLRRefs.Add(centerLR);
                planeCFB.Add(SketchPlane.Create(revitData.Document, centerFB).GetPlane());
                planeCLR.Add(SketchPlane.Create(revitData.Document, centerLR).GetPlane());
            }
            centerFBRefs.RemoveSameDirectionVector();
            centerLRRefs.RemoveSameDirectionVector();
            centerFBRefs.ForEach(x => centerFBRefArray.Append(x));
            centerLRRefs.ForEach(x => centerLRRefArray.Append(x));
            
            //Reference firstCFBRef = centerFBRefs.FirstOrDefault();
            XYZ pFFirstCFBRef = planeCFB.FirstOrDefault().Normal;
            Plane pFFirstCFBPlane = planeCFB.FirstOrDefault();
            Plane pFFirstCLRPlane = planeCLR.FirstOrDefault();

            //Reference firstCLFRef = centerLRRefs.FirstOrDefault();
            XYZ pFFirstCLRRef = planeCLR.FirstOrDefault().Normal;
            
            var gridsInView = revitData.GridsInView.ToList();
            
            try
            {
                gridsParallelCFB = gridsInView.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(pFFirstCFBRef)).ToList();
            }
            catch (Exception)
            {       


            }
            gridsParallelCLR = gridsInView?.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(pFFirstCLRRef)).ToList();


            //gridToDimCFB = gridsParallelCLR.Where(x => (x.Curve as Line).Distance(pFFirstCFBRef)
            //== (gridsParallelCLR.Min(y => Math.Abs((y.Curve as Line).Distance(pFFirstCFBRef))))).FirstOrDefault();
            gridToDimCLR = gridsParallelCLR.Where(x => (x.Curve as Line).Distance(PlaneUtil.ProjectOnto(pFFirstCLRPlane, x.Curve.GetEndPoint(1)))
            == (gridsParallelCLR.Min(y => (y.Curve as Line).Distance(PlaneUtil.ProjectOnto(pFFirstCLRPlane, y.Curve.GetEndPoint(1)))))).FirstOrDefault();
            gridToDimCFB = gridsParallelCFB.Where(x => (x.Curve as Line).Distance(PlaneUtil.ProjectOnto(pFFirstCFBPlane, x.Curve.GetEndPoint(1)))
            == (gridsParallelCFB.Min(y => (y.Curve as Line).Distance(PlaneUtil.ProjectOnto(pFFirstCFBPlane, y.Curve.GetEndPoint(1)))))).FirstOrDefault();

            // xét nếu grid trùng với 2 ref cọc thì ko add grid vào để im, nếu ko trùng thì add vào
            var planeCFBCoincideGrid = planeCFB.Where(x => (x.ProjectOnto(gridToDimCFB.Curve.GetEndPoint(0)).DistanceTo(gridToDimCFB.Curve.GetEndPoint(0)).IsEqual(0))).FirstOrDefault();
            if(planeCFBCoincideGrid == null)
            {
                Reference test = new Reference(gridToDimCFB);
                centerFBRefArray.Append(new Reference(gridToDimCFB));
            }
            var planeCLRCoincideGrid = planeCLR.Where(x => (x.ProjectOnto(gridToDimCLR.Curve.GetEndPoint(0)).DistanceTo(gridToDimCLR.Curve.GetEndPoint(0)).IsEqual(0))).FirstOrDefault();
            if(planeCLRCoincideGrid == null)
            {
                centerLRRefArray.Append(new Reference(gridToDimCLR));
            }
            // đang bí bước tìm origin cho dim 2 phương ... đọc phía trên

            var dimOriginCFB = ettPileCap.Geometry.Origin + offsetDim * centerFBDirection;
            var dimOriginCLR = ettPileCap.Geometry.Origin + offsetDim * centerLRDirection;
            var dimCFBLine = Line.CreateBound(dimOriginCFB, dimOriginCFB + centerLRDirection);
            var dimCLRLine = Line.CreateBound(dimOriginCLR, dimOriginCLR + centerFBDirection);
            if(centerFBRefArray.Size > 1)
            {
                revitData.Document.Create.NewDimension(revitData.ActiveView, dimCFBLine, centerFBRefArray);
            }
            if(centerLRRefArray.Size > 1)
            {
                revitData.Document.Create.NewDimension(revitData.ActiveView, dimCLRLine, centerLRRefArray);
                
            }
        }
        public static void CreateDimPileCapGrid(this Model.Entity.Element ettPileCap)
        {
            var activeView = revitData.ActiveView;

            var doc = revitData.Document;
            var setting = modelData.Setting;
            //var rvL = setting.RevitLink;
            //var tfRvL = rvL.GetTransform();
            var dimType = setting.DimensionType;
            var dimSnapDis = dimType.AsValue("Dimension Line Snap Distance").ValueNumber;
            var offsetDim = setting.OffsetDim;
            var faceEttPileCapinView = ettPileCap.Faces;
            var faceX = faceEttPileCapinView.XFace;
            var faceY = faceEttPileCapinView.YFace;
            var gridsInView = revitData.GridsInView.ToList();
            List<Grid> gridsParallelY = new List<Grid>();
            List<Grid> gridsParallelX = new List<Grid>();
            Grid gridToDimY = null;
            Grid gridToDimX = null;
            ReferenceArray yFaceRefArray = new ReferenceArray();
            ReferenceArray yFaceRefArray1 = new ReferenceArray();
            ReferenceArray xFaceRefArray = new ReferenceArray();
            ReferenceArray xFaceRefArray1 = new ReferenceArray();
            var geoElem = ettPileCap.RevitElement.get_Geometry(new Options());
            var geoSolid = geoElem.Where(x => x is Solid).FirstOrDefault();
            //faceY.ForEach(x => yFaceRefArray.Append(x.Reference));
            //faceX.ForEach(x => xFaceRefArray.Append(x.Reference));
            faceY.ForEach(x => yFaceRefArray1.Append(x.Reference));
            faceX.ForEach(x => xFaceRefArray1.Append(x.Reference));
            try
            {
                gridsParallelY = gridsInView.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(faceY?.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            }
            catch (Exception)
            {

            }
            //gridsParallelY = gridsInView.Where(x => (x.Curve as Line).Direction
            //.IsPerpendicularDirection(faceEttColumninView.YFace?.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            var dimOriginYFace = ettPileCap.Geometry.Origin + (ettPileCap.Geometry.LengthY / 2
                + offsetDim) * faceX.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero); ;
            var dimLineYFace = Line.CreateBound(dimOriginYFace, dimOriginYFace + 5000.0.milimeter2Feet() * faceY.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero));
            //doc.Create.NewDetailCurve(doc.ActiveView, dimLineYFace);
            var dimOriginYFace1 = dimOriginYFace + (dimSnapDis*100).milimeter2Feet() * faceY.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero);
            var dimLineYFace1 = Line.CreateBound(dimOriginYFace1, dimOriginYFace1 + ettPileCap.Geometry.BasisZ);
            gridToDimY = gridsParallelY.Where(x => (x.Curve as Line).Distance(faceY.FirstOrDefault().Origin)
            == (gridsParallelY.Min(y => (y.Curve as Line).Distance(faceY.FirstOrDefault().Origin)))).FirstOrDefault();
            var gridYCoincideFaceY = faceY.Where(x => ((x.GetSurface() as Plane)).ProjectOnto(gridToDimY.Curve.GetEndPoint(0)).DistanceTo(gridToDimY.Curve.GetEndPoint(0)).IsEqual(0)).FirstOrDefault();
            
            try
            {
                if (gridYCoincideFaceY == null)
                {
                    // hiện tại đang dim 1 mặt móng, nếu muốn dim 2 mặt móng bỏ những dòng sau:
                    yFaceRefArray.Clear(); // bỏ
                    
                    yFaceRefArray.Append(new Reference(gridToDimY));
                    
                    foreach (PlanarFace plFace in faceY) // bỏ
                    {
                        if (!(plFace.GetSurface() as Plane).ProjectOnto(gridToDimY.Curve.GetEndPoint(0)).DistanceTo(gridToDimY.Curve.GetEndPoint(0)).IsEqual(0)) // bỏ
                        {
                            
                            yFaceRefArray.Append(plFace.Reference); // bỏ
                            
                        }
                        break; // bỏ
                    }

                }

            }
            catch (Exception)
            {


            }
            //yFaceRefArray.Append(new Reference(gridToDimY));

            gridsParallelX = gridsInView?.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(faceX.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            var dimOriginXFace = ettPileCap.Geometry.Origin + (ettPileCap.Geometry.LengthY / 2
                + offsetDim) * faceY.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero); ;
            var dimOriginXFace1 = dimOriginXFace + (dimSnapDis * 100).milimeter2Feet() * faceX.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero);
            var dimLineXFace = Line.CreateBound(dimOriginXFace, dimOriginXFace + 5000.0.milimeter2Feet() * faceX.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero));
            //doc.Create.NewDetailCurve(doc.ActiveView, dimLineXFace);

            var dimLineXFace1 = Line.CreateBound(dimOriginXFace1, dimOriginXFace1 + ettPileCap.Geometry.BasisX);
            gridToDimX = gridsParallelX.Where(x => (x.Curve as Line).Distance(faceX.FirstOrDefault().Origin)
            == (gridsParallelX.Min(y => (y.Curve as Line).Distance(faceX.FirstOrDefault().Origin)))).FirstOrDefault();
            var gridXCoincideFaceX = faceX.Where(x => ((x.GetSurface() as Plane)).ProjectOnto(gridToDimX.Curve.GetEndPoint(0)).DistanceTo(gridToDimX.Curve.GetEndPoint(0)).IsEqual(0)).FirstOrDefault();

            //doc.Create.NewDetailCurve(doc.ActiveView, Line.CreateBound(ettPileCap.Geometry.Origin, ettPileCap.Geometry.Origin + 5000.0.milimeter2Feet() * ettPileCap.Geometry.BasisZ));
            //doc.Create.NewDetailCurve(doc.ActiveView, Line.CreateBound(ettPileCap.Geometry.Origin, ettPileCap.Geometry.Origin + 5000.0.milimeter2Feet() * ettPileCap.Geometry.BasisX));

            string tesst = null;
            //try
            //{
            if (gridXCoincideFaceX == null)
            {
                // hiện tại đang dim 1 mặt móng, nếu muốn dim 2 mặt móng bỏ những dòng sau:

                xFaceRefArray.Clear();
                xFaceRefArray.Append(new Reference(gridToDimX));
                foreach (PlanarFace plFace in faceX) // bỏ
                {
                    tesst = plFace.Reference.ConvertToStableRepresentation(revitData.Document);
                    if (!(plFace.GetSurface() as Plane).ProjectOnto(gridToDimX.Curve.GetEndPoint(0)).DistanceTo(gridToDimX.Curve.GetEndPoint(0)).IsEqual(0)) // bỏ
                    {
                        xFaceRefArray.Append(plFace.Reference); // bỏ
                        
                    }
                    break; // bỏ
                }


            }
            
            if ((Math.Abs(faceX[0].Origin.Y - (gridToDimX.Curve as Line).Origin.Y) + Math.Abs((gridToDimX.Curve as Line).Origin.Y - faceX[1].Origin.Y)).IsEqual(Math.Abs(faceX[0].Origin.Y - faceX[1].Origin.Y))
                && (!(gridToDimX.Curve as Line).Origin.Y.IsEqual(faceX[0].Origin.Y) && !(gridToDimX.Curve as Line).Origin.Y.IsEqual(faceX[1].Origin.Y)))
            {

                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace, xFaceRefArray, dimType);
                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace1, xFaceRefArray1, dimType);
            }
            else
            {
                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace, xFaceRefArray, dimType);
            }
            if ((Math.Abs(faceY[0].Origin.X - (gridToDimY.Curve as Line).Origin.X) + Math.Abs((gridToDimY.Curve as Line).Origin.X - faceY[1].Origin.X)).IsEqual(Math.Abs(faceY[0].Origin.X - faceY[1].Origin.X))
                && (!(gridToDimY.Curve as Line).Origin.X.IsEqual(faceY[0].Origin.X) && !(gridToDimY.Curve as Line).Origin.X.IsEqual(faceY[1].Origin.X)))
            {
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace1, yFaceRefArray1, dimType);
            }
            else
            {
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);

            }


            //}
            //catch (Exception)
            //{

            //}
            //var dimY = doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);



        }
        public static void CreateDimColumnGrid(this Model.Entity.Element ettColumnInViewRvl)
        {
            var activeView = revitData.ActiveView;

            var doc = revitData.Document;
            var setting = modelData.Setting;
            var rvL = setting.RevitLink;
            var tfRvL = rvL.GetTransform();
            var dimType = setting.DimensionType;
            var dimSnapDis = dimType.AsValue("Dimension Line Snap Distance").ValueNumber;
            var offsetDim = setting.OffsetDim;
            var faceEttColumninView = ettColumnInViewRvl.Faces;
            var faceX = faceEttColumninView.XFace;
            var faceY = faceEttColumninView.YFace;
            var gridsInView = revitData.GridsInView.ToList();
            List<Grid> gridsParallelY = new List<Grid>();
            List<Grid> gridsParallelX = new List<Grid>();
            Grid gridToDimY = null;
            Grid gridToDimX = null;
            ReferenceArray yFaceRefArray = new ReferenceArray();
            ReferenceArray yFaceRefArray1 = new ReferenceArray();
            ReferenceArray xFaceRefArray = new ReferenceArray();
            ReferenceArray xFaceRefArray1 = new ReferenceArray();
            var geoElem = ettColumnInViewRvl.RevitElement.get_Geometry(new Options());
            var geoSolid = geoElem.Where(x => x is Solid).FirstOrDefault();
            yFaceRefArray = faceY.Ref4Dim(ettColumnInViewRvl);
            xFaceRefArray = faceX.Ref4Dim(ettColumnInViewRvl);
            yFaceRefArray1 = faceY.Ref4Dim(ettColumnInViewRvl);
            xFaceRefArray1 = faceX.Ref4Dim(ettColumnInViewRvl);
            try
            {
                gridsParallelY = gridsInView.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(faceY?.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            }
            catch (Exception)
            {


            }
            //gridsParallelY = gridsInView.Where(x => (x.Curve as Line).Direction
            //.IsPerpendicularDirection(faceEttColumninView.YFace?.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            var dimOriginYFace = tfRvL.OfPoint(ettColumnInViewRvl.Geometry.Origin + (ettColumnInViewRvl.Geometry.LengthY / 2
                + offsetDim) * ettColumnInViewRvl.Geometry.BasisY);
            var dimLineYFace = Line.CreateBound(dimOriginYFace, dimOriginYFace + ettColumnInViewRvl.Geometry.BasisX);
            var dimOriginYFace1 = dimOriginYFace + (dimSnapDis * 100).milimeter2Feet() * ettColumnInViewRvl.Geometry.BasisY;
            var dimLineYFace1 = Line.CreateBound(dimOriginYFace1, dimOriginYFace1 + ettColumnInViewRvl.Geometry.BasisX);
            gridToDimY = gridsParallelY.Where(x => (x.Curve as Line).Distance(faceY.FirstOrDefault().Origin)
            == (gridsParallelY.Min(y => (y.Curve as Line).Distance(faceY.FirstOrDefault().Origin)))).FirstOrDefault();
            var gridYCoincideFaceY = faceY.Where(x => (x.Origin.X - (gridToDimY.Curve as Line).Origin.X).IsEqual(0)).SingleOrDefault();
            try
            {
                if (gridYCoincideFaceY == null)
                {
                    yFaceRefArray.Append(new Reference(gridToDimY));

                }

            }
            catch (Exception)
            {


            }
            //yFaceRefArray.Append(new Reference(gridToDimY));

            gridsParallelX = gridsInView?.Where(x => (x.Curve as Line).Direction
            .IsPerpendicularDirection(faceX.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero))).ToList();
            var dimOriginXFace = tfRvL.OfPoint(ettColumnInViewRvl.Geometry.Origin + (ettColumnInViewRvl.Geometry.LengthX / 2
                + offsetDim) * -ettColumnInViewRvl.Geometry.BasisX);
            var dimOriginXFace1 = dimOriginXFace + (dimSnapDis * 100).milimeter2Feet() * -ettColumnInViewRvl.Geometry.BasisX;
            var dimLineXFace = Line.CreateBound(dimOriginXFace, dimOriginXFace + ettColumnInViewRvl.Geometry.BasisY);
            var dimLineXFace1 = Line.CreateBound(dimOriginXFace1, dimOriginXFace1 + ettColumnInViewRvl.Geometry.BasisY);
            gridToDimX = gridsParallelX.Where(x => (x.Curve as Line).Distance(faceX.FirstOrDefault().Origin)
            == (gridsParallelX.Min(y => (y.Curve as Line).Distance(faceX.FirstOrDefault().Origin)))).FirstOrDefault();
            var gridXCoincideFaceX = faceX.Where(x => (x.Origin.Y - (gridToDimX.Curve as Line).Origin.Y).IsEqual(0)).SingleOrDefault();

            //try
            //{
            if (gridXCoincideFaceX == null)
            {
                xFaceRefArray.Append(new Reference(gridToDimX));

            }

            if ((Math.Abs(faceX[0].Origin.Y - (gridToDimX.Curve as Line).Origin.Y) + Math.Abs((gridToDimX.Curve as Line).Origin.Y - faceX[1].Origin.Y)).IsEqual(Math.Abs(faceX[0].Origin.Y - faceX[1].Origin.Y))
                && (!(gridToDimX.Curve as Line).Origin.Y.IsEqual(faceX[0].Origin.Y) && !(gridToDimX.Curve as Line).Origin.Y.IsEqual(faceX[1].Origin.Y)))
                {
             
                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace, xFaceRefArray, dimType);
                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace1, xFaceRefArray1, dimType);
                }
            else
                {
                doc.Create.NewDimension(revitData.ActiveView, dimLineXFace, xFaceRefArray, dimType);
                }
            if ((Math.Abs(faceY[0].Origin.X - (gridToDimY.Curve as Line).Origin.X) + Math.Abs((gridToDimY.Curve as Line).Origin.X - faceY[1].Origin.X)).IsEqual(Math.Abs(faceY[0].Origin.X - faceY[1].Origin.X))
                && (!(gridToDimY.Curve as Line).Origin.X.IsEqual(faceY[0].Origin.X) && !(gridToDimY.Curve as Line).Origin.X.IsEqual(faceY[1].Origin.X)))
                {
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace1, yFaceRefArray1, dimType);
                }
            else
                {
                doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);

                }


            //}
            //catch (Exception)
            //{

            //}
            //var dimY = doc.Create.NewDimension(revitData.ActiveView, dimLineYFace, yFaceRefArray, dimType);



        }
    }
            

}
