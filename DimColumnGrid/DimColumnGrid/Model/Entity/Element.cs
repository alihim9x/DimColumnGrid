using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using Autodesk.Revit.DB.Structure;

namespace Model.Entity
{
    public class Element : NotifyClass
    {
        
        public Autodesk.Revit.DB.Element RevitElement { get; set; }
        private List<Autodesk.Revit.DB.Element> subComponents;
        public List<Autodesk.Revit.DB.Element> SubComponents
        {
            get
            {
                if (subComponents == null)
                {
                    subComponents = this.GetSubComponent();
                }
                return subComponents;
            }
        }
        
        private Model.Entity.Face face;
        public Model.Entity.Face Faces
        {
            get
            {
                if (face == null)
                {
                    face = this.GetFaces();
                }
                return face;
            }
        }
        
        private ElementType? elementType;
        public ElementType? ElementType
        {
            get
            {
                if (elementType == null)
                {
                    elementType = this.GetElementType();
                }
                return elementType;
            }
        }
        private Geometry geometry;
        public Geometry Geometry
        {
            get
            {
                if(geometry == null)
                {
                    geometry = this.GetGeometry();
                }
                return geometry;
            }
        }
    }

       
}
