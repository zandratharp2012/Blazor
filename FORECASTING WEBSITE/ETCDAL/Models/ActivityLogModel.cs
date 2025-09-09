using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class ActivityLogModel
    {
        public int ID { get; set; } 
        public string ProjectId { get; set; }
        public string MessageText { get; set; }
        public DateTime DateAdded { get; set; }
        public string? Notes { get; set; }
        public string PAPeriod { get; set;}
    }
}
