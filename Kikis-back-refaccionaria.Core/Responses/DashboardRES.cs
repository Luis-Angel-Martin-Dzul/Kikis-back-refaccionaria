namespace Kikis_back_refaccionaria.Core.Responses {
    public class DashboardRES {
        public SaleD2 Sale {
            get; set;
        }
        public List<VentasDiaRES> VentasPorDia { get; set; } = new List<VentasDiaRES>();

        // Lista completa de vendedores
        public List<VendedorRES> Vendedores { get; set; } = new List<VendedorRES>();

        // Top 3 vendedores
        public List<VendedorRES> TopVendedores { get; set; } = new List<VendedorRES>();
    }


    // Métricas generales de ventas
    public class SaleD2 {
        public int CantidadVentas {
            get; set;
        }
        public decimal Iva {
            get; set;
        }
        public decimal Subtotal {
            get; set;
        }
        public decimal Total {
            get; set;
        }
        public decimal Cambio {
            get; set;
        }
        public decimal TicketPromedio {
            get; set;
        }
        public decimal PagoPromedio {
            get; set;
        }
    }
    public class VentasDiaRES {
        public int Dia {
            get; set;
        }
        public int NumeroVentas {
            get; set;
        }

        public List<SaleRES> products {
            get; set;
        }
    }

    public class VendedorRES {
        public int SellerId {
            get; set;
        }          // ID del vendedor
        public string SellerName {
            get; set;
        }     // Nombre del vendedor
        public int NumeroVentas {
            get; set;
        }      // Número total de ventas realizadas
        public decimal TotalVentas {
            get; set;
        }   // Total vendido por el vendedor
        public decimal TicketPromedio {
            get; set;
        } // Promedio de ticket de venta
    }

}





