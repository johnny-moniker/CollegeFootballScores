using System;
using System.ServiceModel.Syndication;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace CollegeFootballScores
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnScores_Click(object sender, EventArgs e)
        {
            HideAllTables();
            ActivateTable(((Button)sender).CommandName);

            switch (((Button)sender).CommandArgument) //commandargument of the calling button tells which type of doc to parse
            {
                case "HTML":
                    //fetch html
                    //http://www.oddsshark.com/ncaaf/odds
                    //var htmlData = (new HtmlWeb()).Load("http://www.ncaa.com/scoreboard/football/fbs");                    
                    var htmlData = (new HtmlWeb()).Load("http://www.cbssports.com/collegefootball/scoreboard");                    
                    //parse values out
                    (new DataParser(htmlData)).GetScoresInTableFormat("cbs").ForEach( row => tblHTMLData.Rows.Add(row));
                    break;
                case "XML":
                    //fetch
                    var xmlData = XDocument.Load("http://xml.pinnaclesports.com/pinnaclefeed.aspx?sporttype=Football&sportsubtype=ncaa");
                    //process
                    (new DataParser(xmlData)).GetScoresInTableFormat("pinnacle").ForEach(row => tblXMLData.Rows.Add(row));
                    break;
                case "RSS":
                    //this is unplugged, waiting on a suitable feed for development
                    var xFeed   = XDocument.Load("http://www.nytimes.com/services/xml/rss/nyt/International.xml");
                    var feed    = SyndicationFeed.Load(xFeed.CreateReader()); //syndication feed object
                    break;
            }
        }

        private void ActivateTable(string tableID)
        {
            ((Table)FindControl(tableID)).Visible = true; //show relevant table (button command name stores the tableID)
        }

        private void HideAllTables()
        {
            foreach (var control in FindControl("allTables").Controls)
            {
                if (control is Table)
                {   ((Table) control).Visible = false;  }
            }
        }
    }
}
