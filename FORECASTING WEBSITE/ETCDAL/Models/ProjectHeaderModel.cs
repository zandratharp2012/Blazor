using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCDAL.Models
{
    public class ProjectHeaderModel
    {
        public int RecordId { get; set; }
        public string? PeriodName { get; set; }
        public string? ProjectManager { get; set; }
        public string? ProjectManagerUserName { get; set; }
        public string? MSO { get; set; }
        public string? Status { get; set; }
        public DateTime? DateApprovalRequested { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ApproverUsername { get; set; }   
        public float Total_Avail_Budget { get; set; }

        public decimal _Total_Avail_Budget
        { get 
            {
                if (CONVERSION_RATE == 0)
                {
                    return (decimal)Total_Avail_Budget;
                }
                {
                    return (decimal)Total_Avail_Budget * CONVERSION_RATE;
                }
            }
        }

        // additional fields, not in ETCHeader table

        public string? PROJECT_NAME { get; set; }
         public string? CUSTOMER_NAME { get; set; }

        public decimal CONVERSION_RATE { get; set; }

        public string? CURRENCY_CODE { get; set; }
          public float OpenPoAmount { get; set; }
        public float UnspentCostBudget { get; set; }
        public string? Period_MonthName { get; set; }
        public string? PeriodStatus { get; set; }
        public string? PeriodMonthName{ get; set; }
        public decimal? TotalExtPrice { get; set; }
        public int? isOver100K { get; set; }

        //public float? SubmittedValue { get; set; }
        public float? TEC_SubmittedValue { get; set; }
      
        public string? ApproversList { get; set; }
        public int? BudgetUpdate { get; set; }
        public DateTime? ProjectCompletionDate { get; set; } 

        public string? Region { get; set; }
        public string ProjectCompletionDateFormatted
        {
            get
            {
                return ProjectCompletionDate?.ToString("MM/dd/yyyy");
            }
        }
        public DateTime? PlannedCompletionDate { get; set; }
        public string PlannedCompletionDateFormatted
        {
            get
            {
                return PlannedCompletionDate?.ToString("MM/dd/yyyy");
            }
        }

        public decimal EAC_REVENUE_BUDGET { get; set; }
     
        public decimal _EAC_REVENUE_BUDGET
             {
            get
            {
                if (CONVERSION_RATE == 0)
                {
                    return 1;
                }
                {
                    return Math.Round(EAC_REVENUE_BUDGET * CONVERSION_RATE,2);
                }
            }
        }

        public decimal EAC_COST_BUDGET { get; set; }

        public decimal _EAC_COST_BUDGET
        {
            get
            {
                if (CONVERSION_RATE == 0)
                {
                    return 1;
                }
                {
                    return Math.Round(EAC_COST_BUDGET * CONVERSION_RATE,2);
                }

            }

        }
        public decimal marginValue
        {
            get
            {
                return _EAC_REVENUE_BUDGET - _EAC_COST_BUDGET;
            }
        }

        public decimal MarginPercent
        {
            get
            {
                if ( EAC_REVENUE_BUDGET == 0)
                {
                    return 1; // or handle as needed
                }
                return Math.Round((marginValue / _EAC_REVENUE_BUDGET) * 100,2);
            }
        }
    }
   

}
