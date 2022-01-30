using CWA.DistributedCache;
using DistributedCachingSampleApp.CacheKey;
using DistributedCachingSampleApp.DomainModel;
using DistributedCachingSampleApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DistributedCachingSampleApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly CacheEntryOptions _cacheEntryOptions;
    private readonly IAsyncRepository<Customer> _repository;
    private readonly ICacheManager _cacheManager;

    public CustomerController(IOptions<CacheEntryOptions> cacheEntryOptions,
        IAsyncRepository<Customer> repository, ICacheManager cacheManager)
    {
        _repository = repository;
        _cacheManager = cacheManager;
        _cacheEntryOptions = cacheEntryOptions.Value;
    }

    [HttpGet]
    public async Task<List<Customer>?> Get()
    {
        string cacheKey = CustomerCacheKeys.ListKey;
        List<Customer>? customerList = await _cacheManager.GetAsync<List<Customer>>(cacheKey);
        if (customerList == null)
        {
            customerList = await _repository.ListAllAsync();
            await _cacheManager.SetAsync(cacheKey, customerList, GetOptions()).ConfigureAwait(false);
        }

        return customerList;
    }

    [HttpGet("{customerId}")]
    public async Task<ActionResult<Customer>> Get(int customerId)
    {
        string cacheKey = CustomerCacheKeys.GetKey(customerId);
        Customer? customer = await _cacheManager.GetAsync<Customer>(cacheKey);
        if (customer != null) return customer;
        
        customer =  await _repository.GetByIdAsync(customerId);
        if (customer == null) return NotFound();
        
        await _cacheManager.SetAsync(cacheKey, customer, GetOptions()).ConfigureAwait(false); 
        return customer;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        await _repository.AddAsync(customer);
        string cacheKey = CustomerCacheKeys.GetKey(customer.Id);
        await _cacheManager.SetAsync(cacheKey, customer, GetOptions()).ConfigureAwait(false);
        
        // Add customer to list
        string cacheKeyList = CustomerCacheKeys.ListKey;
        await _cacheManager
            .AddToListAsync(cacheKeyList, customer,
                orderBy => orderBy.Id, 
                true,
                GetOptions());
        
        return customer;
    }


    [HttpPut("{customerId}")]
    public async Task<ActionResult<Customer>> Put(int customerId, Customer customer)
    {
        if (customerId != customer.Id)
        {
            return BadRequest();
        }

        await _repository.UpdateAsync(customer);
        
        // update customer in list
        string cacheKey = CustomerCacheKeys.ListKey;
        await _cacheManager.UpdateInListAsync(
            cacheKey,
            predicate => predicate.Id == customerId,
            customer,
            orderBy => orderBy.Id, 
            true,
            GetOptions());

        return customer;
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> Delete(int customerId)
    {
        var customer = await _repository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        // remove customer from list
        string cacheKey = CustomerCacheKeys.ListKey;
        await _cacheManager.RemoveFromListAsync<Customer>(
            cacheKey,
            _ => _repository.ListAllAsync()
                .GetAwaiter()
                .GetResult()
                .Any(x => x.Id == customerId),
            GetOptions());

       await _repository.DeleteAsync(customer);

        return NoContent();
    }
    
    private DistributedCacheEntryOptions GetOptions()
    {
        var absoluteExpiration = _cacheEntryOptions.AbsoluteExpirationInHours ?? 8;
        var slidingExpiration = _cacheEntryOptions.SlidingExpirationInMinutes ?? 60;
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddHours(absoluteExpiration),
            SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration)
        };
        return options;
    }
}


