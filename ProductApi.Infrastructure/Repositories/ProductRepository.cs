using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class ProductRepository(ProductDbContext context) : IProduct
{
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            var getProduct = await GetByAsync(_=>_.Name!.Equals(entity.Name));
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
            {
                return new Response(false, $"Product with name {getProduct.Name} already exists.");
            }

            var currentProduct = context.Products.Add(entity).Entity;
            await context.SaveChangesAsync();
            if (currentProduct is not null && currentProduct.Id > 0)
            {
                return new Response(true, $"Product with id {currentProduct.Id} has been created.");
            }
            else
            {
                return new Response(false, $"Product with name {entity.Name} failed to be created.");
            }
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);
            
            // return to client
            return new Response(false, "Error creating product"); 
        }
    }

    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            var product = await GetByIdAsync(entity.Id);
            if (product is null || !string.IsNullOrEmpty(product.Name))
            {
                return new Response(false, $"Product with id {entity.Id} does not exists.");
            }

            var currentProduct = context.Products.Update(entity).Entity;
            var rowUpdated = await context.SaveChangesAsync();
            if (rowUpdated > 0)
            {
                return new Response(true, $"Product with id {currentProduct.Id} has been updated.");
            }
            else
            {
                return new Response(false, $"Product with id {entity.Id} failed to be updated.");
            }
        } catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);
            
            // return to client
            return new Response(false, "Error updating product"); 
        }
    }

    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            var product = await GetByIdAsync(entity.Id);
            if (product is null)
            {
                return new Response(false, $"Product with id {entity.Id} does not exist.");
            }
            var currentProduct = context.Products.Remove(product);
            var rowsUpdated = await context.SaveChangesAsync();
            if (rowsUpdated == 1)
            {
                return new Response(true, $"Product with id {product.Id} has been deleted.");
            }
            else
            {
                return new Response(false, $"Product with name {product.Name} failed to be deleted.");
            }

        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);
            
            // return to client
            return new Response(false, "Error update product");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            var products = await context.Products.AsNoTracking().ToListAsync();
            return products is not null ? products : null!;
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);

            // return to client
            throw new Exception("cannot get all products");
        }
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);
            return product is not null ? product : null!;
        }catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);
            
            // return to client
            throw new Exception( $"cannot locate product by id {id}");
        }
    }

    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);

            // return to client
            throw new Exception("Error retriving product");
        }
        
    }
}