using ETCDAL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.JSInterop;

namespace ETC.TaskListService
{
    public class TaskListService
    {

        public bool IsBudgetThresholdExceeded(float? BudgetThreshold)
        {
            return BudgetThreshold < -25000 || BudgetThreshold > 25000;
        }

        public bool IsAmericanProject(ProjectHeaderModel projectHeader)
        {
                if (projectHeader.MSO.StartsWith("NA") || projectHeader.MSO.StartsWith("LA"))
                {
                    return true;
                }
            
            return false;
        }

        public bool IsMarginChangeExceeded(decimal calculatedMargin, decimal marginPercent)
        {
            return (marginPercent -calculatedMargin  ) > 5;
        }


        public bool IsRecordLocked(ProjectHeaderModel projectHeader)
        {
            return projectHeader.Status == "Submitted" || projectHeader.Status == "Approved" || projectHeader.Status == "Approval Pending";
        }

        public bool HasPermission(List<PermissionModel> Permissions)
        {
            return Permissions.Any(p => p.Permission == "Admin" || p.Permission == "Approver" || p.Permission == "ReadAll");
        }

        public decimal CalculateMargin(ProjectHeaderModel projectHeader, decimal BudgetThreshold)
        {
            decimal _ConversionRate = 1;
            decimal NewBudget = 0;
            decimal CalculatedMargin = 0;
            decimal _BudgetThreshold = 1;

            if (projectHeader.CONVERSION_RATE != 0)
            {
                _ConversionRate = projectHeader.CONVERSION_RATE;
            }
                       
                _BudgetThreshold = Math.Round(Convert.ToDecimal(BudgetThreshold) * _ConversionRate, 2);

                NewBudget = projectHeader.marginValue + _BudgetThreshold;
                CalculatedMargin = Math.Round((NewBudget/projectHeader._EAC_REVENUE_BUDGET ) * 100, 2);

            return CalculatedMargin;
        }

        public async Task<SubmissionValidationResult> ValidateSubmissionAsync(ProjectHeaderModel projectHeader,float? BudgetThreshold,decimal CalculatedMargin,IJSRuntime JsRuntime)
        {
            if (IsBudgetThresholdExceeded(BudgetThreshold) && IsAmericanProject(projectHeader))
            {
                string budgetStatus = BudgetThreshold < -25000 ? "Over" : "Under";
                string warningMessage =
                    $"**********************************************************************\n" +
                       $"WARNING: FORECAST DOES NOT MEET BUDGET REQUIREMENTS.\n" +
                       $"**********************************************************************\n" +
                       $"BUDGET IS {budgetStatus} BY:\n{String.Format("{0:C2}", BudgetThreshold)}\n\n" +
                       "Click OK to confirm you have reviewed forecast entries and wish to proceed with submission OR click CANCEL to cancel the submission.";
            
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", warningMessage);
                return new SubmissionValidationResult(confirmed, confirmed);
            }

            if (IsMarginChangeExceeded(CalculatedMargin, projectHeader.MarginPercent) && !IsAmericanProject(projectHeader))
            {
                string warningMessage = $"**********************************************************************\n" +
                                           $"WARNING: FORECAST DOES NOT MEET BUDGET REQUIREMENTS.\n" +
                                           $"**********************************************************************\n" +
                                           $"MARGIN HAS CHANGED BY: {CalculatedMargin - projectHeader.MarginPercent} %\n" +
                                           "Click OK to confirm you have reviewed forecast entries and wish to proceed with submission OR click CANCEL to cancel the submission.";
                bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", warningMessage);
                return new SubmissionValidationResult(confirmed, confirmed);
            }

            bool finalConfirmation = await JsRuntime.InvokeAsync<bool>(
                "confirm",
                "Click OK to confirm you have reviewed forecast entries and wish to proceed with submission. Click CANCEL to cancel the submission."
            );

            return new SubmissionValidationResult(finalConfirmation, false);
        }
    }

    public class SubmissionValidationResult
    {
        public bool IsConfirmed { get; }
        public bool SubmitToApproval { get; }

        public SubmissionValidationResult(bool isConfirmed, bool submitToApproval)
        {
            IsConfirmed = isConfirmed;
            SubmitToApproval = submitToApproval;
        }
    }

}

