//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Cat_Taxonomia_Detalle
    {
        public Cat_Taxonomia_Detalle()
        {
            this.Cat_Taxonomia_Columna = new HashSet<Cat_Taxonomia_Columna>();
            this.Cat_Taxonomia_Detalle1 = new HashSet<Cat_Taxonomia_Detalle>();
            this.Cat_Taxonomia_Detalle11 = new HashSet<Cat_Taxonomia_Detalle>();
            this.Taxonomia_Detalle_Suma = new HashSet<Taxonomia_Detalle_Suma>();
            this.Taxonomia_Detalle_Suma1 = new HashSet<Taxonomia_Detalle_Suma>();
            this.Taxonomia_Reporte = new HashSet<Taxonomia_Reporte>();
            this.Cat_Modelo_Clase = new HashSet<Cat_Modelo_Clase>();
            this.Cat_Modelo_Clase1 = new HashSet<Cat_Modelo_Clase>();
        }
    
        public int Id_Taxonomia_Detalle { get; set; }
        public Nullable<int> Id_Origen_Elemento { get; set; }
        public Nullable<int> Id_Tipo_Formato { get; set; }
        public string Contenido { get; set; }
        public Nullable<int> Id_Tabla_Padre { get; set; }
        public Nullable<int> Id_Axis_Padre { get; set; }
        public string Formato { get; set; }
        public Nullable<int> Orden { get; set; }
        public string Descripcion { get; set; }
        public string Prefijo { get; set; }
        public string Nombre_esquema { get; set; }
        public string Referencia_Tiempo { get; set; }
        public Nullable<System.DateTime> Fecha_Registro { get; set; }
        public Nullable<int> Nivel_Sangria { get; set; }
        public Nullable<bool> Campo_Dinamico { get; set; }
    
        public virtual Cat_Origen_Elemento Cat_Origen_Elemento { get; set; }
        public virtual ICollection<Cat_Taxonomia_Columna> Cat_Taxonomia_Columna { get; set; }
        public virtual ICollection<Cat_Taxonomia_Detalle> Cat_Taxonomia_Detalle1 { get; set; }
        public virtual Cat_Taxonomia_Detalle Cat_Taxonomia_Detalle2 { get; set; }
        public virtual ICollection<Cat_Taxonomia_Detalle> Cat_Taxonomia_Detalle11 { get; set; }
        public virtual Cat_Taxonomia_Detalle Cat_Taxonomia_Detalle3 { get; set; }
        public virtual Cat_Tipo_Formato Cat_Tipo_Formato { get; set; }
        public virtual ICollection<Taxonomia_Detalle_Suma> Taxonomia_Detalle_Suma { get; set; }
        public virtual ICollection<Taxonomia_Detalle_Suma> Taxonomia_Detalle_Suma1 { get; set; }
        public virtual ICollection<Taxonomia_Reporte> Taxonomia_Reporte { get; set; }
        public virtual ICollection<Cat_Modelo_Clase> Cat_Modelo_Clase { get; set; }
        public virtual ICollection<Cat_Modelo_Clase> Cat_Modelo_Clase1 { get; set; }
    }
}
