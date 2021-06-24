using cs4rsa.BasicData;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào chương trình học của sinh viên.
    /// </summary>
    public class StudentProgramCrawler
    {
        private string _specialString;
        private string _fakeUrl;
        public List<HtmlNode> _fileNodes;
        public List<HtmlNode> _folderNodes;
        /// <summary>
        /// Nhận vào một chuỗi đặc biệt được mã hoá từ mã sinh viên.
        /// </summary>
        /// <param name="specialString">Mã đặc biệt được mã hoá từ mã sinh viên.</param>
        public StudentProgramCrawler(string specialString)
        {
            _specialString = specialString;
            _fakeUrl = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t=1623769137415&studentidnumber={specialString}&acaLevid=3&curid=605&cursectionid=2001";
            List<HtmlNode> allNodes = GetAllTrTag();
            GetFolderNodes(allNodes);
            GetFolderNodes(allNodes);
        }

        public List<HtmlNode> GetAllTrTag()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(_fakeUrl);
            List<HtmlNode> trTags = doc.DocumentNode.SelectNodes("//tr").ToList();
            trTags.RemoveAt(0);
            return trTags;
        }


        /// <summary>
        /// Chia list node thành các fileNode và FolderNode.
        /// </summary>
        /// <param name="trNodes">Danh sách tất cả các tr node trong chương trình học.</param>
        public void GetFolderNodes(List<HtmlNode> trNodes)
        {
            List<HtmlNode> folderNodes = new List<HtmlNode>();
            List<HtmlNode> fileNodes = new List<HtmlNode>();
            foreach (HtmlNode node in trNodes)
            {
                string innerHtml = node.InnerHtml;
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(innerHtml);
                HtmlNode rootNode = htmlDocument.DocumentNode;
                HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                HtmlNode spanFileNode = rootNode.SelectSingleNode("//span[@class='file']");
                if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                    folderNodes.Add(node);
                if (spanFileNode != null)
                    fileNodes.Add(node);
            }
            _folderNodes = folderNodes;
            _fileNodes = fileNodes;
        }

        public List<ProgramFolder> GetProgramFolders(List<HtmlNode> folderNodes)
        {
            List<ProgramFolder> programFolders = new List<ProgramFolder>();
            foreach (HtmlNode htmlNode in folderNodes)
            {
                string idValue = htmlNode.Attributes["id"].Value;
                string classValue = htmlNode.Attributes["class"].Value;
                string childOfNode;
                string id = idValue.Split(new char[] { '-' })[1];
                if (classValue.Equals("toptitle"))
                    childOfNode = "0";
                else
                {
                    childOfNode = classValue.Split(new char[] { '-' })[3];
                }
                string name = GetNameFolderNode(htmlNode);

            }
            return null;
        }

        private string GetNameFolderNode(HtmlNode folderNode)
        {
            string html = folderNode.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode span = docNode.SelectSingleNode("//span");
            return Helpers.StringHelper.SuperCleanString(span.InnerText);
        }

        private string GetStudyModeText(HtmlNode folderNode)
        {
            string html = folderNode.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode span = docNode.SelectSingleNode("//span/span");
            return Helpers.StringHelper.SuperCleanString(span.InnerText);
        }
    }
}
