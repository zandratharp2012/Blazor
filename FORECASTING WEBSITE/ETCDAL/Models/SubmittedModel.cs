using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class SubmittedModel
    {
        int ID { get; set; }
       public  int RecordId { get; set; }  
        public DateTime DateSubmitted { get; set; }
        public float SubmittedValue { get; set; }
    }
}
