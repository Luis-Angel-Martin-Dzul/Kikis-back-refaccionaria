using AutoMapper;
using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceSale : IServiceSale {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceEMail _emailService;
        public ServiceSale(IUnitOfWork unitOfWork, IServiceEMail emailService) {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        /*
         *  GET
         */
        public async Task<IEnumerable<SaleRES>> GetSales(SaleFilter filter) {

            //query
            var query = _unitOfWork.Sale
                .GetQuery()
                .Include(sale => sale.SellerNavigation)
                .Include(sale => sale.TbSaleDetails)
                    .ThenInclude(sale => sale.ProductNavigation)
                .Include(sale => sale.TbInvoices)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var sales = await query.Select(sale => new SaleRES {
                Id = sale.Id,
                Seller = new GenericCatalog {
                    Id = sale.SellerNavigation.Id,
                    Name = $"{sale.SellerNavigation.FirstName} {sale.SellerNavigation.LastName}"
                },
                SubTotal = sale.SubTotal,
                Iva = sale.IVA,
                Total = sale.Total,
                Pay = sale.Pay,
                CreateDate = sale.CreateDate,
                SaleDetails = sale.TbSaleDetails.Select(sale => new SaleDetail {
                    Id = sale.Id,
                    Product = sale.Product,
                    Name = sale.ProductNavigation.Name,
                    Price = sale.Price,
                    PriceUnit = sale.PriceUnit,
                    Quantity = sale.Quantity,
                    Total = sale.Total,
                }).ToList(),
                Invoice = sale.TbInvoices.Count() == 0 ? 0 : sale.TbInvoices.FirstOrDefault().Id
            }).ToListAsync();

            return sales;
        }

        public async Task<IEnumerable<InvoiceRES>> GetInvoices(InvoiceFilter filter) {

            //query
            var query = _unitOfWork.Invoice
                .GetQuery()
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var invoices = await query.Select(invoice => new InvoiceRES {

                Id = invoice.Id,
                Sale = invoice.Sale,
                Name = invoice.Name,
                RFC = invoice.RFC,
                CodePostal = invoice.CodePostal,
                Contact = invoice.Contact,
                TaxRegime = invoice.TaxRegime,
                TaxRegimeName = invoice.TaxRegimeName,
                UseCFDI = invoice.UseCFDI,
                UseCFDIName = invoice.UseCFDIName,
                CreateDate = invoice.CreateDate,
            }).ToListAsync();

            return invoices;
        }


        /*
         *  POST
         */
        public async Task<bool> PostSales(SaleREQ request) {


            try {
                await _unitOfWork.BeginTransactionAsync();

                //add sale
                var sale = new TbSale {
                    Seller = request.Seller.Id,
                    SubTotal = request.SubTotal,
                    IVA = request.Iva,
                    Total = request.Total,
                    Pay = request.Pay,
                    CreateDate = request.CreateDate,
                };
                _unitOfWork.Sale.Add(sale);
                await _unitOfWork.SaveChangeAsync();

                var saleDatails = request.SaleDetails.Select(x => new TbSaleDetail {
                    Sale = sale.Id,
                    Product = x.Product,
                    Price = x.Price,
                    PriceUnit = x.PriceUnit,
                    Quantity = x.Quantity,
                    Total = x.Total,
                });
                _unitOfWork.SaleDetail.AddRange(saleDatails);
                await _unitOfWork.SaveChangeAsync();

                //update Product
                var ProductSends = saleDatails
                    .GroupBy(s => s.Product)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Quantity));
                var ProductIds = ProductSends.Keys.ToList();

                var Products = await _unitOfWork.Product
                    .GetQuery()
                    .Where(x => ProductIds.Contains(x.Id))
                    .ToListAsync();

                foreach(var t in Products) {
                    if(ProductSends.TryGetValue(t.Id, out var quantitySend)) {
                        t.Quantity -= quantitySend;
                    }
                }
                _unitOfWork.Product.UpdateRange(Products);
                await _unitOfWork.SaveChangeAsync();


                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar guardar la compra\n{ex.Message}");
            }
        }
        public async Task<int> PostInvoice(InvoiceREQ request) {

            try {

                //add sale
                var invoice = new TbInvoice {
                    Sale = request.Sale,
                    Name = request.Name,
                    RFC = request.RFC,
                    TaxRegime = request.TaxRegime,
                    TaxRegimeName = request.TaxRegimeName,
                    CodePostal = request.CodePostal,
                    UseCFDI = request.UseCFDI,
                    UseCFDIName = request.UseCFDIName,
                    Contact = request.Contact,
                    CreateDate = request.CreateDate,
                };
                _unitOfWork.Invoice.Add(invoice);
                await _unitOfWork.SaveChangeAsync();

                var sale = await GetSales(new SaleFilter { Id = request.Sale });

                _emailService.SendCFDI(invoice, sale.FirstOrDefault());

                return invoice.Id;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar guardar factura\n{ex.Message}");
            }
        }
        public async Task<bool> PostTryInvoice(InvoiceTryREQ request) {

            try {

                //invoice
                var invoices = await GetInvoices(new InvoiceFilter { Id = request.Invoice });
                if(invoices.Count() <= 0)
                    throw new BusinessException("No se encontro factura");

                var invoice = invoices.FirstOrDefault();
                var invoiceMap = new TbInvoice {

                    Id = invoice.Id,
                    Sale = invoice.Sale,
                    Name = invoice.Name,
                    RFC = invoice.RFC,
                    CodePostal = invoice.CodePostal,
                    Contact = invoice.Contact,
                    CreateDate = invoice.CreateDate,
                    TaxRegime = invoice.TaxRegime,
                    TaxRegimeName = invoice.TaxRegimeName,
                    UseCFDI = invoice.UseCFDI,
                    UseCFDIName = invoice.UseCFDIName
                };

                //sale
                var sale = await GetSales(new SaleFilter { Id = request.Sale });
                if(sale == null)
                    throw new BusinessException("No se encontro venta");

                _emailService.SendCFDI(invoiceMap, sale.FirstOrDefault());

                return true;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar timbrar factura\n{ex.Message}");
            }
        }
        public async Task<bool> PostQuote(SaleREQ request) {
            try {

                return _emailService.SendQuote(request);

            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar enviar cotizacion\n{ex.Message}");
            }
        }

        /*
         *  PUT
         */
    }
}
