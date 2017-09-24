using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Saga
{
    public class OrderSagaMap : IEntityTypeConfiguration<OrderState>
    {
        public void Configure(EntityTypeBuilder<OrderState> builder)
        {
            builder.ToTable("OrderStates");
            builder.Property(x => x.CurrentState)
                .HasMaxLength(64);
            builder.Property(x => x.OrderId);
            builder.Property(x => x.PaymentId);
        }
    }
}