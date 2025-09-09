using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ETCDAL.Models;
using Microsoft.Extensions.Primitives;
using Xunit;
using static System.Net.WebRequestMethods;
namespace ETCDAL
{
    public class ETCData : IETCData

    {
        private readonly ISQLDataAccess _db; 

        public ETCData(ISQLDataAccess db)
        {
            _db = db;
        }


        public Task<List<ProjectHeaderModel>> GetProjectHeader(int? RecordId, bool Admin,bool ReadAll, List<string?> UserList, List<string?> RegionList, string userChoice)
        {
            string userlistSQL = "''";
            foreach (string user in UserList)
            {
                userlistSQL = userlistSQL + ",'" + user + "'";
            }

            string regionlistSQL = "''";
            foreach (string region in RegionList)
            {
                regionlistSQL = regionlistSQL + ",'" + region + "'";
            }

            string sql = "";
            _db.ConnectionStringName = "ETC";
            sql = @"select 
                    etc.RecordID,
                    p.PeriodName,
                    p.PeriodMonthName,
                    p.Status as PeriodStatus,
                    etc.ProjectManager,
                    etc.ProjectManagerUsername,
                    etc.DateGenerated,
                    etc.MSO,
                    etc.Status,
                    etc.DateApprovalRequested,
                    etc.DateApproved,
                    etc.ApproverUsername,
                    etc.PeriodID, 
                    a.Project_Name,
                    a.CUSTOMER_NAME,
                    etc.CONVERSION_RATE,
                    etc.EAC_REVENUE_BUDGET,
                    etc.EAC_COST_BUDGET,
                    etc.Currency_Code,
                    etc.projectcompletiondate,
                    etc.plannedcompletiondate,
                    etc.Region,
                    case when ab.Total_Avail_Budget is null then 0 else ab.Total_Avail_Budget end Total_Avail_Budget, 
                    Case when etc.EAC_REVENUE_BUDGET Is null then 0 else etc.EAC_REVENUE_BUDGET end AS TotalExtPrice,
                    Case when (etc.EAC_REVENUE_BUDGET > 100000 and etc.EAC_REVENUE_BUDGET <= 1000000) then 1 
                    when etc.EAC_REVENUE_BUDGET > 1000000 then 2 else 0 end as isOver100K,
                    etc.TotalCost as TEC_SubmittedValue,
                    (CASE WHEN CONVERT(int, ab.Total_Avail_Budget) IS NULL THEN 0 ELSE CONVERT(int, ab.Total_Avail_Budget) 
                    END) - (CASE WHEN CONVERT(int, etc.TotalCost) IS NULL THEN 0 ELSE CONVERT(int, etc.TotalCost) END) as BudgetUpdate
                    from ETCHeader etc
                    left join 
                    (select headerid, sum(AvailableBudget)Total_Avail_Budget from taskheader
                    GROUP BY HEADERID) ab on ab.HeaderId = etc.RecordID
                    left join (select distinct PROJECT_NUMBER, PROJECT_NAME, CUSTOMER_NAME
                    from [dw-gram-test].dbo.PDR_Project where category_type = 'SERVICES') a 
                    on (a.project_Number = ETC.MSO)
                    left join Periods p on p.PeriodID = etc.PeriodID
                    left join dw.[dbo].[ProjectTotalExtPriceReport] proj
                    on proj.PROJECT_NUMBER = etc.MSO
                   -- inner join (select distinct MSO, currency_code from dw.[dbo].dimSO_HDR )HDR on HDR.MSO = ETC.MSO
                    WHERE etc.PeriodName is not null AND etc.PeriodID <> 999";

            if (!string.IsNullOrEmpty(userChoice))
            {
                    sql = sql + " and etc.PeriodName = '" + userChoice + "'";
            }
            if (Admin == true || ReadAll == true)
            {
                sql = sql + " and etc.Region IN (" + regionlistSQL + ")";
            }

            else if (UserList != null)
            {
                sql = sql + " and etc.[ProjectManagerUsername] IN (" + userlistSQL + ")";
            }

            if (RecordId != null)
            {
                sql = sql + " and etc.[RecordId] = " + RecordId + "";
            }

            return _db.LoadData<ProjectHeaderModel, dynamic>(sql, new { });
        }


        public Task UpdateProjectHeader(ProjectHeaderModel projectHeader)
        {
            _db.ConnectionStringName = "ETC";
            string sql = string.Empty;
            string statuscheck = projectHeader.Status;
            sql = @"update ETCHeader set status = @status,DateApprovalRequested = @DateApprovalRequested, DateApproved=@DateApproved," +
                "ApproverUserName = @ApproverUserName where RecordID = @recordId";

            try
            {
                return _db.SaveData(sql, projectHeader);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<List<ProjectHeaderModel>> GetApprovalHeaderData(string? MSO, string? ADUsername, bool Admin, bool ReadAll, List<string?> RegionList)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            string regionlistSQL = "''";
            foreach (string region in RegionList)
            {
                regionlistSQL = regionlistSQL + ",'" + region + "'";
            }


            sql = @"SELECT 
                    etc.RecordID,
                    p.PeriodName,
                    p.PeriodMonthName,
                    p.Status AS PeriodStatus,
                    etc.ProjectManager,
                    etc.ProjectManagerUsername,
                    etc.DateGenerated,
                    etc.MSO,
                    etc.Status,
                    etc.DateApprovalRequested,
                    etc.DateApproved,
                    etc.ApproverUsername,
                    etc.PeriodID,
                    a.Project_Name,
                    a.CUSTOMER_NAME,
                    ApproversList.ApproversList,
                    ab.Total_Avail_Budget,
                    etc.TotalCost as TEC_SubmittedValue,
                    (CASE WHEN CONVERT(int, ab.Total_Avail_Budget) IS NULL THEN 0 ELSE CONVERT(int, ab.Total_Avail_Budget) 
                    END) - (CASE WHEN CONVERT(int, etc.TotalCost) IS NULL THEN 0 ELSE CONVERT(int, etc.TotalCost) END) as BudgetUpdate
                    -- Add the ApproversList column
                    FROM ETCHeader etc
                    LEFT JOIN (
		                    SELECT DISTINCT PROJECT_NUMBER, PROJECT_NAME, CUSTOMER_NAME
		                    FROM [dw-gram-test].dbo.PDR_Project
		                    ) a ON a.Project_Number = etc.MSO
                    LEFT JOIN dbo.Periods p ON p.PeriodID = etc.PeriodID
                    LEFT JOIN (
		                    SELECT recordId, STRING_AGG(approverUserName, ', ') AS ApproversList
		                    FROM (
			                    SELECT DISTINCT recordid, approverUserName
			                    FROM approvals
			                    WHERE approved = 0 
				                    AND rejected = 0
		                    ) AS DistinctApprovers
		                    GROUP BY recordId
		                    ) ApproversList ON ApproversList.recordId = etc.RecordID     
                    left join (select headerid, sum(AvailableBudget)Total_Avail_Budget from taskheader GROUP BY HEADERID) ab on ab.HeaderId = etc.REcordID
                    where etc.status IN ('Approval Pending','Pending - Approval Rejected', 'Approved')";

            if (Admin == true || ReadAll == true)
            {
                sql = sql + " and etc.Region IN (" + regionlistSQL + ")";
            }

            else if (ADUsername != null)
            {
                sql = sql + " and [ProjectManagerUsername] = '" + ADUsername + "'";
            }


            return _db.LoadData<ProjectHeaderModel, dynamic>(sql, new { });
        }

        public Task<List<ProjectHeaderModel>> GetTotalAvailableBudget(int RecordId)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select * from vTotalAvailableBudget where RecordId = " + RecordId + "";


            return _db.LoadData<ProjectHeaderModel, dynamic>(sql, new { });
        }


        public Task InsertActivityLog(string id, string message, string? Notes)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "insert into ActivityLog (ProjectId,MessageText,DateAdded, Notes) values ('" + id + "','" + message + "',getdate(),'" + Notes + "')";
            return _db.SaveData(sql, new { id, message, Notes });
        }

        public Task AddApprover(int RecordId, string ApproverUserName)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "insert into Approvals (RecordId,ApproverUserName, DateApprovalRequested,Approved,Rejected) values (" + RecordId + ",'" + @ApproverUserName + "',getdate(),0,0)";
            return _db.SaveData(sql, new { RecordId, ApproverUserName });
        }

        public Task UpdateApproval(int RecordId, string ApproverUserName)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "update Approvals set DateApproved=getdate(),Approved=1,Rejected=0 where RecordId = " + RecordId + " and ApproverUserName = '" + @ApproverUserName + "'";
            return _db.SaveData(sql, new { RecordId, ApproverUserName });
        }

        public Task DeleteExistingApprovers(int RecordId)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "update Approvals set Deleted=1  where RecordId = " + RecordId + " ";
            return _db.SaveData(sql, new { RecordId });
        }

        public Task<List<ApprovalModel>> GetApprovalList(int RecordId)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "select * from Approvals where RecordId = '" + RecordId + "' and deleted = 0";
            return _db.LoadData<ApprovalModel, dynamic>(sql, new { });
        }


        public Task<List<ActivityLogModel>> GetActivityLog(string? RecordId)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select * from ActivityLog where ProjectId = '" + RecordId + "' order by DateAdded desc";

            return _db.LoadData<ActivityLogModel, dynamic>(sql, new { });
        }


        public Task<List<TaskModel>> GetTaskList(int? RecordId)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            //sql = @"select * from vTaskList where RecordId = '" + RecordId + "'";

            sql = @"select * from vTaskList2 where RecordId = '" + RecordId + "'";


            return _db.LoadData<TaskModel, dynamic>(sql, new { });
        }



        public Task UpdateTaskDetailID(float? TaskPeriodValue, int? TaskDetailID)
        {
            _db.ConnectionStringName = "ETC";
            string sql = string.Empty;

            sql = "update taskDetail set PeriodValue = " + TaskPeriodValue + " where TaskDetailId = '" + TaskDetailID + "'";

            try
            {
                return _db.SaveData(sql, new { TaskPeriodValue, TaskDetailID });
            }
            catch (Exception)
            {
                throw;
            }

        }


        public Task SaveTaskList(string sql)
        {
            _db.ConnectionStringName = "ETC";

            return _db.SaveData(sql, new { });
        }


        public Task<List<PermissionModel>> GetPermissions(string ADUsername)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select
                    u.adusername,
                    p.permission
                    from usersv2 u 
                    left join permissionsv2 p on p.recordid = u.recordid
                    where u.Active = 1 and p.active = 1 and u.ADUsername = '" + ADUsername + "'";

            return _db.LoadData<PermissionModel, dynamic>(sql, new { ADUsername });
        }


        public Task<List<UserModel>> GetUserDetails(string ADUsername)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select * from Usersv2 where ADUsername = '" + ADUsername + "'";

            return _db.LoadData<UserModel, dynamic>(sql, new { ADUsername });
        }

        public Task<List<SubmittedModel>> GetSubmissions()
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"SELECT 
                    dbo.ETCHeader.MSO, 
                    p.PeriodName, 
                    dbo.ETCHeader.ProjectManager, dbo.ETCHeader.Status,
                    dbo.SubmittedTemp.SubmittedValue, 
                    dbo.SubmittedTemp.DateSubmitted
                    FROM          
                    dbo.ETCHeader
                    INNER JOIN
                    dbo.SubmittedTemp 
                    ON dbo.ETCHeader.RecordID = dbo.SubmittedTemp.RecordId
                    LEFT JOIN dbo.Periods p ON p.PeriodID = dbo.ETCHeader.PeriodID
";

            return _db.LoadData<SubmittedModel, dynamic>(sql, new { });
        }

        public Task AddSubmission(int RecordId, float? SubmittedValue, float? TECSubmittedValue)
        {
            //Delete any existing records for this id
            ClearSubmission(RecordId);

            _db.ConnectionStringName = "ETC";
            string sql = "insert into submittedTemp (RecordId,SubmittedValue, DateSubmitted, TEC_SubmittedValue) values (" + RecordId + ",'" + SubmittedValue + "',getdate(),'" + TECSubmittedValue + "')";
            return _db.SaveData(sql, new { RecordId, SubmittedValue, TECSubmittedValue });
        }

        public Task ClearSubmission(int RecordId)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "delete  from submittedTemp where  recordId = " + RecordId + "";
            return _db.SaveData(sql, new { RecordId });
        }

        public Task ClearAnswers(int RecordId)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "delete  from Answers where  ETCHeaderId = " + RecordId + "";
            return _db.SaveData(sql, new { RecordId });
        }

        public Task<List<PeriodsModel>> GetPeriods(string ADUsername)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"SELECT distinct p.* from  PERIODS p
                        left join ETCHeader etc
                        on etc.PeriodID = p.PeriodID
                        where etc.PeriodID IS NOT NULL AND etc.periodID <> 999";


            return _db.LoadData<PeriodsModel, dynamic>(sql, new { });
        }

        public Task<List<ManagerModel>> GetManagerStructure(string ADUsername)
        {
            string sql = "";
            _db.ConnectionStringName = "DAS";

            sql = @"SELECT        ADUsers.samaccountname AS employee, ADUsers_1.samaccountname AS manager " +
                    " FROM   DAS.dbo.ADUsers AS ADUsers_5 RIGHT OUTER JOIN " +
                    "      DAS.dbo.ADUsers AS ADUsers_4 ON ADUsers_5.distinguishedname = ADUsers_4.manager RIGHT OUTER JOIN  " +
                    "     DAS.dbo.ADUsers AS ADUsers_3 ON ADUsers_4.distinguishedname = ADUsers_3.manager RIGHT OUTER JOIN  " +
                    "    DAS.dbo.ADUsers AS ADUsers_2 ON ADUsers_3.distinguishedname = ADUsers_2.manager RIGHT OUTER JOIN  " +
                    "     DAS.dbo.ADUsers AS ADUsers INNER JOIN  " +
                    "    DAS.dbo.ADUsers AS ADUsers_1 ON ADUsers.manager = ADUsers_1.distinguishedname ON ADUsers_2.distinguishedname = ADUsers_1.manager  " +
                    "	 where ADUsers_1.samaccountname = '" + ADUsername + "' " +
                    "  or ADUsers_2.samaccountname  = '" + ADUsername + "' ";

            return _db.LoadData<ManagerModel, dynamic>(sql, new { });
        }




        public Task UpdatePeriodStatusOpen(PeriodsModel period)
        {
            _db.ConnectionStringName = "ETC";
            string sql = string.Empty;

            sql = @"update periods set Status = 'Open' where PeriodId = '" + period.PeriodID + "'";

            try
            {
                return _db.SaveData(sql, period);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task UpdatePeriodStatusClose(PeriodsModel period)
        {
            _db.ConnectionStringName = "ETC";
            string sql = string.Empty;

            sql = @"update periods set Status = 'Closed' where PeriodId = '" + period.PeriodID + "'";

            try
            {
                return _db.SaveData(sql, period);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<List<EngineersModel>> GetEngineerRates()
        {
            _db.ConnectionStringName = "ETC";
            string sql = @"SELECT EngineerID, EngineerType, EngineerRate, StartDate, EndDate, FY
                                FROM EngineerRates
                                WHERE EXISTS (
                                    SELECT 1
                                    FROM EngineerRates AS Subquery
                                    WHERE Subquery.EngineerID = EngineerRates.EngineerID
                                    GROUP BY Subquery.EngineerID
                                    HAVING MAX(Subquery.EndDate) = EngineerRates.EndDate)";
            return _db.LoadData<EngineersModel, dynamic>(sql, new { });
        }

        public Task InsertPAActivityLog(string period, string message)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "insert into ActivityLog (PAPeriod,MessageText,DateAdded) values ('" + period + "','" + message + "',getdate())";
            return _db.SaveData(sql, new { period, message });
        }

        public Task<List<ActivityLogModel>> GetPAActivityLog(string? PAPeriod)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select Top 10 * from ActivityLog where PAPeriod IS NOT NULL order by DateAdded desc";

            return _db.LoadData<ActivityLogModel, dynamic>(sql, new { });

        }


        public Task<List<QuestionModel>> GetQuestions()
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select * from Question where Active = 1";

            return _db.LoadData<QuestionModel, dynamic>(sql, new { });

        }

        


        public Task SaveAnswer(AnswerModel answers)
        {
            _db.ConnectionStringName = "ETC";

            if (answers.AnswerDate != null)
            {
                answers.Answer = answers.AnswerDate.ToString();
            }

            string sql = "insert into Answers (QuestionId,UserId, dateSubmitted,Answer,Deleted,ETCHeaderId) values ('" + answers.QuestionId + "','" + answers.UserId + "',getdate(),'" + answers.Answer + "',0,'" + answers.ETCHeaderId + "')";
            try
            {
                return _db.SaveData(sql, answers);
            }
            catch (Exception)
            {
                throw;
            }
        }
      
        public Task<List<QuestionAnswerDisplayModel>> GetAnswers(int HeaderId)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select * from answers
            inner join question on question.Id = Answers.QuestionId
            where deleted = 0 and  ETCHeaderId = '" + HeaderId + "' order by QuestionId desc";

            return _db.LoadData<QuestionAnswerDisplayModel, dynamic>(sql, new { });

        }

        public Task UpdatePCDate(int RecordId, DateTime? PlannedCompletionDate)
        {


            _db.ConnectionStringName = "ETC";
            string sql = @"update ETCHeader set PlannedCompletionDate = @PlannedCompletionDate where RecordID = @RecordId";

            try
            {
                return _db.SaveData(sql, new { RecordId, PlannedCompletionDate });
            }
            catch (Exception)
            {
                throw;
            }


        }

        public Task<List<UserRegionsModel>> GetUserRegions(string ADUsername)
        {
            string sql = "";
            _db.ConnectionStringName = "ETC";

            sql = @"select
                    distinct
                    u.adusername,
                    r.Region
                    from usersv2 u 
                    left join permissionsv2 p on p.recordid = u.recordid
                    left join userregionsv2 r on r.PermissionId = p.Permissionid
                    where u.Active = 1 and p.active = 1 and r.Active = 1 and u.ADUsername = '" + ADUsername + "' ";
            return _db.LoadData<UserRegionsModel, dynamic>(sql, new { });
        }

        public Task UpdateTotalCost(int RecordId, float TotalCost)
        {

            _db.ConnectionStringName = "ETC";
            string sql = string.Empty;

            sql = @"update ETCHeader set TotalCost = " + TotalCost + " where RecordID = " + @RecordId;

            try
            {
                return _db.SaveData(sql, new { RecordId, TotalCost });
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task UpdateRejected(int RecordId, string ApproverUserName)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "update Approvals set DateRejected=getdate(),Approved=0,Rejected=1 where RecordId = " + RecordId + " and ApproverUserName = '" + @ApproverUserName + "'";
            return _db.SaveData(sql, new { RecordId, ApproverUserName });
        }


        public Task InsertLog(string username, string componentName, string additional)
        {
            _db.ConnectionStringName = "ETC";
            string sql = "insert into AccessLog (Username,ComponentName, Additional, timeLogged) " +
                "values ('" + username + "','" + componentName + "','" + additional + "',getdate())";
            return _db.SaveData(sql, new { username, componentName,additional });
        }

        //public async Task<bool> AllQuestionsAnswered(int RecordId)
        //{
        //    string sql = "";
        //    _db.ConnectionStringName = "ETC";

        //    sql = @"
        //            select a.etcheaderid, 
        //            CASE WHEN count(questionid) = sum(answered_flag) then 1 else 0 end AllAnswered_Flag
        //            from
        //            (
        //            select 
        //            a.etcheaderid,
        //            a.questionid,
        //            case when a.answer is null or a.answer = '' or a.answer = ' ' then 0 else 1 end as Answered_Flag
        //            from Question q
        //            left join Answers a on a.questionid = q.id
        //            where q.Active = 1 and RecordId = " + RecordId + ") a GROUP BY a.ETCHeaderId";

        //    var result = await _db.LoadData<QuestionModel, dynamic>(sql, new { RecordId });
        //    return result.FirstOrDefault()?.AllAnswered_Flag ?? false;

        //}


    }
}
