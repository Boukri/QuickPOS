using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickPOS.Models.Entities;

namespace QuickPOS.Data.Configurations;
internal class IdentityConfig : IEntityTypeConfiguration<AppIdentityUser>
{
    public void Configure(EntityTypeBuilder<AppIdentityUser> builder)
    {
        builder.ToTable("IdentityUsers");
    }
}

internal class IdentityRoleConfig : IEntityTypeConfiguration<AppIdentityRole>
{
    public void Configure(EntityTypeBuilder<AppIdentityRole> builder)
    {
        builder.ToTable("IdentityRoles");
    }
}

internal class UserPermissionConfig : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");
        builder.HasKey(up => up.Id);
        builder.HasOne(up => up.User)
         .WithMany(u => u.Permissions)
         .HasForeignKey(up => up.UserId)
         .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(up => new { up.UserId, up.PermissionKey }).IsUnique();
    }
}