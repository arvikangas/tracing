using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlApp.Database;

namespace SqlApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        readonly SqlDbContext _db;

        public OrdersController(SqlDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _db.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
            return Ok(order);
        }

        [HttpGet("customer-id/{id}")]
        public async Task<ActionResult> GetCustomerId(int id)
        {
            var order = await _db.Orders
                .Select(x => new { x.OrderId, x.CustomerId })
                .Where(x => x.OrderId == id)
                .FirstOrDefaultAsync();
            return Ok(order);
        }

        [HttpGet("employees")]
        public async Task<ActionResult> GetEmployees()
        {
            var employee = await _db.Employees
                .Include(x => x.Orders)
                .Include(x => x.Orders)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.Orders
                })
                .ToListAsync();
            return Ok(employee);
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult> GetEmployee(int id)
        {
            var employee = await _db.Employees
                .Include(x => x.Orders)
                .Select(x => new 
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.Orders
                })
                .Where(x => x.EmployeeId == id)
                .FirstOrDefaultAsync();
            return Ok(employee);
        }
    }
}
