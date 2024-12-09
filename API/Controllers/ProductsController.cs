using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductsDbContext _context;

    public ProductsController(ProductsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts(
    [FromQuery] string? sortBy = null,
    [FromQuery] string? sortOrder = "asc",
    [FromQuery] string? filterByName = null)
    {
        var query = _context.Product.AsQueryable();

        if (!string.IsNullOrEmpty(filterByName))
        {
            query = query.Where(p => p.Name.Contains(filterByName));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "name":
                    query = sortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name);
                    break;
                case "price":
                    query = sortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price);
                    break;
                default:
                    return BadRequest("Invalid sortBy parameter.");
            }
        }

        return query.ToList();
    }


    [HttpGet("{name}")]
    public ActionResult<Product> GetProduct(string name)
    {
        var product = _context.Product.Find(name);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [HttpPost]
    public ActionResult<Product> PostProduct(Product product)
    {
        _context.Product.Add(product);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetProduct), new { name = product.Name }, product);
    }

    [HttpPut("{name}")]
    public IActionResult PutFilm(string name, Product product)
    {
        if (name != product.Name)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{name}")]
    public IActionResult DeleteFilm(string name)
    {
        var product = _context.Product.Find(name);
        if (product == null)
        {
            return NotFound();
        }

        _context.Product.Remove(product);
        _context.SaveChanges();

        return NoContent();
    }
}
