using ETC.TaskListService;
using ETCDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ETCTestProject
{
    public class ProjectHelperTests
    {
        [Fact]
        public void IsAmericanProject_ShouldReturnTrue_WhenMSOStartsWithNA()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel();
            projectHeader.MSO =  "NA123" ;
            var helper = new TaskListService();

            // Act
            bool result = helper.IsAmericanProject(projectHeader);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAmericanProject_ShouldReturnTrue_WhenMSOStartsWithLA()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel();
            projectHeader.MSO = "LA123";
            var helper = new TaskListService();

            // Act
            bool result = helper.IsAmericanProject(projectHeader);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAmericanProject_ShouldReturnFalse_WhenMSOStartsWithAF()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel();
            projectHeader.MSO =  "AF123";
            var helper = new TaskListService();

            // Act
            bool result = helper.IsAmericanProject(projectHeader);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAmericanProject_ShouldReturnFalse_WhenMSOIsEmpty()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel();
            projectHeader.MSO =  "";
            var helper = new TaskListService();

            // Act
            bool result = helper.IsAmericanProject(projectHeader);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsBudgetThresholdExceeded_ShouldReturnTrue_WhenOver25000()
        {
            // Arrange
            float BudgetThreshold = 25001;
            var helper = new TaskListService();

            // Act
            bool result = helper.IsBudgetThresholdExceeded(BudgetThreshold);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsBudgetThresholdExceeded_ShouldReturnFalse_WhenExactly25000()
        {
            // Arrange
            float BudgetThreshold = 25000;
            var helper = new TaskListService();

            // Act
            bool result = helper.IsBudgetThresholdExceeded(BudgetThreshold);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsBudgetThresholdExceeded_ShouldReturnFalse_WhenUnder25000()
        {
            // Arrange
            float BudgetThreshold = 15478;
            var helper = new TaskListService();

            // Act
            bool result = helper.IsBudgetThresholdExceeded(BudgetThreshold);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CalculateMargin_ShouldReturn25pt44()
        {
            // Arrange
            decimal BudgetThreshold = -8582.2m;
           // decimal TotalValue = 0;
            var taskListService = new TaskListService();
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 1,
                EAC_REVENUE_BUDGET = 340058m,
                EAC_COST_BUDGET = 244976m
            };


            // Act
            decimal result = taskListService.CalculateMargin(projectHeader, BudgetThreshold);

            // Assert
            Assert.Equal(25.44m, result);
        }


        //[Fact]
        //public void CalculateMargin_ShouldReturn17pt6()
        //{
        //    // Arrange
        //    float? BudgetThreshold = 0;
        //    var taskListService = new TaskListService();
        //    ProjectHeaderModel projectHeader = new ProjectHeaderModel
        //    {
        //        CONVERSION_RATE = 1,
        //        EAC_REVENUE_BUDGET = 1738.68m,
        //       EAC_COST_BUDGET = 1431.43m
        //    };


        //    // Act
        //    decimal result = taskListService.CalculateMargin(projectHeader, BudgetThreshold);

        //    // Assert
        //    Assert.Equal(17.67m, result);
        //}

        //[Fact]
        //public void CalculateMargin_ShouldReturn90()
        //{
        //    // Arrange
        //    float? BudgetThreshold = 10;
        //    var taskListService = new TaskListService();
        //    ProjectHeaderModel projectHeader = new ProjectHeaderModel
        //    {
        //        CONVERSION_RATE = 1,
        //        EAC_REVENUE_BUDGET = 100,
        //        EAC_COST_BUDGET = 0,
        //    };


        //    // Act
        //    decimal result = taskListService.CalculateMargin(projectHeader, BudgetThreshold);

        //    // Assert
        //    Assert.Equal(90, result);
        //}


        //[Fact]
        //public void CalculateMargin_ConversionRate1point5ShouldReturn100()
        //{
        //    // Arrange
        //    float? BudgetThreshold = 0;
        //    var taskListService = new TaskListService();
        //    ProjectHeaderModel projectHeader = new ProjectHeaderModel
        //    {
        //        CONVERSION_RATE = 1.5m,
        //        EAC_REVENUE_BUDGET = 100,
        //        EAC_COST_BUDGET = 0,
        //    };


        //    // Act
        //    decimal result = taskListService.CalculateMargin(projectHeader, BudgetThreshold);

        //    // Assert
        //    Assert.Equal(100, result);
        //}

        [Fact]
        public void IsMarginChangeExceeded_ShouldReturnTruewhenGreaterThan5()
        {
            //Arrange
            var taskListService = new TaskListService();
            decimal calculatedMargin = 35; //from ETC
            decimal marginPercent = 41; //from Oracle
            //Act
            bool result = taskListService.IsMarginChangeExceeded(calculatedMargin, marginPercent);
            //Assert
            Assert.True(result);
        }


        [Fact]
        public void IsMarginChangeExceeded_ShouldReturnFalsewhenLessThan5()
        {
            //Arrange
            var taskListService = new TaskListService();
            decimal calculatedMargin = 40; //from ETC
            decimal marginPercent = 41; //from Oracle
            //Act
            bool result = taskListService.IsMarginChangeExceeded(calculatedMargin, marginPercent);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMarginChangeExceeded_ShouldReturnFalsewhenETCValueIsGreater()
        {
            //Arrange
            var taskListService = new TaskListService();
            decimal calculatedMargin = 50; //from ETC
            decimal marginPercent = 41; //from Oracle
            //Act
            bool result = taskListService.IsMarginChangeExceeded(calculatedMargin, marginPercent);
            //Assert
            Assert.False(result);
        }


        [Fact]
        public void IsMarginChangeExceeded_ShouldReturnTrueWhenETCHasNegativeMargin()
        {
            //Arrange
            var taskListService = new TaskListService();
            decimal calculatedMargin = -50; //from ETC
            decimal marginPercent = 41; //from Oracle
            //Act
            bool result = taskListService.IsMarginChangeExceeded(calculatedMargin, marginPercent);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void IsRecordLocked_ShouldReturnTrueWhenSubmitted()
        {
            //Arrange
            var taskListService = new TaskListService();
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                Status = "Submitted"
              
            };

            //Act
            bool result = taskListService.IsRecordLocked(projectHeader);
        //Assert
        Assert.True(result);
        }

        [Fact]
        public void IsRecordLocked_ShouldReturnFalseWhenPending()
        {
            //Arrange
            var taskListService = new TaskListService();
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                Status = "Pending"

            };

            //Act
            bool result = taskListService.IsRecordLocked(projectHeader);
            //Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("Submitted", true)]
        [InlineData("Approved", true)]
        [InlineData("Approval Pending", true)]
        [InlineData("Rejected", false)]
        [InlineData("Pending", false)]
        [InlineData("Pending - Approval Rejected", false)]
        public void IsRecordLocked_ShouldReturnExpectedResult(string status, bool expectedResult)
        {
            // Arrange
            var projectHeader = new ProjectHeaderModel { Status = status };
            var taskListService = new TaskListService();

            // Act
            bool result = taskListService.IsRecordLocked(projectHeader);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
