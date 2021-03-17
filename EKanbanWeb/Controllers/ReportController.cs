using AspNetCore.Reporting;
using EKanbanWeb.Data;
using EKanbanWeb.Helpers;
using EKanbanWeb.Models;
using EKanbanWeb.Models.View;
using EKanbanWeb.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing.QrCode;

namespace EKanbanWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private EKanbanWebDBContext DbContext;
        private SqlHelper SqlHelp { get; set; }

        public ReportController(IWebHostEnvironment webHostEnvironment, EKanbanWebDBContext context, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            DbContext = context;
            SqlHelp = new SqlHelper(configuration, hostEnvironment);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print()
        {
            var dt = new DataTable();


            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rp1", "Welcome");

            LocalReport localReport = new LocalReport(path);

            string sp = "select * from SyMenu";
            List<SqlParameter> param = new List<SqlParameter>();
            dt = SqlHelp.ExecQuery(sp, param);
            localReport.AddDataSource("dsMenu", dt);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }

        public IActionResult Print2()
        {
            var dt = new DataTable();

            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\eKanbanReport2.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            LocalReport localReport = new LocalReport(path);

            //QRCoder.QRCodeGenerator qrCodeGenerator = new QRCoder.QRCodeGenerator();
            var barcodeWriterPixelData = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    Margin = 0
                }
            };

            string sql = "select * from v_KanbanRequestMasterDetail";
            dt = SqlHelp.ExecQueryCommand(sql);

            //add image column
            DataColumn column;
            column = new DataColumn();
            column.Caption = "Image";
            column.DataType = Type.GetType("System.Byte[]");
            column.ColumnName = "QRImage";
            column.ReadOnly = false;
            column.Unique = false;
            dt.Columns.Add(column);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //dt.Rows[i].ItemArray[2] = "";
                string qrCodeVal = dt.Rows[i][8].ToString();
                //QRCoder.QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeVal, QRCoder.QRCodeGenerator.ECCLevel.Q);
                //QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
                //Bitmap bmp = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                //dt.Rows[i].ItemArray[18] = "data:image/bmp;base64," + Convert.ToBase64String(BitmapToBytes(bmp));
                //dt.Rows[i].ItemArray[18] = qrCode.GetGraphic(20);

                var pixelData = barcodeWriterPixelData.Write(qrCodeVal);
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var bitmapData = bitmap.LockBits(
                            new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format32bppRgb);
                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                        bitmap.Save(memoryStream, ImageFormat.Bmp);
                        dt.Rows[i].ItemArray[18] = memoryStream.ToArray();
                    }
                }
                dt.AcceptChanges();
            }
            localReport.AddDataSource("dsKanbanRequestMasterDetail", dt);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public IActionResult Print3(int kanbanReqId)
        {
            var dt = new DataTable();
            string sp = "sp_KanbanReport";
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@kanbanReqId", 1));
            dt = SqlHelp.ExecQuery(sp, param);
            List<vKanbanRequest> viewlist = SqlHelp.ConvertToList<vKanbanRequest>(dt);

            vKanbanRequest vKanban = new vKanbanRequest();
            EKanbanReport model = new EKanbanReport();
            if (viewlist != null)
            {
                vKanban = viewlist[0];
                model.vKanbanRequest = vKanban;

                //item list
                sp = "sp_KanbanReportItem";
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@kanbanReqId", 1));
                dt = SqlHelp.ExecQuery(sp, param);
                List<EKanbanReportItem> itemlist = SqlHelp.ConvertToList<EKanbanReportItem>(dt);
                model.ItemList = itemlist;
            }

            return View(model);
        }
    }
}
