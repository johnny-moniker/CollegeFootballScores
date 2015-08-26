using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace CollegeFootballScores
{
    public class DataParser
    {
        
        private string          feedMode;
        private string          provider;
        private HtmlDocument    htmlData;
        private XDocument       xmlData;
        private SyndicationFeed rssData;
        private readonly string nodata = "No Data Available";

        private Dictionary<string, string> GameInfo = new Dictionary<string, string>
        {
            {   "StartTime",                ""}, 
            {   "StartYear",                ""},
            {   "StartMonth",               ""},
            {   "StartDay",                 ""},
            {   "StartUnixTime",            ""}, //this is best to get where possible - absolute value can be converted to any time zone.
            {   "StartFormatted",           ""},
            {   "Broadcaster",              ""},
            {   "HomeTeamName",             ""},
            {   "HomeTeamLogoSource",       ""},
            {   "HomeTeamOdds",             ""},
            {   "VisitingTeamName",         ""},
            {   "VisitingTeamLogoSource",   ""},
            {   "VisitingTeamOdds",         ""},
            {   "GameOddsType",             ""}
        };

        private Dictionary<string, string> HTMLNodeMap = new Dictionary<string, string>
        {
            {"AllGamesXpathSelector",   ""},
            {"TeamLogoXpathSelector",   ""},
            {"TeamNameXpathSelector",   ""},
            {"GameInfoXpathSelector",   ""},
            {"OddsTypeXpathSelector",   ""},
            {"LineOddsXpathSelector",   ""},
            {"SpreadOddsXpathSelector", ""}
        };


        #region Class Constructors

        public DataParser(HtmlDocument rawHTML)
        {
            feedMode    = "HTML";
            htmlData    =  rawHTML;
        }

        public DataParser(XDocument rawXML)
        {
            feedMode    = "XML";
            xmlData     = rawXML;
        }

        public DataParser(SyndicationFeed rssFeed)
        {
            feedMode    = "Rss";
            rssData     = rssFeed;
        }

        #endregion

        #region public methods

        public  List<TableRow> GetScoresInTableFormat(string prov)
        {
            provider = prov;
            switch (feedMode)
            {
                case "HTML":
                    return HTMLDataTableFormatter();
                case "XML":
                    return XMLTableFormatter();
                //case "RSS":
               //     break;
                default:
                    return new List<TableRow>() ;
            }
        }
        
        #endregion

        #region helper methods

        #region html specific parse logic

        private List<TableRow> HTMLDataTableFormatter()
        {
            BuildHTMLMap();

            var tablerows = new List<TableRow>();
           // var elements = htmlData.DocumentNode.SelectNodes(HTMLNodeMap["AllGamesXpathSelector"]);
            foreach (var gameInfoTable in htmlData.DocumentNode.SelectNodes(HTMLNodeMap["AllGamesXpathSelector"]))
            {
                LoadGameInfoDictionary(gameInfoTable);
                tablerows.Add(
                    new TableRow
                    {
                        Cells = {   
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["StartUnixTime"])         ? nodata : GameInfo["StartUnixTime"])},
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["StartFormatted"])        ? nodata : GameInfo["StartFormatted"]) },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["GameOddsType"])          ? nodata : GameInfo["GameOddsType"]) },   
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["VisitingTeamName"])      ? nodata : GameInfo["VisitingTeamName"]) },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["VisitingTeamLogoSource"])? nodata : "<img src='" + GameInfo["VisitingTeamLogoSource"] + "' />") },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["VisitingTeamOdds"])      ? nodata : GameInfo["VisitingTeamOdds"]) },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["HomeTeamName"])          ? nodata : GameInfo["HomeTeamName"]) },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["HomeTeamLogoSource"])    ? nodata : "<img src='" + GameInfo["HomeTeamLogoSource"] + "' />") },
                            new TableCell   {   Text = (string.IsNullOrEmpty(GameInfo["HomeTeamOdds"])          ? nodata : GameInfo["HomeTeamOdds"]) }
                            }
                    });    
            }            

            return tablerows;   
        }

        private void BuildHTMLMap()
        {
            switch (provider)
            {
                case "cbs":     //working
                    HTMLNodeMap["AllGamesXpathSelector"] = "//table[contains(@class,'preEvent')]";
                    HTMLNodeMap["TeamLogoXpathSelector"] = "td/a/img";
                    HTMLNodeMap["TeamNameXpathSelector"] = "td/div/a";
                    HTMLNodeMap["GameTimeXpathSelector"] = "td/span/span";
                    HTMLNodeMap["LineOddsXpathSelector"] = "td[contains(@class,'gameOdds')]";
                    HTMLNodeMap["OddsTypeXpathSelector"] = "td[contains(@class,'gameOdds')]";
                    break;
                case "official": //still in development (jason)
                    HTMLNodeMap["AllGamesXpathSelector"]    = "//div[contains(@class, 'game-contents')]";
                    HTMLNodeMap["TeamLogoXpathSelector"]    = "//td[contains(@class, 'school')]";
                    HTMLNodeMap["TeamNameXpathSelector"]    = "//div[contains(@class, 'team')]/a";
                    HTMLNodeMap["GameTimeXpathSelector"]    = "//div[contains(@class, 'game-status')]";
                    //HTMLNodeMap["LineOddsXpathSelector"] = "td[contains(@class,'gameOdds')]";
                    //HTMLNodeMap["OddsTypeXpathSelector"] = "td[contains(@class,'gameOdds')]";
                    break;
            }
        }

        private void LoadGameInfoDictionary     (HtmlNode gameTable)
        {
            foreach (var gameTableRow in gameTable.ChildNodes)
            {
                switch (provider)
                {
                    case "cbs":
                        if (gameTableRow.HasAttributes)
                        {
                            AddRowDataToDictionary(gameTableRow);
                        }
                        break;
                    case "official":
                        AddRowDataToDictionary(gameTableRow);
                        break;
                }
            }
        }

        private void AddRowDataToDictionary     (HtmlNode gameTableRow)
        {
            switch (provider)
            {
                case "cbs":
                    var gameTableRowclass = gameTableRow.Attributes.Where(attribute => attribute.Name == "class").Select(attribute => attribute.Value).FirstOrDefault();

                    if (gameTableRowclass.Contains("teamInfo"))
                    {
                        var teamType = gameTableRowclass.Contains("homeTeam") ? "HomeTeam" : "VisitingTeam";
                        AddTeamRowDataToDictionary(gameTableRow, teamType);
                    }
                    else if(gameTableRowclass.Contains("gameInfo"))
                    {
                        AddGameRowDataToDictionary(gameTableRow);
                    }  
                    break;
                case "official":
                    break;
            }
         
        }

        private void AddTeamRowDataToDictionary (HtmlNode teamData, string teamType)
        {
            GameInfo[teamType + "Name"]         = teamData.SelectSingleNode(HTMLNodeMap["TeamNameXpathSelector"]).InnerText;
            GameInfo[teamType + "Odds"]         = teamData.SelectSingleNode(HTMLNodeMap["LineOddsXpathSelector"]).InnerText;
            var image = teamData.SelectSingleNode(HTMLNodeMap["TeamLogoXpathSelector"]);
            GameInfo[teamType + "LogoSource"]   = image.GetAttributeValue("delaysrc", "no source");
        }

        private void AddGameRowDataToDictionary (HtmlNode gameData)
        {
            //var gameTimeNode = gameData.SelectSingleNode(HTMLNodeMap["GameTimeXpathSelector"]);

            GameInfo["StartUnixTime"]   = gameData.SelectSingleNode(HTMLNodeMap["GameTimeXpathSelector"]).GetAttributeValue("data-gmt", "no start time info");
            GameInfo["StartFormatted"]  = gameData.SelectSingleNode(HTMLNodeMap["GameTimeXpathSelector"]).InnerText;
            GameInfo["GameOddsType"]    = gameData.SelectSingleNode(HTMLNodeMap["OddsTypeXpathSelector"]).InnerText;
        }
               
        #endregion

        #region xml specific parse logic

        private List<TableRow> XMLTableFormatter()//so far this is provider specific, will create more generic logic if/when a viable secondary option is found.
        {
            var tablerows = new List<TableRow>();
            foreach (var game in xmlData.Descendants("event"))
            {
                var visitingteam    = game.Descendants("visiting_home_draw").Where(team => team.Value == "Visiting").Select(element => element.Parent);
                var hometeam        = game.Descendants("visiting_home_draw").Where(team => team.Value == "Home")    .Select(element => element.Parent);
                tablerows.Add(
                   new TableRow
                   {
                       Cells = {   
                            
                            new TableCell   {   Text = game.Descendants("event_datetimeGMT").FirstOrDefault().ToString() },
                            new TableCell   {   Text = "Both" },   
                            new TableCell   {   Text = visitingteam.Descendants("participant_name").FirstOrDefault().ToString()},
                            new TableCell   {   Text = game.Descendants("spread_visiting").FirstOrDefault().ToString()},
                            new TableCell   {   Text = game.Descendants("spread_adjust_visiting").FirstOrDefault().ToString()},
                            new TableCell   {   Text = hometeam.Descendants("participant_name").FirstOrDefault().ToString()}, 
                            new TableCell   {   Text = game.Descendants("spread_home").FirstOrDefault().ToString()},
                            new TableCell   {   Text = game.Descendants("spread_adjust_home").FirstOrDefault().ToString()}
                            }
                   });    
            }
            return tablerows;
        }

        #endregion

        #endregion

        // CBS OUTER GAME OBJECT
		//
        // <table class="lineScore preEvent">
        //    <tr class="gameInfo">
        //        <td>
        //            <span class="gameDate"> <span class="gmtTime" data-gmt="1441481400" data-gmt-format="%r  %q %e - %I:%M %p %Z">Sat.  Sept. 5 - 3:30 PM EDT</span>  (ABC) </span>
        //        </td>
        //      <td class="gameOdds">Line</td>
        //  </tr>
        //  <tr class="teamInfo awayTeam" >
        //      <td class="teamName">
        //        <a href="/collegefootball/teams/page/BYU/brigham-young-cougars">
        //            <img delaysrc="http://sports.cbsimg.net/images/collegefootball/logos/25x25/BYU.png" width="25" height="25" border="0" class="teamLogo" />
        //        </a>
        //        <div class="teamLocation">
        //                <a href="/collegefootball/teams/page/BYU/brigham-young-cougars">BYU</a>
        //        </div>
        //      </td>
        //      <td class="gameOdds">-</td>
        //    </tr>
        //    <tr class="teamInfo homeTeam">
        //        <td class="teamName">
        //            <a href="/collegefootball/teams/page/NEB/nebraska-cornhuskers" />
        //                <img delaysrc="http://sports.cbsimg.net/images/collegefootball/logos/25x25/NEB.png" width="25" height="25" border="0" class="teamLogo"></a>
        //                <div class="teamLocation">
        //                    <a href="/collegefootball/teams/page/NEB/nebraska-cornhuskers">Nebraska</a>
        //                </div>
        //        </td>
        //        <td class="gameOdds">-</td>
        //    </tr>
        //</table>


      
    }
}