using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;
using TwinsArtstyle.Services.ViewModels.OrderModels;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IRepository _repository;
        private readonly ICartService _cartService;
        private readonly UserManager<User> _userManager;

        public OrderService(IRepository repository, ICartService cartService,
            UserManager<User> userManager)
        {
            _repository = repository;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<OperationResult> Add(OrderDTO orderViewModel, string userId)
        {
            var result = new OperationResult();
            var user = await _userManager.FindByIdAsync(userId);
            var products = await _cartService.GetProductsForUser(userId);
            var totalPriceForCart = await _cartService.CalculateTotalPrice(user.CartId.ToString());

            Order order = new Order()
            {
                Address = await _repository.All<Address>().Where(a => a.Name == orderViewModel.AddressName)
                                    .FirstOrDefaultAsync(),
                User = user,
                Price = totalPriceForCart,
            };

            foreach (var item in products)
            {
                order.OrderProducts.Add(new OrderProductCount()
                {
                    OrderId = order.Id,
                    ProductId = new Guid(item.Product.Id),
                    Count = item.Count,
                });
            }

            try
            {
                await _repository.Add(order);
                await _repository.SaveChanges();
                result.Success = true;
            }
            catch (DbUpdateException)
            {
                result.ErrorMessage = Messages.DbUpdateFailed;
            }   

            return result;
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersForUser(string userId)
        {
            var orders = await _repository.All<Order>()
                .Where(o => o.UserId == userId)
                .Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    AddressName = o.Address.Name,
                    TotalPrice = o.Price,
                    Products = o.OrderProducts.Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Name = p.Product.Name,
                            Category = p.Product.Category.Name,
                            Price = p.Product.Price,
                            Description = p.Product.Description,
                            ImageUrl = p.Product.ImageUrl
                        },
                        Count = p.Count
                    }).ToList()

                }).ToListAsync();

            return orders;
        }

        public async Task<decimal> CalculateTotalPrice(string orderId)
        {
            decimal totalPrice = 0;

            if (orderId != string.Empty)
            {
                totalPrice = await _repository.All<OrderProductCount>()
                     .Where(c => c.OrderId.ToString() == orderId)
                     .SumAsync(p => p.Product.Price * p.Count);
            }

            return totalPrice;
        }

        public async Task<OrderViewModel> GetOrderById(string orderId)
        {
            return await _repository.All<Order>()
                .Where(o => o.Id.ToString() == orderId)
                .Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    AddressName = o.Address.Name,
                    TotalPrice = o.Price,
                    Products = o.OrderProducts.Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Name = p.Product.Name,
                            Category = p.Product.Category.Name,
                            Price = p.Product.Price,
                            Description = p.Product.Description,
                            ImageUrl = p.Product.ImageUrl
                        },
                        Count = p.Count
                    }).ToList(),
                    OrderStatus = o.Status
                }).FirstOrDefaultAsync();
        }

        public async Task<AdminAreaOrderViewModel> GetAdminOrderById(string orderId)
        {
            return await _repository.All<Order>()
                .Where(o => o.Id.ToString() == orderId)
                .Select(o => new AdminAreaOrderViewModel()
                {
                    OrderId = o.Id,
                    User = new UserViewModel()
                    {
                        Email = o.User.Email,
                        FirstName = o.User.FirstName,
                        LastName = o.User.LastName,
                        PhoneNumber = o.User.PhoneNumber,
                    },
                    AddressText = o.Address.AddressText,
                    TotalPrice = o.Price,
                    Status = o.Status,
                    CreationDate = o.CreationDate,
                    Products = o.OrderProducts.Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Name = p.Product.Name,
                            Category = p.Product.Category.Name,
                            Price = p.Product.Price,
                            Description = p.Product.Description,
                            ImageUrl = p.Product.ImageUrl
                        },
                        Count = p.Count
                    }).ToList()

                }).FirstOrDefaultAsync();
            
        }

        public async Task<IEnumerable<AdminAreaOrderViewModel>> GetOrdersForAdminArea()
        {
            return await _repository.All<Order>()
                .Select(o => new AdminAreaOrderViewModel()
                {
                    OrderId = o.Id,
                    User = new UserViewModel()
                    {
                        Email = o.User.Email,
                        FirstName = o.User.FirstName,
                        LastName = o.User.LastName,
                        PhoneNumber = o.User.PhoneNumber,
                    },
                    AddressText = o.Address.AddressText,
                    Status = o.Status,
                    CreationDate = o.CreationDate,
                    TotalPrice = o.Price,
                    Products = o.OrderProducts.Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Category = p.Product.Category.Name,
                            Name = p.Product.Name,
                            Description = p.Product.Description,
                            Price = p.Product.Price,
                            ImageUrl = p.Product.ImageUrl
                        },
                        Count = p.Count

                    }).ToList()

                }).ToListAsync();
        }
    }
}
