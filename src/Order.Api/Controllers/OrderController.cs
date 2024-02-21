using System.Diagnostics;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Domain.Events;
using Order.Api.Infrastructure;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ICapPublisher _capBus;
    private readonly OrderContext _context;

    public OrderController(
        ICapPublisher capPublisher,
        OrderContext context)
    {
        _context = context;
        _capBus = capPublisher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Domain.Dtos.OrderDto orderDto)
    {
        var order = Domain.Model.Order.FromDto(orderDto);
        
        _context.Orders.Add(order);

        await using (_context.Database.BeginTransaction(_capBus, true))
        {
            await _capBus.PublishAsync("order.created",
                new OrderCreated {Id = order.Id, Items = order.Items});

            await _context.SaveChangesAsync();
        }
        
        // workaround to add trace-context to the response headers 
        HttpContext.Response.Headers.Append("trace-context", Activity.Current?.Id);
        
        return Created("/order", order);
    }
}
