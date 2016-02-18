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
    
    public partial class Cat_Validacion_Contexto
    {
        public Cat_Validacion_Contexto()
        {
            this.Periodoes = new HashSet<Periodo>();
            this.Cat_Taxonomia_Columna = new HashSet<Cat_Taxonomia_Columna>();
        }
    
        public int Id_Validacion_Contexto { get; set; }
        public string Documento { get; set; }
        public string Descripcion_Perido { get; set; }
        public Nullable<int> X { get; set; }
        public Nullable<int> Y { get; set; }
        public Nullable<System.DateTime> Fecha_Registro { get; set; }
        public string Usuario_Registro { get; set; }
        public Nullable<int> Periodo_No_Requerido { get; set; }
    
        public virtual ICollection<Periodo> Periodoes { get; set; }
        public virtual ICollection<Cat_Taxonomia_Columna> Cat_Taxonomia_Columna { get; set; }
    }
}