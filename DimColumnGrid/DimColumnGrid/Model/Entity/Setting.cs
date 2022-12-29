using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;

namespace Model.Entity
{
    public class Setting : NotifyClass
    {
       
        private Autodesk.Revit.DB.DimensionType dimensionType;
        public Autodesk.Revit.DB.DimensionType DimensionType
        {
            get
            {
                return dimensionType;
            }
            set
            {
                if (dimensionType == value) return;
                dimensionType = value;
                OnPropertyChanged();

            }
        }
        private double offsetDim;
        public double OffsetDim
        {
            get
            {
                if(offsetDim == 0)
                {
                    offsetDim = 400.0.milimeter2Feet();
                }
                return offsetDim;
            }
            set
            {
                if (offsetDim == value) return;
                offsetDim = value; OnPropertyChanged();
            }
        }
        
       
        private Autodesk.Revit.DB.RevitLinkInstance revitLink;
        public virtual Autodesk.Revit.DB.RevitLinkInstance RevitLink
        {
            get
            {
                return revitLink;
            }
            set
            {
                if (revitLink == value) return;
                revitLink = value;
                OnPropertyChanged();
            }
        }
        //public virtual Autodesk.Revit.DB.RevitLinkInstance RevitLink { get; set; }
    }
}
