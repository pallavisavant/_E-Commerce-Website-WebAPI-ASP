using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.API.Models;

namespace WebApplication4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ShopContext _context;

    public ProductsController(ShopContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllProducts()
    {
        return Ok(await _context.Products.ToArrayAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProduct(int id){
        var product = await _context.Products.FindAsync(id);
        if (product == null){
            return NotFound();
        }
        return Ok(product);
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(
            "GetProduct",
            new {id = product.Id },
            product
        );

    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutProduct(int id, Product product)
    {
        if (id != product.Id){
            return BadRequest();

        }

        _context.Entry(product).State = EntityState.Modified;
        try{
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException){
            if (! _context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }
            else{
                throw;
            }
        }
        return NoContent();

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Product>> DeleteProduct(int id){
        var product = await _context.Products.FindAsync(id);
        if(product == null){
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return product;
    }

     [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery]int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
    
}
