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
using Syncfusion.Pdf.Tables;
using System.IO;

namespace PDFSmush
{
    public partial class CoverGenerator : Form
    {
        public CoverGenerator(string[] args)
        {
            InitializeComponent();

            string folder = args[1];
           string csid = args[2];
            int data_yrmon = Convert.ToInt32(args[3]);
            string state = args[4];

            string type = args[5];


           
             using (var context = new SqlConnection("Server=10.54.64.156;Database=Crosssell;Persist Security Info=True;user id=DealerTool;password=d7@\\mR-:;"))
            {
                var customer_query = string.Format(@"
select * From customer where csid = {0}",csid);
                var customer = context.Query(customer_query, commandTimeout: 400).ToList();

                var mailing_address_query = string.Format(@"
select * From customer_address where csid = {0} and isnull(del,'N')  = 'N' and isnull(type,'') = 'Mailing'",csid);
                var mailing_addresses = context.Query(mailing_address_query, commandTimeout: 400).ToList();

                var location_address_query = string.Format(@"
select * From customer_address where csid = {0} and isnull(del,'N')  = 'N'and isnull(type,'') = 'Location'",csid);
                var location_addresses = context.Query(location_address_query, commandTimeout: 400).ToList();

                var null_address_query = string.Format(@"
select * From customer_address where csid = {0} and isnull(del,'N')  = 'N'and isnull(type,'') = ''",csid);
                var null_addresses = context.Query(null_address_query, commandTimeout: 400).ToList();

                dynamic foundAddress = mailing_addresses.Count() != 0 ? mailing_addresses : (location_addresses.Count() != 0 ? location_addresses : null_addresses);

              

                var contacts_query = string.Format(@"
select * From customer_contact cc 
inner join customer_contact_type ct on ct.csid = cc.csid and ct.con_seq = cc.seq and ct.type_cde in (4,5,6)
where cc.csid = {0} and isnull(cc.del,'N')  = 'N' and (isnull(cc.first,'') <> '' or isnull(cc.last,'') <> '') ",csid);
                var contacts = context.Query(contacts_query, commandTimeout: 400).ToList();

                string contactName = string.Format("{0} {1}",contacts.FirstOrDefault()?.first, contacts.FirstOrDefault()?.last);
                if(contacts.Count() == 0)
                {
                    contactName = customer[0].name;
                }

                List<dynamic> products = new List<dynamic>();
                int countProducts = 1;
                 if(type != "MarketIntel")
                {

                var products_query = string.Format(@"
select distinct cp.*,ma.runname,cp.newused,cpd.format_type as type From cust_prod cp
inner join cust_prod_delivery cpd on cpd.prod_nbr = cp.nbr and cpd.csid = cp.csid and delivery = 'Print'
left join ProductionNoOwner.dbo.market_areas ma on ma.runnumber = cp.ma and ma.state = cp.state_data and ma.hidden = 0
where cp.csid = {0} and cp.state_data = '{1}' and cp.status <> 'C' and cp.product not like 'Auto Market INTEL%' order by cp.ma,product",csid,state);
                products = context.Query(products_query, commandTimeout: 400).ToList();
                countProducts = products.Count();


                 string auditAddressQuery = string.Format(@"insert into NewProcessingNoOwner.dbo.PDFSmushAddresses (csid,addressline1,addressline2,city,state,zip,name)
                values ({0},'{1}','{2}','{3}','{4}','{5}','{6}')",csid,foundAddress[0].address1, foundAddress[0].address2, foundAddress[0].city, foundAddress[0].st, foundAddress[0].zip,contactName);
                context.ExecuteScalar(auditAddressQuery, commandTimeout: 400);

                }

                PdfDocument newDoc = new PdfDocument();
                  PdfLoadedDocument aLoadedDocument = new PdfLoadedDocument(@"\\10.232.224.68\e$\Easy Morph Files\Process\coverTemplate.pdf");
                newDoc.ImportPage(aLoadedDocument,0);
               
                
                FontStyle fs = new FontStyle();
                    
                PdfFont mainFont = new PdfTrueTypeFont(new Font("Helvetica", 8f),fs,8,false,true);
                PdfFont regfont;
               
                int numPagesNeed = (countProducts / 24);
                int fullPagesNeeded = 1;
                fullPagesNeeded = fullPagesNeeded + numPagesNeed;
                if(countProducts == 24)
                {
                    fullPagesNeeded = 1;
                }

              //  for(int i2 = 1; i2 < fullPagesNeeded; i2++)
               // {
                  //  newDoc.ImportPage(aLoadedDocument,0);
               // }
               
                if(countProducts <= 15)
                {
                regfont = new PdfTrueTypeFont(new Font("Helvetica", 8f),fs,8,false,true);
               
                }
                else
                {
                   
                    regfont = new PdfTrueTypeFont(new Font("Helvetica", 6f),fs,6,false,true);
                }
             
                int currentPage = 0;
                List<dynamic> usedProducts = products;
                  PdfPage page = newDoc.Pages[0];
                 PdfGraphics graphics = page.Graphics;

                for(int i3 = 0; i3 < fullPagesNeeded; i3++)
                {
                  
                    if(currentPage == 0)
                    {
                  graphics.DrawString(contactName, mainFont, PdfBrushes.Black, new PointF(58, 195));
                graphics.DrawString(foundAddress[0].address1, mainFont, PdfBrushes.Black, new PointF(58, 205));
                        if(!string.IsNullOrEmpty(foundAddress[0].address2?.Trim()))
                            {
                        graphics.DrawString(foundAddress[0].address2, mainFont, PdfBrushes.Black, new PointF(58, 215));
                            graphics.DrawString(string.Format("{0} {1} {2}", foundAddress[0].city, foundAddress[0].st, foundAddress[0].zip), mainFont, PdfBrushes.Black, new PointF(58, 225));

                        }
                        else
                        {
                            graphics.DrawString(string.Format("{0} {1} {2}", foundAddress[0].city, foundAddress[0].st, foundAddress[0].zip), mainFont, PdfBrushes.Black, new PointF(58, 215));
                        }
                
                    }
                int i=0;

                  foreach(var item in usedProducts.Take(24))
                {
                    string fullString = string.Empty;
                    string prodName = item.product;
                    string runNumberWithStateAndNewUsed = string.Empty;
                    string NU = (item.newused?.Trim() == "New" ? "N" : "U");
                    if(prodName == "Analysis Only")
                    {
                        prodName = "Market Analysis";
                        runNumberWithStateAndNewUsed = string.Format("{0}{1}{2} - {3}",item.state_data, item.ma, NU, item.runname);
                    }
                    if(prodName == "Full Detail")
                    {
                        prodName = "Market Analysis - Full Detail";
                        runNumberWithStateAndNewUsed = string.Format("{0}{1}{2} - {3}",item.state_data, item.ma, NU, item.runname);
                    }
                    else if(prodName == "Summaries")
                    {
                        prodName = "Statewide Summaries";
                        runNumberWithStateAndNewUsed = string.Format("{0}{1}",item.state_data, NU);
                    }
                    else if(prodName == "Snap Shot")
                    {
                        runNumberWithStateAndNewUsed = string.Format("{0}{1}{2} - {3}",item.state_data, item.ma, NU, item.runname);
                    }

                    fullString = prodName + " - " + item.type + " - " + runNumberWithStateAndNewUsed;

                        if(i3 == 0)
                        {

                  graphics.DrawString(fullString, regfont, PdfBrushes.Black, new PointF(58, 282+(i*10)));
                        }
                        else
                        {
                            graphics.DrawString(fullString, regfont, PdfBrushes.Black, new PointF(240, 282+(i*10)));
                        }
                    i++;
                }

                  if(usedProducts.Count > 24)
                    {
                  usedProducts.RemoveRange(currentPage*23,24);
                    }
                  currentPage++;
                }

                if(type == "MarketIntel")
                {
                     graphics.DrawString("Market Intel Report", regfont, PdfBrushes.Black, new PointF(58, 282+(10)));
                }
               

                newDoc.Save(folder + @"\Output.pdf");
                newDoc.Close(true);
                aLoadedDocument.Close(true);

            }

           
            
        }
    }
}
