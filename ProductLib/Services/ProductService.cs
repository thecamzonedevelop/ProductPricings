using RestClientLib;
using System.Net;
using ProductLib.Models;

namespace ProductLib;

public class ProductService
{
    private static readonly List<ProductCreateReq> _reqs =
    [
            new()
            {
                Code = "PRD001",
                Name = "Coca",
                Category= "Food"
            },
            new()
            {
                Code = "PRD002",
                Name = "Dream 125",
                Category= "Vehicle"
            },
            new()
            {
                Code = "PRD003",
                Name = "TShirt-SEA game 2023",
                Category= "Cloth"
            }
    ];
    public static List<ProductCreateReq> InitRequest => _reqs;

    private readonly ProductRepository _repo;
    public ProductService(ProductRepository repo)
    {
        _repo = repo;
    }
 
    public Result<string?> Create(ProductCreateReq req)
    {
        req.Code = req.Code.Trim();
        if (string.IsNullOrEmpty(req.Code))
            return Result<string?>.Fail(HttpStatusCode.BadRequest, $"The request's code is required");

        if (Exist(req.Code).Data == true)
            return Result<string?>.Fail(HttpStatusCode.Found, $"The product with the code, {req.Code}, does already exist");
    
        Product prd = req.ToEntity();
        try
        {
            int n = _repo.CreateAsync(prd).GetAwaiter().GetResult();
            if (n > 0) 
                return Result<string?>.Success(prd.Id, HttpStatusCode.Created, "Successfully created");
            else
                return Result<string?>.Fail(HttpStatusCode.Conflict, "Fail to create a new product");
        }
        catch(Exception ex)
        {
            return Result<string?>.Fail(HttpStatusCode.Conflict, $"{ex.Message}");
        }
        
    }
    
    public Result<List<ProductResponse>> ReadAll()
    {
        var result = _repo.GetQueryable()
                          .Select(x => x.ToResponse())
                          .ToList();
        return Result<List<ProductResponse>>.Success(result);
    }

    public Result<ProductResponse?> Read(string key)
    {
        key = key.ToLower();
        var entity = _repo.GetQueryable()
                          .FirstOrDefault(x => x.Id == key || x.Code == key);
        return Result<ProductResponse?>.Success(entity?.ToResponse());
    }
   
    public Result<bool> Exist(string key)
    {
        key = key.ToLower();
        var result = _repo.GetQueryable()
                          .Any(x => x.Id == key || x.Code == key);
        return Result<bool>.Success(result, result?HttpStatusCode.Found:HttpStatusCode.NotFound);
    }
    public Result<bool> Update(ProductUpdateReq req)
    {
        var key = req.Key.ToLower();
        var found = _repo.GetQueryable()
                         .FirstOrDefault(x => x.Id == key || x.Code == key);
        if (found == null)
            return Result<bool>.Fail(HttpStatusCode.NotFound, $"No product with id/code, {req.Key}");
        var prd = found.Clone();
        prd.Copy(req);
        prd.LastUpdated = DateTime.Now;
        try
        {
            int n = _repo.UpdateAsync(prd).GetAwaiter().GetResult();
            return n > 0
                   ? Result<bool>.Success(true, HttpStatusCode.NoContent, "Successfully updated")
                   : Result<bool>.Fail(HttpStatusCode.Conflict, $"Failed to update product with id/code, {req.Key}");

        }
        catch(Exception ex)
        {
            return Result<bool>.Fail(HttpStatusCode.Conflict, $"{ex.Message}");
        }
    }
    public Result<bool> Delete(string key)
    {
        key = key.ToLower();
        var found = _repo.GetQueryable()
                         .FirstOrDefault(x => x.Id == key || x.Code == key);
        if (found == null)
            return Result<bool>.Fail(HttpStatusCode.NotFound, $"No product with id/code, {key}");
        try
        {
            int n = _repo.DeleteAsync(found.Id!).GetAwaiter().GetResult();
            return n > 0
                   ? Result<bool>.Success(true, HttpStatusCode.NoContent, "Successfully deleted")
                   : Result<bool>.Fail(HttpStatusCode.Forbidden, $"Failed to delete product with id/code, {key}");
        }
        catch(Exception ex)
        {
            return Result<bool>.Fail(HttpStatusCode.Conflict, $"{ex.Message}");
        }
    }
}
