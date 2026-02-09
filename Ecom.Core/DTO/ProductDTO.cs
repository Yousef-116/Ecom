using Ecom.Core.Entites.Product;

namespace Ecom.Core.DTO;

public record ProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public virtual List<PhotoDTO> Photos { get; set; }
    public string CategoryName { get; set; }
}

public record PhotoDTO
{
    public string ImageName { get; set; }

    public int ProductId { get; set; }
}
