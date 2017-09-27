using System;
using Automatonymous;

namespace Order.Saga
{
    public class OrderState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid OrderId { get; set; }
    }
}