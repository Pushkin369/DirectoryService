using DirectoryService.Domain.Aggregates.Departments;
using DirectoryService.Domain.Aggregates.Departments.Entities;
using DirectoryService.Domain.Aggregates.Locations;
using DirectoryService.Domain.Aggregates.Positions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();
        public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();
    }
 }