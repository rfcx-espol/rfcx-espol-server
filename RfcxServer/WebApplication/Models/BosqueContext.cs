using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class BosqueContext : DbContext
    {
        public BosqueContext(DbContextOptions<BosqueContext> options) : base(options)
        {}
        
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<AlertaConfiguracion> AlertasConfiguracion { get; set; }
        public DbSet<Audio> Audios { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; } 
        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<Etiqueta> Etiquetas { get; set; }


        /** 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=bosque.db");
        }
        */

    }
}