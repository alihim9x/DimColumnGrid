﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entity;
using Utility;

namespace SingleData
{
    public partial class ModelData : NotifyClass
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        public static ModelData Instance
        {
            get
            {
                return Singleton.Instance.ModelData;
            }
        }
        private List<Model.Entity.Element> ettElements;
        public List<Model.Entity.Element> EttElements
        {
            get
            {
                if(ettElements == null)
                {
                    ettElements = new List<Element>();
                }
                return ettElements;
            }
            set
            {
                ettElements = value;
            }
        }
        private Element element;
        public Element Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
                OnPropertyChanged();
            }
        }
        private Setting setting;
        public Setting Setting
        {
            get
            {
                if (setting == null)
                {
                    setting = new Setting();
                }
                return setting;
            }
        }
        private ActiveView activeView;
        public ActiveView ActiveView
        {
            get
            {
                if(activeView == null)
                {
                    activeView = new ActiveView();
                }
                return activeView;
            }
        }
        public List<Autodesk.Revit.DB.Element> SelectedPileCaps { get; set; }
        //private List<Model.Entity.Rebar> rebars;
        //public List<Model.Entity.Rebar> Rebars
        //{
        //    get
        //    {
        //        if(rebars == null)
        //        {
        //            rebars = new List<Model.Entity.Rebar>();
        //        }
        //        return rebars;
        //    }
        //    set
        //    {
        //        rebars = value;
        //    }
        //}
        //private List<Model.Entity.Stirrup> stirrups;
        //public List<Model.Entity.Stirrup> Stirrups
        //{
        //    get
        //    {
        //        if(stirrups == null)
        //        {
        //            stirrups = new List<Model.Entity.Stirrup>();
        //        }
        //        return stirrups;
        //    }
        //    set
        //    {
        //        stirrups = value;
        //    }
        //}
        //private Setting setting;
        //public Setting Setting
        //{
        //    get
        //    {
        //        if (setting == null)
        //        {
        //            setting = new Setting();
        //        }
        //        return setting;
        //    }
        //}

        //private IEnumerable<Autodesk.Revit.DB.Structure.RebarBarType> rebarBarTypes;
        //public IEnumerable<Autodesk.Revit.DB.Structure.RebarBarType> RebarBarTypes
        //{
        //    get
        //    {
        //        if (rebarBarTypes == null)
        //        {
        //            rebarBarTypes = RevitData.Instance.RebarBarTypes;
        //        }
        //        return rebarBarTypes;
        //    }
        //}

        //private List<RebarFunction> rebarFunctions;
        //public List<RebarFunction> RebarFunctions
        //{
        //    get
        //    {
        //        if (rebarFunctions == null)
        //        {
        //            rebarFunctions = new List<RebarFunction> {Model.Entity.RebarFunction.Straight,Model.Entity.RebarFunction.Right_Reinforce,
        //            Model.Entity.RebarFunction.Middle_Reinforce, Model.Entity.RebarFunction.Left_Reinforce};
        //        }
        //        return rebarFunctions;
        //    }
        //}

        //private List<RebarLayer> rebarLayers;
        //public List<RebarLayer> RebarLayers
        //{
        //    get
        //    {
        //        if (rebarLayers == null)
        //        {
        //            rebarLayers = new List<RebarLayer> {Model.Entity.RebarLayer.Bottom,Model.Entity.RebarLayer.Middle,Model.Entity.RebarLayer.Top };
        //        }
        //        return rebarLayers;
        //    }
        //}

        //private List<StirrupFunction> stirrupFunctions;
        //public List<StirrupFunction> StirrupFunctions
        //{
        //    get
        //    {
        //        if (stirrupFunctions == null)
        //        {
        //            stirrupFunctions = new List<StirrupFunction> {Model.Entity.StirrupFunction.Closed_Type,Model.Entity.StirrupFunction.C_Type };
        //        }
        //        return stirrupFunctions;
        //    }
        //}

        //private List<StirrupPosition> stirrupPositions;
        //public List<StirrupPosition> StirrupPositions
        //{
        //    get
        //    {
        //        if (stirrupPositions == null)
        //        {
        //            stirrupPositions = new List<StirrupPosition>{Model.Entity.StirrupPosition.Whole, Model.Entity.StirrupPosition.Right, Model.Entity.StirrupPosition.Middle,
        //                Model.Entity.StirrupPosition.Left };
        //        }
        //        return stirrupPositions;
        //    }
        //}

        //private List<Autodesk.Revit.DB.Structure.RebarShape> rebarShapes;
        //public List<Autodesk.Revit.DB.Structure.RebarShape> RebarShapes
        //{
        //    get
        //    {
        //        if (rebarShapes == null)
        //        {
        //            rebarShapes = RevitData.Instance.RebarShapes.Where(x => x.Name == "M_00" || x.Name == "M_17" || x.Name == "M_T2" || x.Name == "M_01").ToList();
        //        }
        //        return rebarShapes;
        //    }
        //}
        //private List<Autodesk.Revit.DB.Structure.RebarCoverType> rebarCoverTypes;
        //public List<Autodesk.Revit.DB.Structure.RebarCoverType> RebarCoverTypes
        //{
        //    get
        //    {
        //        if(rebarCoverTypes == null)
        //        {
        //            rebarCoverTypes = RevitData.Instance.RebarCoverTypes.ToList();
        //        }
        //        return rebarCoverTypes;
        //    }
        //}
        private List<Autodesk.Revit.DB.RevitLinkInstance> revitLinks;
        public virtual List<Autodesk.Revit.DB.RevitLinkInstance> RevitLinks
        {
            get
            {
                if(revitLinks == null)
                {
                    revitLinks = revitData.RevitLinks.ToList();
                }
                return revitLinks;
            }
        }
        private List<Autodesk.Revit.DB.IndependentTag> independentTags;
        public virtual List<Autodesk.Revit.DB.IndependentTag> IndependentTags
        {
            get
            {
                if(independentTags == null)
                {
                    independentTags = revitData.IndependentTags.ToList();
                }
                return independentTags;
            }
        }
        private List<Autodesk.Revit.DB.FamilySymbol> structuralRebarTags;
        public virtual List<Autodesk.Revit.DB.FamilySymbol> StructuralRebarTags
        {
            get
            {
                if(structuralRebarTags == null)
                {
                    structuralRebarTags = revitData.StructuralRebarTags.ToList();
                }
                return structuralRebarTags;
            }
        }
        private List<Autodesk.Revit.DB.MultiReferenceAnnotationType> multiRefAnnotationTypes;
        public List<Autodesk.Revit.DB.MultiReferenceAnnotationType> MultiRefAnnotationTypes
        {
            get
            {
                if(multiRefAnnotationTypes == null)
                {
                    multiRefAnnotationTypes = revitData.MultiRefAnnoTypes.ToList();
                }
                return multiRefAnnotationTypes;
            }
        }
        private List<Autodesk.Revit.DB.DimensionType> dimensionTypes;
        public List<Autodesk.Revit.DB.DimensionType> DimensionTypes
        {
            get
            {
                if(dimensionTypes == null)
                {
                    dimensionTypes = RevitData.Instance.DimensionTypes.ToList();
                }
                return dimensionTypes;
            }
        }
    }
}
