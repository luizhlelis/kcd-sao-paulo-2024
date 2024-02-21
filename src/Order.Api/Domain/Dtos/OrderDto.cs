namespace Order.Api.Domain.Dtos;

public class OrderDto
{
    public List<OrderItemDto> Items { get; set; } = new ();
}

public class OrderItemDto
{
    public int ProductCode { get; set; }

    public double Price { get; set; }

    public int Amount { get; set; }
}