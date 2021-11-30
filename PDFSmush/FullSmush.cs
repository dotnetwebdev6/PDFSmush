using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFSmush
{
    public partial class FullSmush : Form
    {
        public FullSmush(string[] args)
        {
            InitializeComponent();

            string folder = args[1];
            int count = Convert.ToInt32(args[2]);
               PdfDocument finalDoc = new PdfDocument();


            var items = Enumerable.Range(1, count).Select(s => folder + @"\" + s + ".pdf").ToList().ToArray();
            
            //convert all fonts to embedded
            foreach (var item in items)
            {
                if(item.Contains("1.pdf"))
                {
                    continue;
                }
                PdfLoadedDocument aLoadedDocument = new PdfLoadedDocument(item);
                int i = 0;
                foreach (var item2 in aLoadedDocument.UsedFonts)
                {
                  //  if(item2.Type != PdfFontType.TrueTypeEmbedded)
                    try
                   {
                        aLoadedDocument.UsedFonts[i].Replace(new PdfTrueTypeFont(new Font("Arial", 12), false));

                    }
                    catch (Exception)
                    {

                    }


                    i++;
                }
                //Create a new compression option.
              //  PdfCompressionOptions options = new PdfCompressionOptions();

                //Enable the optimize font option
             //   options.OptimizeFont = true;

                //Assign the compression option to the document
                //aLoadedDocument.CompressionOptions = options;
                //Save the document
                aLoadedDocument.Save(item);

                aLoadedDocument.Close(true);
            }


             PdfDocument.Merge(finalDoc, items);

             finalDoc.Save(folder + @"\Combined.pdf");

             finalDoc.Close(true);

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(folder + @"\Combined.pdf");


            PdfDocument document = new PdfDocument();
            //  ////Set increasing value of the page margin as margin of the PDF document

            PdfUnitConvertor convertor = new PdfUnitConvertor();

            float inches = .5f;

            //Convert inches to points 
            float points = convertor.ConvertUnits(inches, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);
            document.PageSettings.Margins.Left = points;
            // document.PageSettings.Margins.Right = 40;
            //  document.PageSettings.Margins.Top = 20;
            // document.PageSettings.Margins.Bottom = 20;
            for (int i = 0; i < loadedDocument.Pages.Count; i++)
            {
                //Get loaded page as template
                PdfTemplate template = loadedDocument.Pages[i].CreateTemplate();

                //Create new page
                PdfPage page = document.Pages.Add();

                //Create Pdf graphics for the page
                PdfGraphics g = page.Graphics;


                //Draw template with the size as loaded page size
                g.DrawPdfTemplate(template, new PointF(0, 0), new SizeF(page.GetClientSize().Width, page.GetClientSize().Height));
            }
            //Save and Close document
            document.Save(folder + @"\Final.pdf");
            document.Close(true);
            this.Close();
        }
    }
}
