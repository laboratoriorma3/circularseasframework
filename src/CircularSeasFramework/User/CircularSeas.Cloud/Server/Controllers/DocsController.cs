﻿using System;
using System.IO;
using System.Threading.Tasks;
using CircularSeas.Adapters;
using CircularSeas.Infrastructure.DB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocsController : Controller
    {
        private readonly IDbService dbService;
        private readonly IGenPDF pdf;
        private readonly IWebHostEnvironment env;

        public DocsController(IDbService dbService, IGenPDF pdf, IWebHostEnvironment env)
        {
            this.dbService = dbService;
            this.pdf = pdf;
            this.env = env;
        }

        /// <summary>
        /// Download a PDF with the information of a filament spool (GUID, Date and QR code)
        /// </summary>
        /// <param name="orderId">GUID of an order</param>
        /// <returns></returns>
        [HttpGet("pdf/{orderId}")]
        public async Task<IActionResult> GetPdf([FromRoute] Guid orderId)
        {
            try
            {
                var order = await dbService.GetOrder(orderId);

                var bytes = pdf.CreateSpoolPDF(order);

                return File(bytes, "application/pdf",
                    $"{order.CreationDate.ToString("yyMMdd")}_{order.Node.Name}_{order.Material.Name}.pdf");

            }
            catch (Exception ex)
            {
                throw;
                BadRequest(ex.Message);

            }
        }
    }

}