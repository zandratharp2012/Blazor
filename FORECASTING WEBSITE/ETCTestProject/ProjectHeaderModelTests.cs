using ETC.TaskListService;
using ETCDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCTestProject
{
    public class ProjectHeaderModelTests
    {
        [Fact]
        public void ConversionRateOf1ShouldReturnMatchingValues()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 1,
                EAC_REVENUE_BUDGET = 1,
                EAC_COST_BUDGET = 1,
            };
          

            // Act
            decimal CostResult = projectHeader.EAC_COST_BUDGET_ConvCurr ;
            decimal RevenueResult = projectHeader.EAC_REVENUE_BUDGET_ConvCurr;

            // Assert
            Assert.Equal(CostResult, projectHeader.EAC_COST_BUDGET);
            Assert.Equal(RevenueResult, projectHeader.EAC_REVENUE_BUDGET);
        }


        [Fact]
        public void ConversionRateOfDecimalShouldReturnNewValues()
        {
            // Arrange
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 627.352m,
                EAC_REVENUE_BUDGET = 29134.8m, //29134.8/627.352=18277775.05
                EAC_COST_BUDGET = 16994.95m, //16994.95/627.325=10661815.87

            };

            // Act
            decimal CostResult = projectHeader.EAC_COST_BUDGET_ConvCurr;
            decimal RevenueResult = projectHeader.EAC_REVENUE_BUDGET_ConvCurr;

            // Assert
            Assert.Equal(RevenueResult, 18277775.05m);
            Assert.Equal(CostResult, 10661815.87m);
            
        }

        [Fact]
        public void MarginPercent_ShouldReturnOne_WhenEACRevenueBudgetIsZero()
        {
            // Arrange
            var projectHeader = new ProjectHeaderModel
            {
                EAC_REVENUE_BUDGET = 0,
                EAC_COST_BUDGET = 1000
            };

            // Act
            decimal result = projectHeader.MarginPercent;

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void MarginPercent_ShouldReturnCorrectValue_WhenEACRevenueBudgetIsNonZero()
        {
            // Arrange
            var projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 1,
                EAC_REVENUE_BUDGET = 2000,
                EAC_COST_BUDGET = 1000
            };

            // Act
            decimal result = projectHeader.MarginPercent;

            // Assert
            decimal expected = Math.Round((1000m / 2000m) * 100, 2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MarginPercent_ShouldReturnZero_WhenMarginValueIsZero()
        {
            // Arrange
            var projectHeader = new ProjectHeaderModel
            {
                EAC_REVENUE_BUDGET = 2000,
                EAC_COST_BUDGET = 2000
            };

            // Act
            decimal result = projectHeader.MarginPercent;

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void MarginValue_ShouldReturn()
        {
            // Arrange
         
            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 1,
                EAC_REVENUE_BUDGET = 1738.68m,
                EAC_COST_BUDGET = 1431.43m
            };


            // Act
            decimal result = projectHeader.marginValue;

            // Assert
            Assert.Equal(307.25m, result);
        }

        [Fact]
        public void MarginPercent_ShouldReturn()
        {
            // Arrange

            ProjectHeaderModel projectHeader = new ProjectHeaderModel
            {
                CONVERSION_RATE = 1500.2304m,
                EAC_REVENUE_BUDGET = 1738.68m,
                EAC_COST_BUDGET = 1431.43m
            };


            // Act
            decimal result = projectHeader.MarginPercent;

            // Assert
            Assert.Equal(17.67m, result);
        }
    }
}
