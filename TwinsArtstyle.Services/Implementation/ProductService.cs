﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository _repository;

        public ProductService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddProduct(ProductViewModel productViewModel)
        {
            Product product = new Product()
            {
                Name = productViewModel.Name,
                Description = productViewModel.Description,
                ImageUrl = productViewModel.ImageUrl,
                Price = productViewModel.Price,
                Category = await _repository.All<Category>()
                .Where(c => c.Name == productViewModel.Category).FirstOrDefaultAsync()
            };

            if(product.Category == null)
            {
                throw new ArgumentNullException("Invalid category!");
            }

            await _repository.Add(product);
            await _repository.SaveChanges();
        }

        public async Task<IEnumerable<ProductViewModel>> GetProducts()
        {
            return await _repository.All<Product>()
                .Select(p => new ProductViewModel()
                {
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
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    Category = p.Category.Name,
                    Description = p.Description,
                    Price = p.Price

                }).ToListAsync();
        }
    }
}
