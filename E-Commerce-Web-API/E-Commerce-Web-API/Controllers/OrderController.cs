using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using E_Commerce_Web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersAsync()
        {
            var ordersDTOs = await _orderService.GetOrdersAsync();
            if (ordersDTOs is null)
            {
                return NotFound("Orders not found");
            }

            return Ok(ordersDTOs);
        }

        [HttpGet("{id}", Name = nameof(GetOrderByIdAsync))]
        [ProducesResponseType<OrderDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order ID");
            }
            var orderDTO = await _orderService.GetOrderByIdAsync(id);
            if (orderDTO is null)
            {
                return NotFound("Order not found");
            }

            return Ok(orderDTO);
        }
        [HttpPost]
        [ProducesResponseType<Order>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrderAsync(CreateOrderDTO orderdto)
        {
            if (orderdto is null)
            {
                return BadRequest("Invalid order data");
            }
            var order = await _orderService.CreateOrderAsync(orderdto);

            return CreatedAtRoute(nameof(GetOrderByIdAsync), new { id = order.ID }, order);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOrderAsync(int id)
        {
            var order = await _orderService.GetOrderEntityByIdAsync(id);
            if (order is null)
            {
                return NotFound("Order not found");
            }

            await _orderService.DeleteOrderAsync(order);
            return NoContent();
        }
        [HttpPut("{id}/address")]
        [ProducesResponseType<OrderDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateOrderAddressDTO dto)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid order ID provided." });
            }

            try
            {
                OrderDTO? updatedOrder = await _orderService.UpdateOrderAddressAsync(id, dto);

                if (updatedOrder == null)
                {
                    return NotFound(new { Message = $"Order with ID {id} was not found." });
                }

                return Ok(new { Message = "Shipping address updated successfully!", Data = updatedOrder });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType<OrderDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDTO dto)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid order ID provided." });
            }

            try
            {
                OrderDTO? updatedOrder = await _orderService.UpdateOrderStatusAsync(id, dto);

                if (updatedOrder == null)
                {
                    return NotFound(new { Message = $"Order with ID {id} was not found." });
                }

                return Ok(new { Message = "Order status updated successfully!", Data = updatedOrder });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An internal server error occurred.", Details = ex.Message });
            }
        }
    }
}
