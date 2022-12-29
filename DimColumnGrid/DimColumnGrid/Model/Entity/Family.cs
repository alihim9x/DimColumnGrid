using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Model.Entity
{
    public class Family
    {

        public Model.Entity.FamilyDocument FamilyDocument { get; set; }
        public Autodesk.Revit.DB.Family RevitFamily { get; set;}
        private IEnumerable<Autodesk.Revit.DB.FamilySymbol> familySymbols;
        public IEnumerable<Autodesk.Revit.DB.FamilySymbol> FamilySymbols
        {
            get
            {
                if(familySymbols == null)
                {
                    familySymbols = this.GetFamilySymbols();
                }
                return familySymbols;
            }
        }
        private Autodesk.Revit.DB.FamilySymbol defaultFamilySymbol;
        public Autodesk.Revit.DB.FamilySymbol DefaultFamilySymbol
        {
            get
            {
                if(defaultFamilySymbol == null)
                {
                    defaultFamilySymbol = FamilySymbols.First();
                    if (!defaultFamilySymbol.IsActive) defaultFamilySymbol.Activate();
                }
                return defaultFamilySymbol;
            }
        }
    }
}
