using Microsoft.EntityFrameworkCore;

namespace ExcelExportetWpf.Models
{
	public class BakuBusContext : DbContext
	{
		public DbSet<BakuBus>? BakuBuses { get; set; }

		private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BakuBusDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BakuBus>().ToTable("BakuBus");


		}
	}
}
