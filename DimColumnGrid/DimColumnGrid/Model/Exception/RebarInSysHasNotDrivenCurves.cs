using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Exception
{
    class RebarInSysHasNotDrivenCurves : System.Exception
    {
        public override string Message  {get {return "RebarInSystem không có DrivingCurves!"; } } // Ghi đè thuộc tính Message của System.Exception
    }
    
}
