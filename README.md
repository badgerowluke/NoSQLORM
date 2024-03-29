# NoSQLORM

[![example workflow](https://github.com/badgerowluke/NoSQLORM/actions/workflows/core.build.yml/badge.svg)[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=badgerowluke_NoSQLORM&metric=alert_status)](https://sonarcloud.io/dashboard?id=badgerowluke_NoSQLORM)

## What and why

The goal of this project is to provide a single testable surface for our projects to interact with persistance/storage/data stores/databases.

First question: Why not just use Entity Framework? Our answer, Entity Framework traditionally wants control over the database (code first), or the code (database first) during your development process.

Second question: Why not just use another ORM, say Dapper? We love Dapper and it does just about everything we want it to use, but where we run into issues is when we start to reach the upper bounds of Code Coverage (you know that 100% number).  Being static extension methods off your SqlClient means that if you leak any kind of logic into your repository layer you have to end your testing there.  Also non-relational data stores aren't covered currently by some of these tools.

Third question: You're wrapping up provider SDKs and APIs, why not just use them directly? For this, let's remember that ORMs need some information in order to make their magic happen.  Take for instance this POCO:

```csharp
public class River
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string RiverId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Srs { get; set; }
    public object[] Levels { get; set; }
    public object[] Flow { get; set; }
    public object[] RiverData { get; set; }
    public string State { get; set; }
    public string StateCode { get; set; }

}
```
Now, here is the same object, manipulated so that we can use Microsoft's, Azure Table SDK

```csharp
public class River : ITableEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string RiverId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Srs { get; set; }
    public object[] Levels { get; set; }
    public object[] Flow { get; set; }
    public object[] RiverData { get; set; }
    public string State { get; set; }
    public string StateCode { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string ETag { get; set; }

    public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
    {
        ...
    }

    public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
    {
        ...
    }

}
```

In our projects, we prefer not to maintain multiple versions of the same object (MV-VM). On fast moving projects, or large projects (with _many_ engineers/developers) we've seen drift, and that drift is an opportunity for error.  Yes this error/risk is mitigated by diligent Pull Request Reviews, but again: large, fast moving projects.  

Our goal with this is for you to keep your repository logic with your repository and your models someplace else entirely that can be shared by your front-end/mobile team and your API/backend team.  

## How

### dependency injection

So a couple of usage examples, below we're doing dependency injection on an Azure Function project.

1. we're creating a singleton of the account builder
2. we're also creating a singleton of an instance of `IAzureStorage` (in this case an `AzureTableBuilder`)
3. we create the repository type that we're going to use in this Functions Context 

```csharp

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddSingleton<ICloudStorageAccount>(new CloudStorageAccountBuilder(myConfig.storageConnectionString));
        builder.Services.AddSingleton<IAzureStorage, AzureTableBuilder>();

        ...

        builder.Services.AddSingleton<RiverRepositoryConfig>(sp => myConfig);
        builder.Services.AddSingleton<IRiverRepository, RiverRepository>();
    }
```
4. in our Service/Business Logic layer, we register the configuration and call through to our repository
    * this lets us go ahead and mock out the behaviors for unit testing as necessary
        * this also allows us to mix data sources simply 
        * in this example, we're mixing between Azure Table Storage and Azure Search (REST endpoint)

     

```csharp
    public async Task<IEnumerable<River>> GetRivers(string partName)
    {
        repo.Register(_config);
        if(string.IsNullOrEmpty(partName)){
            return repo.GetRivers();
        } else {
            if(!string.IsNullOrEmpty(GetStateCode(partName)))
            {
                return await repo.GetRiversByState(partName);
            }
            return await repo.GetRiversAsync(partName);
        }
    }

    public async Task<IEnumerable<River>> GetRiversByState(string stateCode)
    {
        ... 
        
        folders.CollectionName = _riverTable;

        var entities = await folders.GetAsync<RiverEntity>(r => r.PartitionKey.Equals(stateCode));
        var outList = new List<River>();
        foreach(var entity in entities)
        {
            var river = entity.ToRiver();
            outList.Add(river);
        }

        return outList;
    }   


```
5. a caveat with respect to utilizing a lamba expression to filter your query of the Azure Table. You should avoid utilizing the string convienence methods `isNullOrEmpty` or `isNullOrWhitespace` as this iteration of the ORM struggles with transposing that to the appropriate value in the Table Query Syntax

```csharp
    [Fact]
    public void EncodeNullOrEmptyAndEquals()
    {
        var query = _builder.BuildQueryFilter<River>(r => !r.StateCode.Equals("") 
                                                        && r.StateCode == "WV");
        query.Should().BeEquivalentTo("StateCode ne '' and StateCode eq 'WV'");
    }

    [Fact]
    public void EncodeNullOrEmptyWithoutMethodCall()
    {
        var query = _builder.BuildQueryFilter<River>(r => r.StateCode != ""  
                                                        && r.StateCode == "WV");
        query.Should().BeEquivalentTo("StateCode ne '' and StateCode eq 'WV'");            
    }

    [Fact]
    public void EncodeNullOrEmptyUsingStringNullOrEmpty() 
    {
        var query = _builder.BuildQueryFilter<River>(r => (r.StateCode != null || r.StateCode != "")
                                                        && r.StateCode == "WV");
        query.Should().BeEquivalentTo("StateCode ne '' or StateCode ne '' and StateCode eq 'WV'");   
    }

```



