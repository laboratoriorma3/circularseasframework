﻿using System;
using System.IO;
using CircularSeas.Adapters;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Newtonsoft.Json;

namespace CircularSeas.Infrastructure.GenPDF
{
    public class PdfGenerator : IGenPDF
    {
        public PdfGenerator()
        {

        }

        public virtual void CreatePdf(string dest)
        {
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(dest);

            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(writer);
            // Initialize document
            Document document = new Document(pdf);
            //Add paragraph to the document
            document.Add(new Paragraph("Hello World!"));


            ImageData imageData = ImageDataFactory.Create(QrGenerator.GenerateByteArray("Codigo QR"));


            document.Add(new Image(imageData));
            //Close document
            document.Close();
        }

        public byte[] CreateSpoolPDF(Models.Order order)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);

                Table header = new Table(2);
                header.AddCell("Identifier");
                header.AddCell(order.Id.ToString());
                header.AddCell("Client");
                header.AddCell(order.Node.Name);
                header.AddCell("Material");
                header.AddCell(order.Material.Name);

                var qrInfo = new CircularSeas.Models.DTO.QrDTO()
                {
                    OrderId = order.Id,
                    MaterialId = order.Material.Id,
                    MaterialName = order.Material.Name,
                };
                string qrJson = JsonConvert.SerializeObject(qrInfo);
                //Add paragraph to the document
                document.Add(header);
                ImageData imageData = ImageDataFactory.Create(QrGenerator.GenerateByteArray(qrJson));
                var qr = new Image(imageData).SetWidth(300).SetHeight(300);

                document.Add(qr);
                //Close document
                document.Close();
                return ms.ToArray();
            }
        }
    }
}
