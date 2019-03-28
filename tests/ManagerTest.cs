using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;
using System;

namespace FlagsNet.Tests
{
    public class ManagerTest
    {
        Manager manager;
        CustomFlag flag;

        [OneTimeSetUp]
        public void Setup()
        {
            flag = new CustomFlag
            {
                Name = "flags-net",
                Parameter = "flags-net-parameter"
            };
            manager = new Manager(new MemoryFlagSource());

            manager.Add("feature:switch:enabled", FlagStatus.Activated);
            manager.Add("feature:switch:disabled", FlagStatus.Deactivated);
            manager.Add("feature:number", 1, FlagStatus.Activated);
            manager.Add("feature:list", new List<string>{"1", "2", "3"}, FlagStatus.Activated);
            manager.Add("feature:custom", flag, FlagStatus.Activated);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_If_Switch_Does_Not_Exist()
        {
            var feature = manager.Active("feature:switch:invalid");

            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Active_Switch()
        {
            var feature = manager.Active("feature:switch:enabled");

            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_Switch()
        {
            var feature = manager.Active("feature:switch:disabled");

            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Active_When_Number_Matches()
        {
            var feature = manager.Active<int>("feature:number", el => el == 1);

            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_When_Number_Does_Not_Match()
        {
            var feature = manager.Active<int>("feature:number", el => el == 2);

            Assert.IsFalse(feature);
        }

        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        public void Test_Manager_Should_Return_Active_When_Number_Belongs_To_List(string number)
        {
            var feature = manager.Active<string>("feature:list", el => el == number);
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Active_When_At_Least_One_Number_Belongs_To_List()
        {
            var feature = manager.Active<string>("feature:list", el => el == "1" || el == "5");
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Should_Return_Inactive_When_Number_Does_Not_Belong_To_List()
        {
            var feature = manager.Active<string>("feature:list", el => el == "5");
            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Custom_Should_Return_Active_If_Pattern_Matches()
        {
            var feature = manager.Active<CustomFlag>("feature:custom", f => f.Name == "flags-net" && f.Parameter == "flags-net-parameter");
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Custom_Should_Return_Active_If_Pattern_Does_Not_Match()
        {
            var feature = manager.Active<CustomFlag>("feature:custom", f => f.Name == "flags-net2");
            Assert.IsFalse(feature);
        }
    }
}