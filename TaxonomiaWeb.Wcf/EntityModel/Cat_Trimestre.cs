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
    
    public partial class Cat_Trimestre
    {
        public Cat_Trimestre()
        {
            this.Periodoes = new HashSet<Periodo>();
            this.Cat_Validacion_Trimestre = new HashSet<Cat_Validacion_Trimestre>();
            this.Periodo_Sin_Presentar = new HashSet<Periodo_Sin_Presentar>();
            this.Taxonomia_Reporte_Detalle = new HashSet<Taxonomia_Reporte_Detalle>();
        }
    
        public int Id_Trimestre { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> Trimestre { get; set; }
    
        public virtual ICollection<Periodo> Periodoes { get; set; }
        public virtual ICollection<Cat_Validacion_Trimestre> Cat_Validacion_Trimestre { get; set; }
        public virtual ICollection<Periodo_Sin_Presentar> Periodo_Sin_Presentar { get; set; }
        public virtual ICollection<Taxonomia_Reporte_Detalle> Taxonomia_Reporte_Detalle { get; set; }
    }
}
