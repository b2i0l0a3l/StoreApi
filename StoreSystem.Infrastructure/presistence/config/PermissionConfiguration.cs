using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreSystem.Core.Constants;
using StoreSystem.Core.Entities;

namespace StoreSystem.Infrastructure.Persistence.Config
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Code).IsUnique();

            var permissions = PermissionCodes.GetAllPermissions().Select((code, index) => new Permission
            {
                Id = index + 1,
                Code = code,
                Name = code.Replace(".", " ")
           });

            builder.HasData(permissions);
        }
    }
}
