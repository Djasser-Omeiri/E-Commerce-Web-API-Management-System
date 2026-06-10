using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.Enums;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce_Web_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDTO orderDTO)
        {
            var order = new Order
            {
                ShippingAddress = orderDTO.ShippingAddress,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in orderDTO.Items)
            {
                var product = await _unitOfWork.Products.GetProductEntityByIdAsync(item.ProductID);
                if (product == null || product.Stock == null)
                    throw new Exception($"Product {item.ProductID} not found");

                if (product.Stock.Quantity < item.Quantity)
                    throw new Exception($"Insufficient stock for product {item.ProductID}");

                product.Stock.Quantity -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    PriceAtPurchase = product.Price
                });
            }

            var created = await _unitOfWork.Orders.CreateOrderAsync(order);
            await _unitOfWork.CompleteAsync();
            return created;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            await _unitOfWork.Orders.DeleteOrderAsync(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order == null) return null;

            return new OrderDTO
            {
                ID = order.ID,
                OrderTime = order.OrderTime,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ID = oi.ID,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    ProductName = oi.Product?.Name ?? string.Empty
                }).ToList()
            };
        }

        public async Task<Order?> GetOrderEntityByIdAsync(int id)
        {
            return await _unitOfWork.Orders.GetOrderEntityByIdAsync(id);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetOrdersAsync();
            return orders.Select(o => new OrderDTO
            {
                ID = o.ID,
                OrderTime = o.OrderTime,
                TotalPrice = o.TotalPrice,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status.ToString(),
                OrderItems = o.OrderItems.Select(oi => new E_Commerce_Web_API.DTOs.OrderItem.OrderItemDTO
                {
                    ID = oi.ID,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    ProductName = oi.Product?.Name ?? string.Empty
                }).ToList()
            });
        }
        public async Task<OrderDTO?> UpdateOrderStatusAsync(int id, CreateOrderDTO dto)
        {
            Order? order = await _unitOfWork.Orders.GetOrderEntityByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            // 3. THE BUSINESS LOGIC: Parse the string from the DTO into your actual Enum type.
            // 'true' makes the parsing ignore case (e.g., "shipped" or "Shipped" will both work).
            if (Enum.TryParse(typeof(eOrderStatus), dto.NewStatus, true, out var parsedStatus))
            {
                order.Status = (eOrderStatus)parsedStatus;
            }
            else
            {
                // If the frontend sends something crazy like "InTransit", throw an exception
                throw new ArgumentException($"'{dto.NewStatus}' is not a valid order status.");
            }

            // 4. Save the changes to SQL Server.
            // Because Entity Framework is actively tracking the 'order' object we fetched above,
            // it automatically knows the status changed and will execute the SQL UPDATE statement.
            await _unitOfWork.CompleteAsync();

            // 5. Map the updated database entity back into your clean OrderDTO
            return new OrderDTO
            {
                ID = order.ID,
                OrderTime = order.OrderTime,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(), // Convert enum back to string for the frontend
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product?.Name ?? "Unknown Product",
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}
