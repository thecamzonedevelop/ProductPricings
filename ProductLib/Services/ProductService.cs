using RestClientLib;
using System.Net;
using ProductLib.Models;
using ProductLib.repository;

namespace ProductLib;

public class ProductService
{
    private static readonly List<ProductCreateReq> reqs =
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
    public static List<ProductCreateReq> InitRequest => reqs;

    private readonly ProductRepository _repo;
    public ProductService(ProductRepository repo)
    {
        _repo = repo;
    }
    public List<string?> Initialize()
    {
        if (!_repo.GetQueryable().Any())
        {
            var products = reqs.Select(req => req.ToEntity()).ToList();
            _repo.Create(products);
            return products.Select(x => x.Id).ToList();
        }
        return [];
    }

    public Result<string?> Create(ProductCreateReq req)
    {
        req.Code = req.Code.Trim();
        if (string.IsNullOrEmpty(req.Code))
            return Result<string?>.Fail(HttpStatusCode.BadRequest, $"The request's code is required");

        if (Exist(req.Code).Data == true)
            return Result<string?>.Fail(HttpStatusCode.Found, $"The product with the code, {req.Code}, does already exist");
    
        Product prd = req.ToEntity();
        _repo.Create(prd);
        return Result<string?>.Success(prd.Id, HttpStatusCode.Created, "Successfully created");
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
                          .FirstOrDefault(x => string.Equals(x.Id, key, StringComparison.OrdinalIgnoreCase)
                                            || string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
        return Result<ProductResponse?>.Success(entity?.ToResponse());
    }
   
    public Result<bool> Exist(string key)
    {
        var result = _repo.GetQueryable()
                          .Any(x => string.Equals(x.Id, key, StringComparison.OrdinalIgnoreCase)
                                 || string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
        return Result<bool>.Success(result, result?HttpStatusCode.Found:HttpStatusCode.NotFound);
    }
    public Result<bool> Update(ProductUpdateReq req)
    {
        var found = _repo.GetQueryable()
                         .FirstOrDefault(x => string.Equals(x.Id, req.Key, StringComparison.OrdinalIgnoreCase) 
                                           || string.Equals(x.Code, req.Key, StringComparison.OrdinalIgnoreCase));
        if (found == null)
            return Result<bool>.Fail(HttpStatusCode.NotFound, $"No product with id/code, {req.Key}");
        var prd = found.Clone();
        prd.Copy(req);
        prd.LastUpdated = DateTime.Now;
        var result = _repo.Update(prd);
        return result == true 
               ? Result<bool>.Success(result, HttpStatusCode.NoContent, "Successfully updated")
               : Result<bool>.Fail(HttpStatusCode.NotFound, $"Failed to update product with id/code, {req.Key}");
    }
   
    public Result<bool> Delete(string key)
    {
        var found = _repo.GetQueryable()
                         .FirstOrDefault(x => string.Equals(x.Id, key, StringComparison.OrdinalIgnoreCase) 
                                           || string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
        if (found == null)
            return Result<bool>.Fail(HttpStatusCode.NotFound, $"No product with id/code, {key}");
        
        var result = _repo.Delete(found.Id!);
        return result == true 
               ? Result<bool>.Success(result, HttpStatusCode.NoContent, "Successfully deleted")
               : Result<bool>.Fail(HttpStatusCode.Forbidden, $"Failed to delete product with id/code, {key}");
    }
}
