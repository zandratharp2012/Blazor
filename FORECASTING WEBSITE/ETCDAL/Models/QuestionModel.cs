using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class QuestionModel
    {
public int id { get; set; }
        public string? QuestionText { get; init; }
        public string? QuestionType { get; init; }
        public int? Active { get; init; }
        public int? DisplayOrder { get; init; }
        public bool AllAnswered_Flag { get; set; }
    }
}
