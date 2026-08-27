using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PetDesktop.Back.Repositories.Common;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Models.Pet> Pet {get; set; } = null!;
    public DbSet<Models.SpriteSheet> SpriteSheet { get; set; } = null!;
    
    public void EnsureCreated()
    {
        if (!Directory.Exists(Config.Config.DataBaseFolder))
        {
            Directory.CreateDirectory(Config.Config.DataBaseFolder);
        }
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. CONFIGURACIÓN EXPLÍCITA DE PET
        modelBuilder.Entity<Models.Pet>(builder =>
        {
            builder.ToTable("Pet");
            
            builder.Ignore(p => p.ActualAnimation);
            
            // Clave Primaria (Obligatoria por defecto)
            builder.HasKey(p => p.Name);
            builder.Property(p => p.Name)
                .IsRequired();

            // Mensaje: Permite nulos, valor por defecto "Drink Water!"
            builder.Property(p => p.Message)
                .IsRequired(false)
                .HasDefaultValue("Drink Water!");

            // Intervalo de tiempo: Requerido, convertido a string ("c") para SQLite
            builder.Property(p => p.TimeMessageInterval)
                .IsRequired()
                .HasConversion(
                    v => v.ToString("c"),   
                    v => TimeSpan.Parse(v)
                );
        });

        // 2. CONFIGURACIÓN EXPLÍCITA DE SPRITESHEET
        modelBuilder.Entity<Models.SpriteSheet>(builder =>
        {
            builder.ToTable("SpriteSheet");
            
            // Clave Primaria
            builder.HasKey(s => new { s.AssociatedPet, s.Name });
            builder.Property(s => s.Name)
                .IsRequired();

            builder.Property(s => s.Route)
                .IsRequired();

            builder.Property(s => s.FrameWidth)
                .IsRequired();

            builder.Property(s => s.FrameHeight)
                .IsRequired();

            // Clave Foránea: Requerida porque todo SpriteSheet debe pertenecer a una mascota
            builder.Property(s => s.AssociatedPet)
                .IsRequired();

            // Relación inversa: Mapea la FK físicac con ON DELETE CASCADE
            builder.HasOne<Models.Pet>()                             
                .WithMany()                                   
                .HasForeignKey(s => s.AssociatedPet)          
                .HasPrincipalKey(p => p.Name)                 
                .OnDelete(DeleteBehavior.Cascade);            // Si borras la Pet, se borran sus SpriteSheets
        });
    }
}