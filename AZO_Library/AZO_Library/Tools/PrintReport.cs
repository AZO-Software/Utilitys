using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZO_Library.Tools
{
    /// <summary>
    /// Clase encargada de imprimir un reporte con la informacion especificada
    /// </summary>
    public class PrintReport
    {
        #region Globals

        private static int m_currentPageIndex;
        private static IList<Stream> m_streams;

        #endregion

        #region Public Methods

        /// <summary>
        /// Mandar imprimir el reporte a la impresora que este por default, agregando el valor recibido como parametro
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="table"></param>
        /// <param name="reportName"></param>
        public static void Print(string parameterName, string parameterValue, System.Data.DataTable table, string reportName)
        {
            try
            {
                LocalReport report = new LocalReport();
                report.ReportEmbeddedResource = reportName;
                report.DataSources.Add(new ReportDataSource("DataSet1", table));
                ReportParameter parameterFolio = new ReportParameter(parameterName, parameterValue);
                report.SetParameters(parameterFolio);
                Export(report);
                Print();
            }
            catch(Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "PrintSalesNotes(string, string, DataTable, string)", ex);
            }
        }

        /// <summary>
        /// Manda imprimir el reporte, tomando en cuenta la tabla
        /// </summary>
        /// <param name="table"></param>
        /// <param name="reportName"></param>
        public static void Print(System.Data.DataTable table, string reportName)
        {
            try
            {
                LocalReport report = new LocalReport();
                report.ReportEmbeddedResource = reportName;
                report.DataSources.Add(new ReportDataSource("DataSet1", table));
                Export(report);
                Print();
            }
            catch (Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "PrintSalesNotes(DataTable, string)", ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Export the given report as an EMF (Enhanced Metafile) file.
        /// </summary>
        /// <param name="report"></param>
        private static void Export(LocalReport report)
        {
            try
            {
                string deviceInfo =
                @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>8.5in</PageWidth>
                    <PageHeight>11in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.25in</MarginLeft>
                    <MarginRight>0.25in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream, out warnings);
                foreach (Stream stream in m_streams)
                {
                    stream.Position = 0;
                }
            }
            catch (Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "Export(LocalReport)", ex);
            }
        }

        /// <summary>
        /// Routine to provide to the report renderer, in order to
        ///    save an image for each page of the report.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileNameExtension"></param>
        /// <param name="encoding"></param>
        /// <param name="mimeType"></param>
        /// <param name="willSeek"></param>
        /// <returns></returns>
        private static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            try
            {
                Stream stream = new MemoryStream();
                m_streams.Add(stream);

                return stream;
            }
            catch (Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "CreateStream(string, string, Encoding, string, bool)", ex);
                return null;
            }
        }

        /// <summary>
        /// Imprime el documento previamente generado
        /// </summary>
        private static void Print()
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                {
                    throw new Exception("Error: no stream to print.");
                }
                PrintDocument printDoc = new PrintDocument();//este objeto permite obtener la impresora que realizara la impresion

                if (!printDoc.PrinterSettings.IsValid)
                {
                    throw new Exception("Error: cannot find the default printer.");
                }
                else
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    m_currentPageIndex = 0;
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "Print()", ex);
            }
        }

        /// <summary>
        /// Handler for PrintPageEvents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        private static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

                // Adjust rectangular area with printer margins.
                Rectangle adjustedRect = new Rectangle(
                    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                    ev.PageBounds.Width,
                    ev.PageBounds.Height);

                // Draw a white background for the report
                ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

                // Draw the report content
                ev.Graphics.DrawImage(pageImage, adjustedRect);

                // Prepare for the next page. Make sure we haven't hit the end.
                m_currentPageIndex++;
                ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            }
            catch (Exception ex)
            {
                ManagerExceptions.WriteToLog("PrintSalesNotes", "PrintPage(object, PrintPageEventArgs)", ex);
            }
        }

        #endregion
    }
}
