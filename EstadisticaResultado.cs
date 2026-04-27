using System;
using System.Collections.Generic;

namespace ProjecteCobolDavid
{
    public class EstadisticaResultado
    {
        public decimal MediaGastos { get; set; }
        public List<Top3Gasto> Top3Gastos { get; set; }

        public EstadisticaResultado()
        {
            Top3Gastos = new List<Top3Gasto>();
        }
    }

    public class Top3Gasto
    {
        public string Tipus { get; set; }
        public decimal TotalGasto { get; set; }
    }
}
