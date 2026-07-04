using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.DTOs.User;
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

        public async Task<Order> CreateOrderAsync(CreateOrderDTO orderDTO, string userId)
        {
            var order = new Order
            {
                ShippingAddress = orderDTO.ShippingAddress,
                OrderItems = new List<OrderItem>(),
                UserId = userId
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
                }).ToList(),
                User = new UserDTO
                {
                    ID = order.UserId,
                    Username = order.User.UserName!
                }
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
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    ID = oi.ID,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    ProductName = oi.Product?.Name ?? string.Empty
                }).ToList()
            });
        }
        public async Task<OrderDTO?> UpdateOrderAddressAsync(int id, UpdateOrderAddressDTO dto)
        {
            Order? order = await _unitOfWork.Orders.GetOrderEntityByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            if (order.Status != eOrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot update the shipping address because the order is no longer Pending.");
            }

            order.ShippingAddress = dto.NewShippingAddress;

            await _unitOfWork.CompleteAsync();

            OrderDTO responseDto = new OrderDTO
            {
                ID = order.ID,
                OrderTime = order.OrderTime,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product?.Name ?? "Unknown Product",
                    Quantity = oi.Quantity
                }).ToList(),
                User = new UserDTO
                {
                    ID = order.UserId,
                    Username = order.User.UserName!
                }
            };

            return responseDto;
        }

        public async Task<OrderDTO?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO dto)
        {
            Order? order = await _unitOfWork.Orders.GetOrderEntityByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            eOrderStatus newStatus;
            if (!Enum.TryParse<eOrderStatus>(dto.NewStatus, true, out newStatus))
            {
                throw new ArgumentException($"'{dto.NewStatus}' is not a valid order status.");
            }

            if (order.Status == eOrderStatus.Cancelled || order.Status == eOrderStatus.Delivered)
            {
                throw new InvalidOperationException("Cannot modify the status of an order that has already been canceled or delivered.");
            }

            order.Status = newStatus;

            await _unitOfWork.CompleteAsync();

            OrderDTO responseDto = new OrderDTO
            {
                ID = order.ID,
                OrderTime = order.OrderTime,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product?.Name ?? "Unknown Product",
                    Quantity = oi.Quantity
                }).ToList(),
                User = new UserDTO
                {
                    ID = order.UserId,
                    Username = order.User.UserName!
                }
            };

            return responseDto;
        }
    }
}
