﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRepository repository;
        private readonly UserManager<User> _userManager;

        public CartService(IRepository repo,
            UserManager<User> userManager)
        {
            repository = repo;
            _userManager = userManager;
        }

        public async Task<Cart> CreateCart()
        {
            Cart cart = new Cart();
            await repository.Add(cart);
            await repository.SaveChanges();
            return cart;
        }

        public async Task<OperationResult> DeleteCart(Guid cartId)
        {
            var result = new OperationResult();

            var cart = await repository.FindById<Cart>(cartId);

            if (cart != null)
            {
                try
                {
                    repository.Remove(cart);
                    await repository.SaveChanges();
                    result.Success = true;
                }
                catch (DbUpdateException)
                {
                    result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
                }
            }

            return result;
        }

        public async Task<OperationResult> AddToCart(string cartId, string productId, int count)
        {
            var operationResult = new OperationResult();
            var cartGuid = new Guid(cartId);
            var productGuid = new Guid(productId);
            var cart = await repository.FindById<Cart>(cartGuid);
            var product = await repository.FindById<Product>(productGuid);

            if (cart != null && product != null && count > 0)
            {
                try
                {
                    var cartProduct = repository.All<CartProductCount>()
                        .Where(c => c.Cart == cart && c.Product == product)
                        .FirstOrDefault();

                    if (cartProduct != null)
                    {
                        cartProduct.Count += count;
                    }
                    else
                    {
                        await repository.Add(new CartProductCount()
                        {
                            Cart = cart,
                            Product = product,
                            Count = count
                        });
                    }

                    await repository.SaveChanges();
                    operationResult.Success = true;
                }
                catch (DbUpdateException)
                {
                    operationResult.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
                }
            }

            return operationResult;
        }

        public async Task<IEnumerable<CartProductViewModel>> GetProductsForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userCartId = user.CartId;

                var items = await repository.All<CartProductCount>()
                    .Where(c => c.CartId == userCartId)
                    .Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Id = p.ProductId.ToString(),
                            Name = p.Product.Name,
                            ImageUrl = p.Product.ImageUrl,
                            Price = p.Product.Price,
                            Category = p.Product.Category.Name
                        },
                        Count = p.Count
                    }).ToListAsync();

                return items;
            }

            return Enumerable.Empty<CartProductViewModel>();
        }

        public async Task<IEnumerable<CartProductViewModel>> GetProductsForUser(ClaimsPrincipal user)
        {

            if (user != null)
            {
                var userCartId = user.FindFirst("CartId").Value;

                var items = await repository.All<CartProductCount>()
                    .Where(c => c.CartId.ToString() == userCartId)
                    .Select(p => new CartProductViewModel()
                    {
                        Product = new ProductViewModel()
                        {
                            Id = p.ProductId.ToString(),
                            Name = p.Product.Name,
                            ImageUrl = p.Product.ImageUrl,
                            Price = p.Product.Price,
                            Category = p.Product.Category.Name
                        },
                        Count = p.Count
                    }).ToListAsync();

                return items;
            }

            return Enumerable.Empty<CartProductViewModel>();
        }

        public async Task<decimal> CalculateTotalPrice(string cartId)
        {
            decimal totalPrice = 0;

            if (cartId != string.Empty)
            {
                totalPrice = await repository.All<CartProductCount>()
                     .Where(c => c.CartId.ToString() == cartId)
                     .SumAsync(p => p.Product.Price * p.Count);
            }

            return totalPrice;
        }

        public async Task<OperationResult> RemoveFromCart(string productId, string cartId)
        {
            var entity = await repository.All<CartProductCount>()
                .Where(c => c.CartId.ToString() == cartId && c.ProductId.ToString() == productId)
                .FirstOrDefaultAsync();
            var result = new OperationResult();

            if (entity != null)
            {
                try
                {
                    repository.Remove(entity);
                    await repository.SaveChanges();
                    result.Success = true;
                }
                catch (DbUpdateException)
                {
                    result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
                }
            }

            return result;
        }

        public async Task<OperationResult> CleanCart(string cartId)
        {
            var result = new OperationResult();
            var cartItems = await repository.All<CartProductCount>()
                .Where(c => c.CartId.ToString() == cartId)
                .ToListAsync();

            try
            {
                repository.RemoveRange(cartItems);
                await repository.SaveChanges();
                result.Success = true;
            }
            catch (DbUpdateException)
            {
                result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
            }

            return result;
        }
    }
}
