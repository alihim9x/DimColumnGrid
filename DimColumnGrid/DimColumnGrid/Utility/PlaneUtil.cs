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
    public static class PlaneUtil
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
        public static double SignedDistanceTo(this Autodesk.Revit.DB.Plane plane, Autodesk.Revit.DB.XYZ p)
        {
            Debug.Assert(GeomUtil.IsEqual(plane.Normal.GetLength(), 1), "Expected normalised plane normal");
            Autodesk.Revit.DB.XYZ v = p - plane.Origin;
            return plane.Normal.DotProduct(v);
        }
        public static Autodesk.Revit.DB.XYZ ProjectOnto(this Autodesk.Revit.DB.Plane plane, Autodesk.Revit.DB.XYZ p)
        {
            double d = plane.SignedDistanceTo(p);
            Autodesk.Revit.DB.XYZ q = p - d * plane.Normal;

            Debug.Assert(GeomUtil.IsZero(plane.SignedDistanceTo(q)), "Expected point on a plane to have zero distance to plan");
            return q;
        }
        public static Autodesk.Revit.DB.UV ProjectInto(this Autodesk.Revit.DB.Plane plane, Autodesk.Revit.DB.XYZ p)
        {
            Autodesk.Revit.DB.XYZ q = plane.ProjectOnto(p);
            Autodesk.Revit.DB.XYZ o = plane.Origin;
            Autodesk.Revit.DB.XYZ d = q - o;
            double u = d.DotProduct(plane.XVec);
            double v = d.DotProduct(plane.YVec);
            return new Autodesk.Revit.DB.UV(u, v);
        }public static List<Autodesk.Revit.DB.Reference> RemoveSameDirectionVector (this List<Autodesk.Revit.DB.Reference> listRef)
        {
            Autodesk.Revit.DB.Plane currentPlane = null;
            Autodesk.Revit.DB.Plane checkedPlane = null;
            double check = 0;
            List<Autodesk.Revit.DB.Reference> newListRef = new List<Autodesk.Revit.DB.Reference>();
            for (int i = 0; i < listRef.Count; i++)
            {
                currentPlane = Autodesk.Revit.DB.SketchPlane.Create(revitData.Document, listRef[i]).GetPlane();
                for (int j = i + 1;  j < listRef.Count; j++)
                {
                    checkedPlane = Autodesk.Revit.DB.SketchPlane.Create(revitData.Document, listRef[j]).GetPlane();
                    check = currentPlane.ProjectOnto(checkedPlane.Origin).DistanceTo(checkedPlane.Origin);
                    if(check.IsEqual(0))
                    {
                        listRef.RemoveAt(j);
                    }
                }
            }

            return newListRef;
            
        }
        public static Autodesk.Revit.DB.XYZ GetReferenceDirection (this Autodesk.Revit.DB.Reference ref1, Autodesk.Revit.DB.Document doc)
        {
            Autodesk.Revit.DB.XYZ res = Autodesk.Revit.DB.XYZ.Zero;
            Autodesk.Revit.DB.XYZ wworkPlaneNormal = doc.ActiveView.SketchPlane.GetPlane().Normal;
            if (ref1.ElementId == Autodesk.Revit.DB.ElementId.InvalidElementId) return res;
            Autodesk.Revit.DB.Element elem = doc.GetElement(ref1.ElementId);
            if (elem == null) return res;
            if(ref1.ElementReferenceType == Autodesk.Revit.DB.ElementReferenceType.REFERENCE_TYPE_SURFACE ||
                ref1.ElementReferenceType == Autodesk.Revit.DB.ElementReferenceType.REFERENCE_TYPE_LINEAR)
            {
                Autodesk.Revit.DB.XYZ bEnd = new Autodesk.Revit.DB.XYZ(10, 10, 10);
                Autodesk.Revit.DB.ReferenceArray refArr = new Autodesk.Revit.DB.ReferenceArray();
                refArr.Append(ref1);
                Autodesk.Revit.DB.Dimension dim = null;
                revitData.Transaction.Commit();
                using (Autodesk.Revit.DB.Transaction t = new Autodesk.Revit.DB.Transaction(doc, "test1"))
                {
                    t.Start();
                    using (Autodesk.Revit.DB.SubTransaction st = new Autodesk.Revit.DB.SubTransaction(doc))
                    {
                        st.Start();
                        Autodesk.Revit.DB.ReferencePlane refPlane = doc.Create.NewReferencePlane(Autodesk.Revit.DB.XYZ.Zero,
                            bEnd, bEnd.CrossProduct(Autodesk.Revit.DB.XYZ.BasisZ).Normalize(), doc.ActiveView);
                        Autodesk.Revit.DB.ModelCurve mc = doc.Create.NewModelCurve(Autodesk.Revit.DB.Line.CreateBound(Autodesk.Revit.DB.XYZ.Zero,
                            new Autodesk.Revit.DB.XYZ(10, 10, 10)), Autodesk.Revit.DB.SketchPlane.Create(doc, refPlane.Id));
                        refArr.Append(mc.GeometryCurve.GetEndPointReference(0));
                        dim = doc.Create.NewDimension(doc.ActiveView, Autodesk.Revit.DB.Line.CreateBound(Autodesk.Revit.DB.XYZ.Zero,
                            new Autodesk.Revit.DB.XYZ(10, 0, 0)), refArr);
                        Autodesk.Revit.DB.ElementTransformUtils.MoveElement(doc, dim.Id, new Autodesk.Revit.DB.XYZ(0, 0.1, 0));
                        st.Commit();
                    }
                    if(dim != null)
                    {
                        Autodesk.Revit.DB.Curve cv = dim.Curve;
                        cv.MakeBound(0, 1);
                        Autodesk.Revit.DB.XYZ pt1 = cv.GetEndPoint(0);
                        Autodesk.Revit.DB.XYZ pt2 = cv.GetEndPoint(1);
                        res = pt2.Subtract(pt1).Normalize();
                    }
                    t.RollBack();
                }
            }
            revitData.Transaction.Start();
            return res;
        }
    }
}
