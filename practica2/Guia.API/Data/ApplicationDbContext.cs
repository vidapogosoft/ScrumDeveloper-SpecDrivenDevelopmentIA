using Microsoft.EntityFrameworkCore;
using Guia.API.Models;

namespace Guia.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Aquí le decimos qué clases se vuelven tablas en SQL
        // Tablas principales del sistema
        public virtual DbSet<Tema> Temas { get; set; }
        public virtual DbSet<Significado> Significados { get; set; }

        // Nuevas tablas de usuario
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<PersonaDetalle> PersonasDetalles { get; set; }
        public virtual DbSet<PersonaNumerologia> PersonasNumerologia { get; set; }
        public virtual DbSet<FraseGratitud> FrasesGratitud { get; set; }
        public virtual DbSet<RetoSemanal> RetosSemanales { get; set; }
        public virtual DbSet<Bitacora> Bitacoras { get; set; }
        public virtual DbSet<SignoZodiacal> SignosZodiacales { get; set; }
        public virtual DbSet<FaseLunar> FasesLunares { get; set; }
        public virtual DbSet<ElementoAstro> ElementosAstro { get; set; }
        public virtual DbSet<Arcano> Arcanos { get; set; }
        public virtual DbSet<ArbolVida> ArbolesVida { get; set; }
        public virtual DbSet<Arquetipo> Arquetipos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos que PersonaId sea único para mantener la relación 1 a 1
            modelBuilder.Entity<PersonaDetalle>()
                .HasIndex(d => d.PersonaId)
                .IsUnique();

            modelBuilder.Entity<PersonaNumerologia>()
                .HasIndex(n => n.PersonaId)
                .IsUnique();
        }
    }
}
