using ETCDAL.Models;

namespace ETCDAL
{
    public interface IETCData
    {
        Task<List<ProjectHeaderModel>> GetProjectHeader(int? RecordId, bool Admin,bool ReadAll, List<string?> UserList, List<string?> RegionList, string userChoice);
        Task UpdateProjectHeader(ProjectHeaderModel projectHeader);
        Task<List<ProjectHeaderModel>> GetTotalAvailableBudget(int RecordId);
        Task<List<ProjectHeaderModel>> GetApprovalHeaderData(string? MSO, string? ADUsername, bool Admin, bool ReadAll, List<string?> RegionList);
        Task AddApprover(int RecordId, string ApproverUserName);
        Task UpdateApproval(int RecordId, string ApproverUserName);
        Task DeleteExistingApprovers(int RecordId);
        Task<List<ApprovalModel>> GetApprovalList(int RecordId);
        Task<List<TaskModel>> GetTaskList(int? RecordId);
        Task  SaveTaskList(string sql);
        Task InsertActivityLog(string id, string message, string? Notes);
        Task<List<ActivityLogModel>> GetActivityLog(string? RecordId);

        Task<List<PermissionModel>> GetPermissions(string ADUsername);
        Task<List<UserModel>> GetUserDetails(string ADUsername);


        Task<List<SubmittedModel>> GetSubmissions();
        Task AddSubmission(int RecordId, float? SubmittedValue, float? TECSubmittedValue);
        Task<List<PeriodsModel>> GetPeriods(string ADUsername);
        Task UpdatePeriodStatusOpen(PeriodsModel period);
        Task UpdatePeriodStatusClose(PeriodsModel period);

        Task<List<ManagerModel>> GetManagerStructure(string ADUsername);
        Task<List<EngineersModel>> GetEngineerRates();
        Task InsertPAActivityLog(string period, string message);
        Task<List<ActivityLogModel>> GetPAActivityLog(string? PAPeriod);
        Task UpdateTaskDetailID(float? TaskPeriodValue, int? TaskDetailID);
        Task<List<QuestionModel>> GetQuestions();
        Task SaveAnswer(AnswerModel answer);

        Task ClearAnswers(int RecordId);
        Task<List<QuestionAnswerDisplayModel>> GetAnswers(int HeaderId);
        Task UpdatePCDate(int RecordId, DateTime? PlannedCompletionDate);
        Task<List<UserRegionsModel>> GetUserRegions(string ADUsername);
        Task UpdateTotalCost(int RecordId, float TotalCost);
        Task UpdateRejected(int RecordId, string ApproverUserName);

        Task InsertLog(string username, string componentName, string additional);
        //Task<bool> AllQuestionsAnswered(int RecordId);
    }
}