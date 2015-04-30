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

namespace AZO_Library.ControlUtilitys
{
    public class PrintReport
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public PrintReport(int idNote, System.Data.DataTable table, string reportName)
        {
            try
            {
                LocalReport report = new LocalReport();
                report.ReportEmbeddedResource = reportName;
                report.DataSources.Add(new ReportDataSource("DataSet1", table));
                ReportParameter parameterFolio = new ReportParameter("Folio", idNote.ToString());
                report.SetParameters(parameterFolio);
                Export(report);
                Print();
            }
            catch(Exception ex)
            {
                //ManagerExceptions.writeToLog("PrintSalesNotes", "PrintSalesNotes", ex);
            }
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        private void Export(LocalReport report)
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
                    stream.Position = 0;
            }
            catch (Exception ex)
            {
                //ManagerExceptions.writeToLog("PrintSalesNotes", "Export", ex);
            }
        }

        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            try
            {
                Stream stream = new MemoryStream();
                m_streams.Add(stream);
                return stream;
            }
            catch (Exception ex)
            {
                //ManagerExceptions.writeToLog("PrintSalesNotes", "CreateStream", ex);
                return null;
            }
        }

        private void Print()
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                    throw new Exception("Error: no stream to print.");
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
                //ManagerExceptions.writeToLog("PrintSalesNotes", "Print", ex);
            }
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
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
                //ManagerExceptions.writeToLog("PrintSalesNotes", "PrintPage", ex);
            }
        }
    }
}
