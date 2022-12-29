using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class GeometryUtil
    {
        public static Model.Entity.Geometry GetGeometry(this Model.Entity.Element element)
        {
            var geometry = new Model.Entity.Geometry();
            var revitElem = element.RevitElement;
            var typeElem = element.ElementType;
            Autodesk.Revit.DB.FamilyInstance columnsFi = null;
            Autodesk.Revit.DB.Wall wallFi = null;
            Autodesk.Revit.DB.Transform tfRv = null;
            switch (element.ElementType)
            {
                case Model.Entity.ElementType.Column:
                case Model.Entity.ElementType.PileCap:
                    columnsFi = revitElem as Autodesk.Revit.DB.FamilyInstance;
                    tfRv = columnsFi.GetTotalTransform();
                    break;
                case Model.Entity.ElementType.Wall:
                    wallFi = revitElem as Autodesk.Revit.DB.Wall;
                    break;
                    
                default:
                    throw new Model.Exception.CaseNotCheckException();
            }
            //var columnsFi = revitElem as Autodesk.Revit.DB.FamilyInstance;
           
            Autodesk.Revit.DB.XYZ dir = new Autodesk.Revit.DB.XYZ(0, 0, 1);
            switch(typeElem)
            {
                case Model.Entity.ElementType.PileCap:
                    geometry.BasisX = tfRv.BasisY;
                    geometry.BasisY = tfRv.BasisZ;
                    geometry.BasisZ = tfRv.BasisX;
                    //geometry.BasisX = element.Faces.YFace.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero);
                    //geometry.BasisY = element.Faces.XFace.FirstOrDefault().ComputeNormal(Autodesk.Revit.DB.UV.Zero);

                    geometry.LengthX = columnsFi.Symbol.AsValue("Length").ValueNumber.milimeter2Feet();
                    geometry.LengthY = columnsFi.Symbol.AsValue("Width").ValueNumber.milimeter2Feet();
                    geometry.LengthZ = columnsFi.Symbol.AsValue("Foundation Thickness").ValueNumber.milimeter2Feet();
                    geometry.Origin = tfRv.Origin;
                    break;
                case Model.Entity.ElementType.Framing:
                    geometry.BasisX = tfRv.BasisY;
                    geometry.BasisY = tfRv.BasisZ;
                    geometry.BasisZ = tfRv.BasisX;
                    geometry.LengthX = columnsFi.Symbol.AsValue("b").ValueNumber.milimeter2Feet();
                    geometry.LengthY = columnsFi.Symbol.AsValue("h").ValueNumber.milimeter2Feet();
                    geometry.LengthZ = columnsFi.AsValue("Cut Length").ValueNumber.milimeter2Feet();

                    geometry.Origin = tfRv.Origin - (geometry.BasisY * (geometry.LengthY / 2)) - (geometry.BasisX * (geometry.LengthX / 2))
                        - (geometry.BasisZ*(geometry.LengthZ/2));  // THÊM DI CHUYỂN ORIGIN VỀ ĐIỂM ĐẦU THEO Z
                    geometry.OriginRevit = tfRv.Origin;
                    break;
                case Model.Entity.ElementType.Column:
                    geometry.BasisX = tfRv.BasisX;
                    geometry.BasisY = tfRv.BasisY;
                    geometry.BasisZ = tfRv.BasisZ;
                    geometry.LengthX = columnsFi.Symbol.AsValue("b").ValueNumber.milimeter2Feet();
                    geometry.LengthY = columnsFi.Symbol.AsValue("h").ValueNumber.milimeter2Feet();
                    geometry.Origin = tfRv.Origin;
                    break;
                case Model.Entity.ElementType.Wall:
                    //geometry.BasisX = tfRv.BasisX;
                    //geometry.BasisY = tfRv.BasisY;
                    //geometry.BasisZ = tfRv.BasisZ;
                    var locationLine = (wallFi.Location as Autodesk.Revit.DB.LocationCurve).Curve;
                   

                    if ((locationLine as Autodesk.Revit.DB.Line).Direction.IsXOrY())
                    {
                        geometry.BasisX = (locationLine as Autodesk.Revit.DB.Line).Direction;
                        geometry.BasisY = (locationLine as Autodesk.Revit.DB.Line).Direction.Normalize().CrossProduct(dir);
                        geometry.LengthX = wallFi.AsValue("Length").ValueNumber.milimeter2Feet();
                        geometry.LengthY = wallFi.Width;
                    }
                    else
                    {
                        geometry.BasisX = (locationLine as Autodesk.Revit.DB.Line).Direction.Normalize().CrossProduct(dir);
                        geometry.BasisY = (locationLine as Autodesk.Revit.DB.Line).Direction;
                        geometry.LengthX = wallFi.Width;
                        geometry.LengthY = wallFi.AsValue("Length").ValueNumber.milimeter2Feet(); ;

                    }
                    geometry.LengthZ = wallFi.AsValue("Unconnected Height").ValueNumber.milimeter2Feet();
                    geometry.Origin = wallFi.GetSingleSolid().ComputeCentroid();
                    break;
                default:
                    throw new Model.Exception.CaseNotCheckException();
                  
            }
            return geometry;
        }
    }
}
