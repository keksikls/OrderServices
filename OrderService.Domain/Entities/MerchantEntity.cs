namespace OrderService.Domain.Entities;

public class MerchantEntity : BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Phone { get; set; }
    public string Website { get; set; }
    public List<UserEntity> Users { get; set; }
}