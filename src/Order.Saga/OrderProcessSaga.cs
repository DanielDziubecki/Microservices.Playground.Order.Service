using Automatonymous;
using MassTransit;
using Shared.Payment.Contracts;
using Shared.Order.Contracts;
namespace Order.Saga
{
    //todo: should be hosted somewhere else. For sure not in service
    public class OrderProcessSaga : MassTransitStateMachine<OrderState>
    {
        private string OperationId { get; set; }

        public OrderProcessSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentBegins, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentFails, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentSucceded, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderSubmited, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(OrderCreated)
                    .Then(context =>
                    {
                        context.Instance.OrderId = context.Data.OrderId;
                        if (context.TryGetPayload(out ConsumeContext<IOrderCreated> consumeContext))
                        {
                            var opId = consumeContext.Headers.Get("operationId", "");
                            if (!string.IsNullOrEmpty(opId))
                                OperationId = opId;
                        }
                    })
                    .TransitionTo(OrderRegistered));

            During(OrderRegistered,
                When(PaymentBegins)
                    .Then(context=> { context.Instance.PaymentId = context.Data.PaymentId; })
                    .TransitionTo(PaymentProcessBegin),
                When(PaymentFails)
                    .TransitionTo(PaymentProcessFails)
                    .Publish(context => new DeleteOrder{OrderId = context.Instance.OrderId})
                    .Finalize(),
                When(PaymentSucceded)
                    .TransitionTo(PaymentProcessSucceded)
                    .Publish(context => new OrderSubmitted{OrderId = context.Instance.OrderId})
                    .Finalize());
        }


        public State OrderRegistered { get; private set; }
        public State PaymentProcessBegin { get; private set; }
        public State PaymentProcessFails { get; private set; }
        public State PaymentProcessSucceded { get; private set; }
        public State OrderSubmitted { get; private set; }

      //  public Schedule<OrderState, OrderExpired> OrderExpired { get; private set; }


        public Event<IOrderCreated> OrderCreated { get; private set; }
        public Event<IPaymentProcessBegin> PaymentBegins { get; private set; }
        public Event<IPaymentProcessFail> PaymentFails { get; private set; }
        public Event<IPaymentProcessSucceded> PaymentSucceded { get; private set; }
        public Event<IOrderSubmitted> OrderSubmited { get; private set; }
    }
    //public class OrderExpired
    //{
    //    private readonly OrderState orderState;
    //    public Guid OrderId => orderState.CorrelationId;

    //    public OrderExpired(OrderState orderState)
    //    {
    //        this.orderState = orderState;
    //    }
    //}
}