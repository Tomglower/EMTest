namespace EMTest.DTO;

public class DeliveryOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public double Weight { get; set; }
    public string District { get; set; }
    public DateTime DeliveryTime { get; set; }
}