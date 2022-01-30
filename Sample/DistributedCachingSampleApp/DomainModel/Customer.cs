namespace DistributedCachingSampleApp.DomainModel;
public class Customer : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public int? Mark { get; set; }
}