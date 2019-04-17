using System;

namespace FlagsNet
{
    public class CircuitBreaker
    {
        private readonly TimeSpan timeSpan;

        private CircuitStatus status;
        private DateTime lastFail;

        public CircuitBreaker(int phaseSeconds = 60)
        {
            status = CircuitStatus.Closed;
            this.timeSpan = TimeSpan.FromSeconds(phaseSeconds);
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
                    if (delta.TotalSeconds > timeSpan.TotalSeconds)
                    {
                        status = status == CircuitStatus.HalfOpen ? CircuitStatus.Closed : CircuitStatus.HalfOpen;
                    }
                }
                return status;
            }
        }
    }
}