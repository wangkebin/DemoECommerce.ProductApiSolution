using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversion
{
    public static Product ToProduct(this ProductDTO productDTO) => new()
    {
        Id = productDTO.Id,
        Name = productDTO.Name,
        Quantity = productDTO.Quantity,
        Price = productDTO.Price
    };

    public static (ProductDTO?, IEnumerable<ProductDTO>?) FromProduct(Product product, IEnumerable<Product>? products)
    {
        // return single
        if (product is not null || products is null)
        {
            return (new ProductDTO(
                product!.Id,
                product.Name!,
                product.Quantity,
                product.Price), null);
        }
        // return list
        if (products is not null || product is null)
        {
            return (null, products!.Select(p =>
                new ProductDTO(p.Id, p.Name!, p.Quantity, p.Price)).ToList());
        }

        return (null, null);
    }

}