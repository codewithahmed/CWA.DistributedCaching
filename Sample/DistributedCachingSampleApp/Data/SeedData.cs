using DistributedCachingSampleApp.DomainModel;

namespace DistributedCachingSampleApp.Data;

public static class SeedData
{
  public static List<Customer> Customers()
  {
    int id = 1;

    var customers = new List<Customer>()
    {
      new Customer {Id = id++, FirstName = "Ahmed", LastName = "Qaid", Mark = 20},
      new Customer {Id = id++, FirstName = "Ali", LastName = "Mohammed", Mark = 30},
      new Customer {Id = id++, FirstName = "Saud", LastName = "Alfadhli", Mark = 40},
    };
    return customers;
  }

  public static void PopulateTestData(AppDbContext dbContext)
  {
    dbContext.Customer.AddRange(Customers());

    dbContext.SaveChanges();
  }
}

