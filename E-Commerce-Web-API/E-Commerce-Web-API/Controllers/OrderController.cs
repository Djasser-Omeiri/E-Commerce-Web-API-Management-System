using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using E_Commerce_Web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType<IEnumerable<OrderDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            string? filterUserId = isAdmin ? null : userId;
            var ordersDTOs = await _orderService.GetOrdersFilterAsync(filterUserId);
            if (ordersDTOs is null)
            {
                _logger.LogWarning("GetOrdersAsync failed - no orders found.");
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid order ID requested: {OrderId} by user: {UserID}", id, userId);
                return BadRequest("Invalid order ID");
            }
            var orderDTO = await _orderService.GetOrderByIdAsync(id);
            if (orderDTO is null)
            {
                _logger.LogWarning("Order not found. ID: {OrderId} by user: {UserId})", id, userId);
                return NotFound("Order not found");
            }
            if (!User.CanAccess(orderDTO.User.ID))
            {
                _logger.LogWarning("Access denied to order ID: {OrderId} by user: {UserId})", id, userId);
                return Forbid();
            }

            return Ok(orderDTO);
        }
        [HttpPost]
        [ProducesResponseType<Order>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrderAsync(CreateOrderDTO orderdto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = User.Identity?.Name ?? "anonymous";
            if (User.IsInRole("Admin"))
            {
                _logger.LogWarning("CreateOrderAsync called by admin: {AdminId} at {Time}", userId, DateTime.UtcNow);
            }

            if (orderdto is null)
            {
                _logger.LogWarning("CreateOrderAsync failed - invalid order data. User: {UserId} at {Time}", userId, DateTime.UtcNow);
                return BadRequest("Invalid order data");
            }
            var order = await _orderService.CreateOrderAsync(orderdto, userId!);

            return CreatedAtRoute(nameof(GetOrderByIdAsync), new { id = order.ID }, order);
        }

        [HttpPut("{id}/address")]
        [ProducesResponseType<OrderDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateOrderAddressDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Admin"))
            {

                _logger.LogInformation("UpdateAddress called for order ID: {OrderId} by admin: {UserId} at {Time} ", id, userId, DateTime.UtcNow);
            }
            if (id <= 0)
            {
                _logger.LogWarning("UpdateAddress failed - invalid order ID: {OrderId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return BadRequest(new { Message = "Invalid order ID provided." });
            }
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null)
            {
                _logger.LogWarning("UpdateAddress failed - order not found. ID: {OrderId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return NotFound(new { Message = $"Order with ID {id} was not found." });
            }
            if (!User.CanAccess(order.User.ID))
            {
                _logger.LogWarning("Access denied for UpdateAddress. OrderId: {OrderId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return Forbid();
            }
            try
            {
                OrderDTO? updatedOrder = await _orderService.UpdateOrderAddressAsync(id, dto);

                if (updatedOrder == null)
                {
                    _logger.LogWarning("UpdateAddress failed - order not found after update. ID: {OrderId} at {Time}", id, DateTime.UtcNow);
                    return NotFound(new { Message = $"Order with ID {id} was not found." });
                }

                return Ok(new { Message = "Shipping address updated successfully!", Data = updatedOrder });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("UpdateAddress invalid operation for order ID: {OrderId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateAddress unexpected error for order ID: {OrderId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
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
            var adminid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("UpdateStatus called by admin: {Admin} for order ID: {OrderId}, NewStatus: {Status} at {Time}", adminid, id, dto.NewStatus, DateTime.UtcNow);

            if (id <= 0)
            {
                _logger.LogWarning("UpdateStatus failed - invalid order ID: {OrderId}, Admin: {Admin} at {Time}", id, adminid, DateTime.UtcNow);
                return BadRequest(new { Message = "Invalid order ID provided." });
            }

            try
            {
                OrderDTO? updatedOrder = await _orderService.UpdateOrderStatusAsync(id, dto);

                if (updatedOrder == null)
                {
                    _logger.LogWarning("UpdateStatus failed - order not found. ID: {OrderId}, Admin: {Admin} at {Time}", id, adminid, DateTime.UtcNow);
                    return NotFound(new { Message = $"Order with ID {id} was not found." });
                }

                return Ok(new { Message = "Order status updated successfully!", Data = updatedOrder });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("UpdateStatus argument error for order ID: {OrderId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("UpdateStatus invalid operation for order ID: {OrderId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateStatus unexpected error for order ID: {OrderId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
                return StatusCode(500, new { Error = "An internal server error occurred.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteOrderAsync(int id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Admin"))
            {
                _logger.LogInformation("DeleteOrderAsync called by admin: {AdminId} for order ID: {OrderId} at {Time}", adminId, id, DateTime.UtcNow);
            }

            var order = await _orderService.GetOrderEntityByIdAsync(id);
            if (order is null)
            {
                _logger.LogWarning("DeleteOrderAsync failed - order not found. ID: {OrderId}, Admin: {AdminId} at {Time}", id, adminId, DateTime.UtcNow);
                return NotFound("Order not found");
            }
            if (!User.CanAccess(order.UserId))
            {
                _logger.LogWarning("Access denied for DeleteOrderAsync. OrderId: {OrderId}, Admin: {AdminId} at {Time}", id, adminId, DateTime.UtcNow);
                return Forbid();
            }
            // Add the delete logic here based on enum
            await _orderService.DeleteOrderAsync(order);
            return NoContent();
        }
    }
}
