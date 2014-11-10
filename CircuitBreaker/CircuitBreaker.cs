using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private event EventHandler CircuitStateChanged;
        private Timer timer;
        private uint treshold;
        private uint timeout;
        private CircuitState circuitState;
        private uint failureCount;
        private object returnObject;

        #region Ctors
        public CircuitBreaker()
            : this(3000, 5)
        {

        }

        public CircuitBreaker(uint timeout, uint treshold)
        {
            this.timeout = timeout;
            this.treshold = treshold;
            this.circuitState = CircuitState.Closed;
            this.failureCount = 0;

            this.timer = new Timer(timeout);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }
        #endregion

        /// <summary>
        /// Entry point. method will take the delegate with parameters to invoke.
        /// </summary>
        /// <param name="delOperation"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Run(Delegate delOperation, object[] parameters)
        {
            if (circuitState == CircuitState.Open) throw new OpenCircuitException("Process is currently open", null);
            try
            {
                Trip();
                returnObject = delOperation.DynamicInvoke(parameters);
            }
            catch (Exception ex)
            {
                ResultHandle();                
            }

            if (this.circuitState == CircuitState.HalfOpen)
            {
                // is in a half-open state, then reset
                Reset();
            }

            return returnObject;
        }

        private void ResultHandle()
        {
            if (this.circuitState == CircuitState.HalfOpen)
            {
                Trip();
            }
            else if (failureCount < treshold)
            {
                failureCount++;
            }
            else if (failureCount >= treshold)
            {
                Trip();
            }

            throw new OperationFailedException("Operation failed", null);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChangeState(CircuitState.HalfOpen);
            this.timer.Stop();
            if (returnObject == null)
            {
                failureCount++;
                if (failureCount > treshold)
                { 
                    ChangeState(CircuitState.Closed);
                    throw new OperationFailedException("Operation failed", null);
                }
                Trip();
            }
        }

        private void ChangeState(CircuitState newState)
        {
            this.circuitState = newState;
        }

        private void Trip()
        {
            ChangeState(CircuitState.Open);
            this.timer.Start();
        }

        private void Reset()
        {
            ChangeState(CircuitState.Closed);
            this.timer.Stop();
        }
    }
}
