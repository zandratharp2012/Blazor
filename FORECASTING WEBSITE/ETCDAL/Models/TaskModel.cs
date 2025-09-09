using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class TaskModel
    {
        public string? TaskName { get; set; }
        public int? TaskId { get; set; }
        public int? TaskDetailId { get; set; }
        public string? TaskNumber { get; set; }
        public string? PeriodName { get; set; }
        public float? PeriodValue { get; set; }
        public float? SumBudgeted_Burdened_Cost { get; set; }

        public float? ITDCost { get; set; } = 0;
        public float? AvailableBudget { get; set; } = 0;
        public float? TotalEstimatedCost { get; set; } = 0;
        public string? Period_MonthName { get; set; }
        public float? SumVendor_Cost { get; set; } = 0;
        public float? UnallocatedAvailBudget { get; set; } = 0;
        public float? QTD { get; set; } = 0;
        public bool? ZeroRow { get; set; }

        public float? BudgetRemaining { get; set; } = 0;
        public bool IsModified { get; set; } = false;
        // Constructor
        public TaskModel()
        {
            RecalculateZeroRow();
        }

        public void RecalculateZeroRow()
        {
            ZeroRow = (ITDCost == 0 && AvailableBudget == 0 && SumVendor_Cost == 0);
        }

    }



    
}
