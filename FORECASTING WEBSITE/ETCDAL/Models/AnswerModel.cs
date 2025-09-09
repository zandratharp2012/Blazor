using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ETCDAL.Models
{

    


    public class AnswerModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? UserId { get; set; }
        public DateTime? dateSubmitted { get; set; }
        public string? Answer { get; set; }

        public DateTime? AnswerDate { get; set; }
        public int? Deleted { get; set; }

        public int? ETCHeaderId { get; set; }

       
    }
}
