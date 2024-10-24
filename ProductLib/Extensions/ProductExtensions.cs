using ProductLib.Models;

namespace ProductLib;

public static class ProductExtensions
{
    public static ProductResponse ToResponse(this Product prd)
    {
        return new ProductResponse()
        {
            Id = prd.Id,
            Code = prd.Code,
            Name = prd.Name,
            Category = Enum.GetName<Category>(prd.Category)
        };
    }
    public static Product ToEntity(this ProductCreateReq req)
    {
        var category = Category.None;
        Category.TryParse(req.Category, out category);
        return new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Code = req.Code,
            Name = req.Name,
            Category = category,
            Created = DateTime.Now,
            LastUpdated = null
        };
    }

    public static void Copy(this Product prd , ProductUpdateReq req)
    {   
        prd.Name = req.Name ?? prd.Name;
        prd.Category = req.Category != null ? Enum.Parse<Category>(req.Category) : prd.Category;
    }
    
    public static Product Clone(this Product prd)
    {
        return new Product()
        {
            Id = prd.Id,
            Code = prd.Code,
            Name = prd.Name,
            Category = prd.Category,
            Created = prd.Created,
            LastUpdated = prd.LastUpdated,
        };
    }
    public static void Copy(this Product prd, Product other)
    {
        prd.Id = other.Id;
        prd.Code = other.Code;
        prd.Name = other.Name;
        prd.Category = other.Category;
        prd.Created = other.Created;
        prd.LastUpdated = other.LastUpdated;
    }
   
}


