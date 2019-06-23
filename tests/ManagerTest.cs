using System.Collections.Generic;
using NUnit.Framework;

using FlagsNet.Providers;
using System;
using Newtonsoft.Json;

namespace FlagsNet.Tests
{
    public abstract class ManagerTest
    {
        protected Manager manager;

        [OneTimeSetUp]
        public void Setup()
        {
            CustomFlag flag = new CustomFlag
            {
                Name = "flags-net",
                Parameter = "flags-net-parameter"
            };

            manager = CreateManager();
            manager.Add("feature:switch:enabled", true);
            manager.Add("feature:switch:disabled", false);
            manager.Add("feature:number", 1, true);
            manager.Add("feature:list", new List<string>{"1", "2", "3"}, true);
            manager.Add("feature:custom", flag, true);
        }

        protected abstract Manager CreateManager();

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
        public void Test_Manager_Custom_Should_Return_Inactive_If_Pattern_Does_Not_Match()
        {
            var feature = manager.Active<CustomFlag>("feature:custom", f => f.Name == "flags-net2");
            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Custom_Should_Return_Active_Using_KeyValuePair_Match()
        {
            var feature = manager.Active<IDictionary<string, string>>("feature:custom", p => p.ContainsKey("Name"));
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Should_Find_By_Json_Path()
        {
            var feature = manager.Active("feature:custom", "$[?(@.Name == 'flags-net' && @.Parameter == 'flags-net-parameter')]");
            Assert.IsTrue(feature);
        }

        [Test]
        public void Test_Manager_Custom_Should_Return_Inactive_If_Pattern_Has_Not_Being_Passed()
        {
            var feature = manager.Active("feature:custom");
            Assert.IsFalse(feature);
        }

        [Test]
        public void Test_Manager_Load_Data()
        {
            var content = "{\"feature:basic\": {\"Activated\": \"True\",\"Conditions\": null},\"feature:intermediate\": {\"Activated\": \"True\",\"Conditions\": [{\"param\": \"ab\"}, {\"param\":\"bc\"}]},\"feature:advanced\": {\"Activated\": \"True\",\"Conditions\": [{\"param\": \"ab\", \"data\": [\"1\", \"2\"]}, {\"param\":\"bc\"}]}}";
            manager.Load(content);

            Assert.IsTrue(manager.Active("feature:basic"));
            Assert.IsFalse(manager.Active("feature:intermediate", "$[?(@.param == 'oi')]"));
            Assert.IsTrue(manager.Active("feature:advanced", "$[?(@.param == 'ab')].data[?(@ == '1')]"));
        }
    }
}