﻿using Microsoft.EntityFrameworkCore;
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
using TwinsArtstyle.Services.ViewModels.ProductModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository _repository;

        public ProductService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> AddProduct(ProductViewModel productViewModel)
        {
            var result = new OperationResult();
            Product product = new Product()
            {
                Name = productViewModel.Name,
                Description = productViewModel.Description,
                ImageUrl = productViewModel.ImageUrl,
                Price = productViewModel.Price,
                Category = await _repository.All<Category>()
                .Where(c => c.Name == productViewModel.Category).FirstOrDefaultAsync()
            };

            if (product.Category == null)
            {
                result.ErrorMessage = "Invalid category!";
            }

            try
            {
                await _repository.Add(product);
                await _repository.SaveChanges();
                result.Success = true;
            }
            catch (DbUpdateException)
            {
                result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
            }

            return result;
        }

        public async Task<IEnumerable<ProductViewModel>> GetProducts()
        {
            return await _repository.All<Product>()
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<ProductViewModel>> GetByCategory(string categoryName)
        {
            return await _repository.All<Product>()
                .Where(p => p.Category.Name == categoryName)
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id.ToString(),
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    Category = p.Category.Name,
                    Description = p.Description,
                    Price = p.Price

                }).ToListAsync();
        }

        public async Task<bool> Exists(string productId)
        {
            return (await _repository.FindById<Product>(new Guid(productId))) == null ? false: true;
        }

        public async Task<ProductViewModel> GetById(string productId)
        {
            return await _repository.All<Product>()
                .Where(p => p.Id.ToString() == productId)
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Description = p.Description,
                })
                .FirstOrDefaultAsync();
        }
    }
}
