using BusinessLogicLayer.Dtos.CategoryDtos;

namespace BusinessLogicLayer.Dtos.CharmDtos;

public class CustomBraceletViewModel
{
    public Guid CustomBraceletId { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public string? CustomBraceletName { get; set; }
    public string? Image { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? TotalCapitalExpense { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Note { get; set; }

    public virtual CategoryViewModel Category { get; set; } = null!;
    public virtual ICollection<CustomBraceletCharmViewModel> CustomBraceletCharms { get; set; } = new List<CustomBraceletCharmViewModel>();
}
