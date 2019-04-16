using System;
using NUnit.Framework;

namespace FlagsNet.Tests
{
    public class CircuitBreakerTest
    {
        [Test]
        public void Test_Should_Return_Closed_If_Has_No_Error()
        {
            var circuit = new CircuitBreaker();
            Assert.AreEqual(CircuitStatus.Closed, circuit.Status);
        }

        [Test]
        public void Test_Should_Return_Open_If_Has_One_Error()
        {
            var circuit = new CircuitBreaker();
            circuit.SetFail();
            Assert.AreEqual(CircuitStatus.HalfOpen, circuit.Status);
        }

        [Test]
        public void Test_Should_Return_Open_If_Has_Two_Errors()
        {
            var circuit = new CircuitBreaker();
            circuit.SetFail();
            circuit.SetFail();
            Assert.AreEqual(CircuitStatus.Open, circuit.Status);
        }

        [Test]
        public void Test_Should_Return_Half_Open_After_One_Minute()
        {
            var circuit = new CircuitBreaker();
            circuit.SetFail();
            circuit.SetFail(DateTime.UtcNow.AddMinutes(-1));
            Assert.AreEqual(CircuitStatus.HalfOpen, circuit.Status);
        }

        [Test]
        public void Test_Should_Return_Closed_After_One_Minute()
        {
            var circuit = new CircuitBreaker();
            circuit.SetFail(DateTime.UtcNow.AddMinutes(-1));
            Assert.AreEqual(CircuitStatus.Closed, circuit.Status);
        }
    }
}