using System;

namespace FlagsNet
{
    public class CircuitBreaker
    {
        private CircuitStatus status;
        private DateTime lastFail;

        public CircuitBreaker()
        {
            status = CircuitStatus.Closed;
        }

        public void SetFail()
        {
            SetFail(DateTime.UtcNow);
        }

        public void SetFail(DateTime when)
        {
            lastFail = when;
            if (status == CircuitStatus.Closed)
            {
                status = CircuitStatus.HalfOpen;
            }
            else if (status == CircuitStatus.HalfOpen)
            {
                status = CircuitStatus.Open;
            }
        }

        public CircuitStatus Status
        {
            get
            {
                if (status != CircuitStatus.Closed)
                {
                    var delta = DateTime.UtcNow - lastFail;
                    if (delta.TotalMinutes > 1)
                    {
                        status = status == CircuitStatus.HalfOpen ? CircuitStatus.Closed : CircuitStatus.HalfOpen;
                    }
                }
                return status;
            }
        }
    }
}