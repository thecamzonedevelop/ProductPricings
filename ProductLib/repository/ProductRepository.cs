using Microsoft.EntityFrameworkCore;

namespace ProductLib;

public class ProductRepository
{
    private readonly DbContext _context;

    public ProductRepository(DbContext context)
    {
        _context = context;
    }

    public Task<int> CreateAsync(Product entity)
    {
        _context.Set<Product>().Add(entity.Clone());
        return _context.SaveChangesAsync();
    }
    public Task<int> CreateAsync(List<Product> products)
    {
        _context.Set<Product>().AddRange(products);
        return _context.SaveChangesAsync();
    }

    public IQueryable<Product> GetQueryable() => _context.Set<Product>().AsQueryable();

    public Task<int> UpdateAsync(Product entity)
    {
        var found = GetQueryable().FirstOrDefault(x => x.Id == entity.Id);
        if (found != null)
        {
            found.Copy(entity);
            return _context.SaveChangesAsync();
        }
        return Task.FromResult(0);
    }
    public Task<int> DeleteAsync(string id)
    {
        var found = _context.Set<Product>().FirstOrDefault(x => x.Id == id);
        if (found == null) return Task.FromResult(0);

        var result = _context.Set<Product>().Remove(found);
        return _context.SaveChangesAsync();
    }

}