using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingleData;
using System.Threading.Tasks;
using Utility;


namespace Model.Entity
{
    public class ActiveView
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        
        public Autodesk.Revit.DB.View RevitActiveView
        {
            get
            {
                return revitData.UIDocument.ActiveView;
            }
        }
        private List<Model.Entity.Element> entityElements;
        public List<Model.Entity.Element> EntityElements
        {
            get
            {
                entityElements = this.GetEntityElements();
                return entityElements;  
            }
        }
        
    }
}
