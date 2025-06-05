using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppCalisto.Data.Repositories
{
    public class ProductRepository : EntityRepository<Product>, IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().Include(p => p.Service).ThenInclude(s => s.Company).ToListAsync();
        }

        public async Task<List<Product>> GetByServiceIdAsync(int serviceId)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Service).ThenInclude(s => s.Company)
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Service).ThenInclude(s => s.Company).FirstOrDefaultAsync(p => p.Id == id);
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var products = _context.Products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            })
            .ToList();

            products.Insert(0, new SelectListItem
            {
                Text = "(Select a product...)",
                Value = string.Empty
            });

            return products;
        }
    }
}
