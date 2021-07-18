using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using HtmlAgilityPack;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào này trả về một bộ data chứa các item là một bắt cặp với thứ
    /// trong tuần và nơi học, nhằm xác định xung đột nơi học giữa hai giờ đầu và sau.
    /// </summary>
    public class MetaDataCrawler
    {
        private DayPlaceMetaData _metaData = null;

        private readonly string _detailClassGroupUrl;
        public MetaDataCrawler(string detailClassGroupUrl)
        {
            _detailClassGroupUrl = detailClassGroupUrl;
        }

        public DayPlaceMetaData ToDayPlaceMetaData()
        {
            if (_metaData != null)
                return _metaData;
            else
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument document =  htmlWeb.Load(_detailClassGroupUrl);
                string innertext = document.DocumentNode.InnerHtml;
                string xpath = "//ul[contains(@class, 'thugio')]";
                HtmlNode thugio = document.DocumentNode.SelectNodes(xpath).ToArray()[1];
                HtmlNode[] liTags = thugio.Descendants("li").ToArray();

                DayPlaceMetaData metaData = new DayPlaceMetaData();

                foreach (HtmlNode litag in liTags)
                {
                    string liInnerText = litag.InnerText;
                    string placeText = litag.ChildNodes["a"].InnerText;
                    string[] separatingStrings = { "&nbsp;-" };
                    string[] splitData = StringHelper.SuperCleanString(liInnerText)
                                                    .Replace(placeText, "")
                                                    .Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);

                    DayOfWeek day = BasicDataConverter.ToDayOfWeek(splitData[0]);
                    Place place = BasicDataConverter.ToPlace(placeText);
                    Room room = new Room(StringHelper.SuperCleanString(splitData[1]), place);
                    DayPlacePair dayPlacePair = new DayPlacePair(day, room, place);
                    metaData.AddDayTimePair(day, dayPlacePair);
                }
                _metaData = metaData;
                return metaData;
            }
        }
    }
}
