# CWA.DistributedCaching
A simple package to implement and extended the functionality of `IDistributedCache` interface (Memory, Redis) to make the caching more easier.

## Prerequisites

### Setting Up Redis

This step `Creating a Redis docker container` is if you are going to deal with redis caching,
but if you are going to deal with memory caching you don't need it.

For this step, I assume that you have already installed Docker on your machine. 
It’s handy to have this so that you can spin up your own Redis container whenever you want for development purposes.

#### Creating a Redis docker container
    docker run --name redis-cache -p 6379:6379 -d redis

#### By default, Redis runs on the local 6379 port , To change the port you can use this commend
    docker run --name my-redis -p 5002:6379 -d redis

## Installation

First install the latest version of `CWA.DistributedCaching` nuget package into your project as follows:

    Install-Package CWA.DistributedCaching

## Basic usage
```C#
    [HttpGet]
    public async Task<List<Customer>?> Get()
    {
        string cacheKey = CustomerCacheKeys.ListKey;
        List<Customer>? customerList = await _distributedCache.GetAsync<List<Customer>>(cacheKey);
        if (customerList == null)
        {
            customerList = await _repository.ListAllAsync();
            await _distributedCache.SetAsync(cacheKey, customerList, GetOptions()).ConfigureAwait(false);
        }
        return customerList;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        await _repository.AddAsync(customer);
        string cacheKey = CustomerCacheKeys.GetKey(customer.Id);
        await _distributedCache.SetAsync(cacheKey, customer, GetOptions()).ConfigureAwait(false);
    
        // Add customer to list
        string cacheKeyList = CustomerCacheKeys.ListKey;
        await _distributedCache
            .AddToListAsync<Customer, int>(cacheKeyList, customer,
                GetOptions());
    
        return customer;
    }
```


# Bug(🐞) Report

Dont forget to submit an issue if you face. we will try to resolve as soon as possible.

# Give a star (⭐)

**If you find this library useful to you, please don't forget to encouraging me to do such more stuffs by giving a star (⭐) to this repository. Thank you.**
