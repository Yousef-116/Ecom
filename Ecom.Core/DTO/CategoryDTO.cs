namespace Ecom.Core.DTO;

public record CategoryDTO(string Name, string Description);
public record UpdateCategoryDTO(int id, string Name, string Description);
