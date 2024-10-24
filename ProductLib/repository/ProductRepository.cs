using ProductLib.Context;

namespace ProductLib.repository;
using System.Collections.Generic;

public class ProductRepository
{
    private readonly FileContext _context;

    public ProductRepository(FileContext context)
    {
        _context = context;
    }
    
    public void Create(Product entity)
    {
        _context.Products.Add(entity.Clone());
        _context.SaveChanges();
    }
    public void Create(List<Product> products)
    {
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    public IQueryable<Product> GetQueryable()=> _context.Products.AsQueryable();

    public bool Update(Product entity)
    {
        var found = GetQueryable().FirstOrDefault(x => x.Id == entity.Id);
        if (found != null)
        {
            found.Copy(entity);
            _context.SaveChanges();
        }
        return found != null;
    }
    public bool Delete(string id)
    {
        var found = _context.Products.Find(x => x.Id == id);
        if (found == null) return false;

        var result = _context.Products.Remove(found);
        _context.SaveChanges();
        return result;
    }
}