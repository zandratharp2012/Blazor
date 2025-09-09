using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class ApprovalModel
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string? ApproverUserName { get; set; }
        public DateTime DateApprovalRequested { get; set;}

        public DateTime DateApproved { get; set;}

        public int Approved { get; set;}

        public int Rejected { get; set;}
        public DateTime DateRejected { get; set; }
    }
}
