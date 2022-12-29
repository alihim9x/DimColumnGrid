using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Face
    {
        public List<Autodesk.Revit.DB.PlanarFace> XFace { get; set; } = new List<Autodesk.Revit.DB.PlanarFace>();
        public List<Autodesk.Revit.DB.PlanarFace> YFace { get; set; } = new List<Autodesk.Revit.DB.PlanarFace>();

       
    }
}
