using System;
using System.Reflection;
using Consulta_medica.Dto.Request;
using Consulta_medica.Dto.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Consulta_medica.Models
{
    public partial class consulta_medicaContext : DbContext
    {
        public consulta_medicaContext()
        {
        }

        public consulta_medicaContext(DbContextOptions<consulta_medicaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrador> Administrador { get; set; }
        public virtual DbSet<Citas> Citas { get; set; }
        public virtual DbSet<Especialidad> Especialidad { get; set; }
        public virtual DbSet<Horario> Horario { get; set; }
        public virtual DbSet<Medico> Medico { get; set; }
        public virtual DbSet<Paciente> Paciente { get; set; }
        public virtual DbSet<Tipousuario> Tipousuario { get; set; }
        public virtual DbSet<HistorialMedico> HistorialMedico { get; set; }
        public virtual DbSet<CitasMedicasReporteResponse> citasMedicasReporteResponse { get; set; }
        public virtual DbSet<Permisos> Permisos { get; set; }
        public virtual DbSet<Pagos> Pagos { get; set; }
        //sp
        public virtual DbSet<ConfiguracionesResponse> ConfiguracionesResponses { get; set; }
        public virtual DbSet<CitasQueryDto> CitasQueryDtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-NHMNTAF\\SQLEXPRESS;Database=consulta_medica;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CitasQueryDto>(e => { e.HasNoKey(); });

            modelBuilder.Entity<ConfiguracionesResponse>(entity => 
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Permisos>(entity =>
            {
                entity.ToTable("permisos");
            });

            modelBuilder.Entity<CitasMedicasReporteResponse>(e =>
            {
                e.HasNoKey();
            });

            modelBuilder.Entity<HistorialMedico>(entity =>
            {
                entity.ToTable("historialMedico");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Codes)
                    .IsRequired()
                    .HasColumnName("codes")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Codmed)
                    .IsRequired()
                    .HasColumnName("codmed")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Diagnostico)
                    .IsRequired()
                    .HasColumnName("diagnostico")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Dnip).HasColumnName("dnip");

                entity.Property(e => e.Fecct)
                    .HasColumnName("fecct")
                    .HasColumnType("datetime");

                entity.Property(e => e.Receta)
                    .IsRequired()
                    .HasColumnName("receta")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.Codad)
                    .HasName("PK__administ__9215B00122A52D99");

                entity.ToTable("administrador");

                entity.Property(e => e.Codad)
                    .HasColumnName("codad")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Dni).HasColumnName("dni");

                entity.Property(e => e.Iptip)
                    .HasColumnName("iptip")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Nac)
                    .HasColumnName("nac")
                    .HasColumnType("date");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Pswd)
                    .HasColumnName("pswd")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Sexo)
                    .IsRequired()
                    .HasColumnName("sexo")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Citas>(entity =>
            {
                entity.ToTable("citas");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Codmed)
                    .HasColumnName("codmed")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Dnip).HasColumnName("dnip");

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Feccit)
                    .HasColumnName("feccit")
                    .HasColumnType("date");

                entity.Property(e => e.Hora).HasColumnName("hora");
                entity.Property(e => e.Codes).HasColumnName("codes");
            });

            modelBuilder.Entity<Especialidad>(entity =>
            {
                entity.HasKey(e => e.Codes)
                    .HasName("PK__especial__920DF629BD59231C");

                entity.ToTable("especialidad");

                entity.Property(e => e.Codes)
                    .HasColumnName("codes")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Costo)
                    .HasColumnName("costo")
                    .HasColumnType("decimal(6, 1)");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Horario>(entity =>
            {
                entity.HasKey(e => e.Idhor)
                    .HasName("PK__horario__07186FE173CDB99E");

                entity.ToTable("horario");

                entity.Property(e => e.Idhor)
                    .HasColumnName("idhor")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Codes)
                    .HasColumnName("codes")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Dias)
                    .HasColumnName("dias")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Hfin).HasColumnName("hfin");

                entity.Property(e => e.Hinicio).HasColumnName("hinicio");
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasKey(e => e.Codmed)
                    .HasName("PK__medico__5DDE468147EEF0FD");

                entity.ToTable("medico");

                entity.Property(e => e.Codmed)
                    .HasColumnName("codmed")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Asis)
                    .HasColumnName("asis")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Codes)
                    .HasColumnName("codes")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Correo)
                    .HasColumnName("correo")
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.Dni).HasColumnName("dni");

                entity.Property(e => e.Idhor)
                    .HasColumnName("idhor")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Idtip)
                    .HasColumnName("idtip")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Nac)
                    .HasColumnName("nac")
                    .HasColumnType("date");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Pswd)
                    .HasColumnName("pswd")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Sexo)
                    .HasColumnName("sexo")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Dnip)
                    .HasName("PK__paciente__2D55C7DFC84041A9");

                entity.ToTable("paciente");

                entity.Property(e => e.Dnip)
                    .HasColumnName("dnip")
                    .ValueGeneratedNever();

                entity.Property(e => e.Idtip)
                    .HasColumnName("idtip")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Nomp)
                    .HasColumnName("nomp")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Numero).HasColumnName("numero");
            });

            modelBuilder.Entity<Tipousuario>(entity =>
            {
                entity.HasKey(e => e.Idtip)
                    .HasName("PK__tipousua__2A412891CFE99A53");

                entity.ToTable("tipousuario");

                entity.Property(e => e.Idtip)
                    .HasColumnName("idtip")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Nomtip)
                    .HasColumnName("nomtip")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
