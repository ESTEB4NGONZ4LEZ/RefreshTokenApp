
using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.Property(x => x.Username)
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(x => x.Email)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Password)
               .IsRequired();

        builder.HasIndex(x => new {x.Username, x.Email})
               .IsUnique();

        builder.HasMany(x => x.Rols)
               .WithMany(x => x.Users)
               .UsingEntity<UserRol>(
                    x => x.HasOne(a => a.Rol)
                          .WithMany(e => e.UserRols)
                          .HasForeignKey(i => i.IdRol),

                    x => x.HasOne(a => a.User)
                          .WithMany(e => e.UserRols)
                          .HasForeignKey(i => i.IdUser),
                    x => 
                    {
                        x.HasKey(a => new {a.IdUser, a.IdRol});
                    });
    }
}
