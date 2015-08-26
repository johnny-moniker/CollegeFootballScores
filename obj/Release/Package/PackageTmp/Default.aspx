<%@ Page Title="About" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CollegeFootballScores.About" %>
<html>
    <head>
        <Title></Title>
        <style>
            table {
                border-collapse: collapse;
                text-align: center;
                float: left;
                clear: both;
            }
            
            td {
                 border: 1px solid black;
                padding: 1px;
            }
            .radiopanel {
                float: left;
                clear: both;
                display: none;
            }
            .feed-button{
				border: 2px solid blue;
				padding: 2px 4px;
				color: blue;
				background-color:whitesmoke;
				font-size:16px;
				font-weight:bold;
				border-radius:3px;
				text-align:center;
				float:left;
				margin-left: 10px;
				margin: 10px 0 10px 10px;
                cursor: pointer;
			}

            .table-headercell {
                padding: 4px;
                background-color: purple;
                color: yellow;
            }
        </style>
    </head>
    <body>
        <form id="Form1" runat="server">
            <asp:button ID="btnHTMLscraper" runat="server" OnClick="btnScores_Click" text="Page Scrape"     CommandArgument="HTML"  CommandName="tblHTMLData"  CssClass="feed-button" />
            <asp:button ID="btnXML"         runat="server" OnClick="btnScores_Click" text="XML Endpoint"    CommandArgument="XML"   CommandName="tblXMLData"   CssClass="feed-button" />
            <asp:button ID="btnRSS"         runat="server" OnClick="btnScores_Click" text="RSS Parse"       CommandArgument="RSS"   CommandName="tblRSSData"   CssClass="feed-button" />
            <div class="radiopanel"><input id="radio-cbs" type="radio" value="cbs" name="htmlfeed"/><label for="radio-cbs">cbs</label><input id="radio-official" type="radio" value="official ncaa" name="htmlfeed"/><label for="radio-official">official</label></div>
            <br />
            <asp:Image ID="Image1" runat="server" Height="33px" ImageUrl="~/Images/loading.gif" Visible="False" Width="35px" />
            <br />

            <div id="allTables" runat="server">
                <asp:Table runat="server" ID="tblHTMLData" Visible="False">
                <asp:TableHeaderRow runat="server">
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Game Start <br/>(unix time)</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Formatted Start Time</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Type Of Odds</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visiting Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visiting Logo</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visitor Odds</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Logo</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Odds</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            <asp:Table runat="server" ID="tblXMLData" Visible="False">
                <asp:TableHeaderRow  runat="server">
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Formatted Start Time</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Type Of Odds</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visiting Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visitor Line</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visitor Spread</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Line</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Spread</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            <asp:Table runat="server" ID="tblRSSData" Visible="False">
                <asp:TableHeaderRow  runat="server">
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Game Start <br/>(unix time)</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Formatted Start Time</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Type Of Odds</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visiting Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visiting Logo</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Visitor Odds</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Team</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Logo</asp:TableHeaderCell>
                    <asp:TableHeaderCell runat="server" CssClass="table-headercell">Home Odds</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
            </div>
            
        </form>
    </body>
</html>
