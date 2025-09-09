using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class EngineersModel
    {
        public int EngineerID { get; set; }
        public string EngineerType { get; set; }
        public float EngineerRate { get; set; }
        public string FY { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Hours{ get; set; }

        public float ExpenseReports { get; set; }
        public float VendorReqs { get; set; }

    }
}
