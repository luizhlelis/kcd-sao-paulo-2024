using Order.Api.Domain.Dtos;

namespace Order.Api.Domain.Model;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreationDate { get; set; } = DateTime.Now;

    public List<OrderItem> Items { get; set; } = new ();

    public double TotalPrice { get; set; }
    
    public static Order FromDto(OrderDto orderDto)
    {
        var order = new Order();
        foreach (var item in orderDto.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductCode = item.ProductCode,
                Price = item.Price,
                Amount = item.Amount
            });
         
            // Product price must be stored in the database instead of accepting value from requests
            order.TotalPrice =+ (item.Price * item.Amount);
        }
        
        return order;
    }
}
