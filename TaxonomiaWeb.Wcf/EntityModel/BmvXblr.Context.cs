﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaxonomiaWeb.Wcf.EntityModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BmvXblrEntities : DbContext
    {
        public BmvXblrEntities()
            : base("name=BmvXblrEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Cat_Ano> Cat_Ano { get; set; }
        public DbSet<Cat_Origen_Elemento> Cat_Origen_Elemento { get; set; }
        public DbSet<Cat_Tipo_Formato> Cat_Tipo_Formato { get; set; }
        public DbSet<Cat_Trimestre> Cat_Trimestre { get; set; }
        public DbSet<Cat_Validacion_Contexto> Cat_Validacion_Contexto { get; set; }
        public DbSet<Contexto> Contextoes { get; set; }
        public DbSet<Periodo> Periodoes { get; set; }
        public DbSet<Taxonomia_Detalle_Suma> Taxonomia_Detalle_Suma { get; set; }
        public DbSet<Taxonomia_Reporte> Taxonomia_Reporte { get; set; }
        public DbSet<Taxonomia_Reporte_Detalle> Taxonomia_Reporte_Detalle { get; set; }
        public DbSet<Cat_Taxonomia_Columna> Cat_Taxonomia_Columna { get; set; }
        public DbSet<Cat_Contenido> Cat_Contenido { get; set; }
        public DbSet<Cat_Taxonomia_Detalle> Cat_Taxonomia_Detalle { get; set; }
        public DbSet<Cat_Validacion_Trimestre> Cat_Validacion_Trimestre { get; set; }
        public DbSet<Periodo_Sin_Presentar> Periodo_Sin_Presentar { get; set; }
        public DbSet<Cat_Modelo_Clase> Cat_Modelo_Clase { get; set; }
    }
}
