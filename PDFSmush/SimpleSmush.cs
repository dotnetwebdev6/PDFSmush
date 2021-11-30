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
using Dapper;
using System.Data.SqlClient;

namespace PDFSmush
{
    public partial class SimpleSmush : Form
    {
        public SimpleSmush(string[] args)
        {
            InitializeComponent();

            string folder = args[1];
            int count = Convert.ToInt32(args[2]);
            int data_yrmon = Convert.ToInt32(args[3]);
            string state = args[4];
               PdfDocument finalDoc = new PdfDocument();


            var items = Enumerable.Range(1, count).Select(s => folder + @"\" + s + ".pdf").ToList().ToArray();

          


             PdfDocument.Merge(finalDoc, items);
           
             finalDoc.Save(folder + @"\Combined.pdf");

             finalDoc.Close(true);



             PdfLoadedDocument aLoadedDocument = new PdfLoadedDocument(folder + @"\Combined.pdf");
            PdfDocument newDoc = new PdfDocument();
            
            int totalPages = aLoadedDocument.Pages.Count;
            
            int startIndex = 0;
              newDoc.PageSettings.Size = PdfPageSize.Letter;
int endIndex = aLoadedDocument.Pages.Count - 1;
            newDoc.PageSettings.Margins.All = 0; 
          

           

            for (int i = 0; i < aLoadedDocument.Pages.Count; i++) 
{ 
  
               
        PdfTemplate template = aLoadedDocument.Pages[i].CreateTemplate(); 

   PdfPage page = newDoc.Pages.Add(); 
 
   PdfGraphics g = page.Graphics; 

             
 
               
   g.DrawPdfTemplate(template, new PointF(0, 0), new SizeF(page.Size.Width, page.Size.Height)); 
} 







          
            newDoc.Save(folder + @"\Final_" + state + "_" + data_yrmon + ".pdf");
            newDoc.Close(true);
             using (var context = new SqlConnection("Server=10.54.64.156;Database=NewProcessingNoOwner;Persist Security Info=True;user id=DealerTool;password=d7@\\mR-:;"))
            {
                
                var query = string.Format(@"
Insert Into PDFSmushTotalPages (data_yrmon,state_data,TotalPages) values({0},'{1}',{2})",data_yrmon,state,totalPages);
                context.ExecuteScalar(query, commandTimeout: 400);


            }

           
           
        }
    }
}
