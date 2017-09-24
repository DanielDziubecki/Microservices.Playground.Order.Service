using System;

namespace Shared.Payment.Contracts
{
    public interface IPaymentProcessFail
    {
        Guid? PaymentId { get; set; }
        Guid OrderId { get; set; }
    }
}