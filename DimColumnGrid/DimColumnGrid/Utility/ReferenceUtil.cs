using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Utility;

namespace Utility
{
    public static class ReferenceUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        
        public static Autodesk.Revit.DB .Reference  MakeLinkedRef4Dim(this Autodesk.Revit.DB.Reference reference)
        {
            var doc = revitData.Document;
            if (reference.LinkedElementId == Autodesk.Revit.DB.ElementId.InvalidElementId) return null;
            string[] ss = reference.ConvertToStableRepresentation(doc).Split(':');
            string res = string.Empty;
            bool first = true;
            foreach  (string s in ss)
            {
                string t = s;
                if(s.Contains("RVTLINK"))
                {
                    if(res.EndsWith(":0")) { t = "RVTLINK"; }
                    else { t = "0:RVTLINK"; }
                }
                if(!first)
                {
                    res = string.Concat(res, ":", t);
                }
                else
                {
                    res = t;
                    first = false;
                }
            }
            return Autodesk.Revit.DB.Reference.ParseFromStableRepresentation(doc, res);
        }

    }
}
