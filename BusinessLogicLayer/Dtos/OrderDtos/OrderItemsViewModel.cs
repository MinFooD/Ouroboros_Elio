using BusinessLogicLayer.Dtos.CharmDtos;
using BusinessLogicLayer.Dtos.DesignDtos;

namespace BusinessLogicLayer.Dtos.OrderDtos;

public class OrderItemsViewModel
{
    public Guid OrderItemId { get; set; }

    public Guid? OrderId { get; set; }

    public bool ProductType { get; set; }

    public Guid? DesignId { get; set; }

    public Guid? CustomBraceletId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual DesignViewModel? Design { get; set; }

    public virtual OrderViewModel? Order { get; set; }

    public virtual CustomBraceletViewModel? CustomBracelet { get; set; }
}
