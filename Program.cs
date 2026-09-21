#:package Microsoft.EntityFrameworkCore.SqlServer@11.0.0-rc.1.26425.128
#:property PublishAot=false

using Microsoft.EntityFrameworkCore;
using Diagnostics = Microsoft.EntityFrameworkCore.Diagnostics;

Console.WriteLine("Hello, world!");

var connStr = "";
var options = new DbContextOptionsBuilder<Context>()
    // .UseSqlServer(connStr)
    .UseAzureSql(connStr, options =>
    {
        options.EnableRetryOnFailure();
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        // The default timeout is 30s
        options.CommandTimeout(60);
        options.UseCompatibilityLevel(150);
    })
    .ConfigureWarnings(warnings =>
    {
        warnings.Throw(Diagnostics.CoreEventId.OldModelVersionWarning);
        warnings.Throw(Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning);
    })
    .EnableThreadSafetyChecks()
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
var context = new Context(options);
context.Database.EnsureCreated();

context.Data.Add(new Data());

context.SaveChanges();

class Data
{
    public int Id { get; init; }
    public decimal Factor { get; init; }
    public decimal Number { get; init; }
    public decimal Product { get; init; }
}

class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<Data> Data => Set<Data>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Data>();
        entity.ToTable("Data");
        entity.HasKey(x=> x.Id);
        entity.Property(x => x.Factor).HasPrecision(3,3).IsRequired();
        entity.Property(x => x.Number).HasPrecision(12, 5).IsRequired().HasDefaultValueSql("0.00000");
        entity.Property(x => x.Product).HasPrecision(18, 2).IsRequired().HasComputedColumnSql("CAST(Number * (1 + Factor) AS DECIMAL(18,2))");
    }
}