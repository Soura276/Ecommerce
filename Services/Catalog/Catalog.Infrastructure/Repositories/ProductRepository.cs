﻿using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
              _context = context;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context.Brands.Find(x => true).ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context.Types.Find(x => true).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter = filter & builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                var brandFilter = builder.Eq(p => p.Brands.Id, catalogSpecParams.BrandId);
                filter &= brandFilter;
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                var typeFilter = builder.Eq(p => p.Types.Id, catalogSpecParams.TypeId);
                filter &= typeFilter;
            }
            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogSpecParams, filter);
            //var data = await _context.Products
            //    .Find(filter)
            //    .Skip((catalogSpecParams.PageIndex-1)*catalogSpecParams.PageSize)
            //    .Limit(catalogSpecParams.PageSize)
            //    .ToListAsync();
            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                totalItems,
                data
            );
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string brandName)
        {
            return await _context.Products.Find(x => x.Brands.Name.ToLower() == brandName.ToLower()).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await _context.Products.Find(x => x.Name.ToLower() == name.ToLower()).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id , product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefn = Builders<Product>.Sort.Ascending(p => p.Name);
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceAsc":
                        sortDefn = Builders<Product>.Sort.Ascending(p => p.Price);
                        break;
                    case "priceDesc":
                        sortDefn = Builders<Product>.Sort.Descending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }
            return await _context.Products
                .Find(filter)
                .Sort(sortDefn)
                .Skip((catalogSpecParams.PageIndex - 1) * catalogSpecParams.PageSize)
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();
        }
    }
}
