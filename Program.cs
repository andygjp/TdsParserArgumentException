#:package Microsoft.EntityFrameworkCore.SqlServer@11.0.0-rc.1.26425.128
#:package Microsoft.Data.SqlClient@7.1.0
#:property PublishAot=false

using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, world!");

var pword = "your password";
var connStr = $"Server=localhost;Database=TdsParserArgumentException;User Id=SA;Password={pword};Encrypt=False;";
var options = new DbContextOptionsBuilder<Context>().UseSqlServer(connStr).Options;
var context = new Context(options);
context.Database.EnsureCreated();

context.Data.Add(new Data());

context.SaveChanges();

class Data
{
    public int Id { get; init; }
    public decimal Factor { get; init; }
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
    }
}