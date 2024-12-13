using Core.Data.DTOs;
using Core.Interfaces;
using Core.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        // Simuler une base de données
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Produit A" },
            new Product { Id = 2, Name = "Produit B" }
        };

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Mapping simple, à améliorer avec AutoMapper par exemple
            var dtos = _products.Select(p => new ProductDto { Id = p.Id, Name = p.Name });
            return await Task.FromResult(dtos);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return null;

            var dto = new ProductDto { Id = product.Id, Name = product.Name };
            return await Task.FromResult(dto);
        }

        // Autres méthodes implémentées
    }
}