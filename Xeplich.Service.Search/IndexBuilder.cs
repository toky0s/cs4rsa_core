using Cs4rsa.Database.DataProviders;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xeplich.Service.Search.Properties;


namespace Xeplich.Service.Search
{
    public class DataModel
    {
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string Discipline { get; set; }
        public string Keyword { get; set; }
        public string SubjectDescription { get; set; }
        public string DisplayedText { get; set; }
    }

    public class IndexBuilder
    {
        private readonly RawSql _rawSql;

        public IndexBuilder(RawSql rawSql)
        {
            _rawSql = rawSql;
        }

        public void BuildIndex()
        {
            const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
            var IndexPath = Settings.Default.LuceneIndexPath; // Đường dẫn tới thư mục lưu trữ index

            // Tạo analyzer và Directory lưu index
            var analyzer = new VietnameseAnalyzer(AppLuceneVersion);
            var indexDir = FSDirectory.Open(IndexPath);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer)
            {
                OpenMode = OpenMode.CREATE
            };

            using (var writer = new IndexWriter(indexDir, indexConfig))
            {
                // Lấy dữ liệu từ cơ sở dữ liệu
                var dataModels = GetDataFromDatabase();
                foreach (var data in dataModels)
                {
                    Console.WriteLine($"Indexing Subject: {data.SubjectName} {data.SubjectCode} {data.Discipline} {data.Keyword} {data.SubjectDescription}");
                    var doc = new Document
            {
                new TextField("SubjectName", data.SubjectName ?? "", Field.Store.YES),
                new TextField("SubjectCode", data.SubjectCode ?? "", Field.Store.YES),
                new TextField("Discipline", data.Discipline ?? "", Field.Store.YES),

                // Nếu Keyword là số, dùng Int32Field để tối ưu range query
                new Int32Field("Keyword", int.TryParse(data.Keyword, out var kw) ? kw : 0, Field.Store.YES),

                new TextField("SubjectDescription", data.SubjectDescription ?? "", Field.Store.YES),
            };
                    writer.AddDocument(doc);
                }
                writer.Commit();
            }
        }

        private List<DataModel> GetDataFromDatabase()
        {
            string sql = "SELECT Keywords.SubjectName as SubjectName, Disciplines.Name || ' ' || Keywords.Keyword1 as SubjectCode, Disciplines.name as Discipline, Keywords.Keyword1 as Keyword, '' as SubjectDescription from Disciplines join Keywords on Keywords.DisciplineId = Disciplines.DisciplineId";
            return _rawSql.ExecReader(sql, record =>
            {
                return new DataModel
                {
                    SubjectName = record.GetString(record.GetOrdinal("SubjectName")),
                    SubjectCode = record.GetString(record.GetOrdinal("SubjectCode")),
                    Discipline = record.GetString(record.GetOrdinal("Discipline")),
                    Keyword = record.GetString(record.GetOrdinal("Keyword")),
                    SubjectDescription = record.GetString(record.GetOrdinal("SubjectDescription"))
                };
            });
        }
        public void Search(
            out List<DataModel> dataModels,
            out int totalHits,
            string keyword,
            int maxRecords = 15)
        {
            var IndexPath = Settings.Default.LuceneIndexPath;
            var results = new List<DataModel>();
            var analyzer = new VietnameseAnalyzer(LuceneVersion.LUCENE_48);

            using (var dir = FSDirectory.Open(IndexPath))
            {
                using (var reader = DirectoryReader.Open(dir))
                {
                    var searcher = new IndexSearcher(reader);

                    // Tìm trên nhiều field
                    var fields = new[] { "SubjectName", "SubjectCode", "Discipline", "Keyword", "SubjectDescription" };
                    var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, fields, analyzer);

                    Query query;
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        // Nếu keyword rỗng thì lấy tất cả document
                        query = new MatchAllDocsQuery();
                    }
                    else
                    {
                        query = parser.Parse(keyword);
                    }

                    var topDocs = searcher.Search(query, maxRecords);
                    var hits = topDocs.ScoreDocs;
                    totalHits = topDocs.TotalHits;
                    foreach (var hit in hits)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        results.Add(new DataModel
                        {
                            SubjectName = doc.Get("SubjectName"),
                            SubjectCode = doc.Get("SubjectCode"),
                            Discipline = doc.Get("Discipline"),
                            Keyword = doc.Get("Keyword"),
                            SubjectDescription = doc.Get("SubjectDescription")
                        });
                    }

                    dataModels = results;
                }
            }
        }

        public void SearchWithBoost(
            out List<DataModel> dataModels,
            out int totalHits,
            string keyword,
            int maxRecords = 15)
        {
            var IndexPath = Settings.Default.LuceneIndexPath;
            var results = new List<DataModel>();
            var analyzer = new VietnameseAnalyzer(LuceneVersion.LUCENE_48);

            using (var dir = FSDirectory.Open(IndexPath))
            {
                using (var reader = DirectoryReader.Open(dir))
                {
                    var searcher = new IndexSearcher(reader);

                    // Các field cần tìm
                    var fields = new[] { "SubjectName", "SubjectCode", "Discipline", "Keyword", "SubjectDescription" };

                    // Thiết lập trọng số cho từng field
                    var boosts = new Dictionary<string, float>
                    {
                        { "SubjectCode", 4.0f },          // cao nhất
                        { "Discipline", 3.0f },           // kết hợp Discipline
                        { "Keyword", 3.0f },              // kết hợp Keyword
                        { "SubjectName", 2.0f },          // tiếp theo
                        { "SubjectDescription", 1.0f }    // thấp nhất
                    };

                    var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, fields, analyzer, boosts);

                    Query query;
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        query = new MatchAllDocsQuery();
                    }
                    else
                    {
                        query = parser.Parse(keyword);
                    }

                    AddHighlighter(query);

                    var topDocs = searcher.Search(query, maxRecords);
                    var hits = topDocs.ScoreDocs;
                    totalHits = topDocs.TotalHits;
                    foreach (var hit in hits)
                    {
                        var doc = searcher.Doc(hit.Doc);
                        results.Add(new DataModel
                        {
                            SubjectName = doc.Get("SubjectName"),
                            SubjectCode = doc.Get("SubjectCode"),
                            Discipline = doc.Get("Discipline"),
                            Keyword = doc.Get("Keyword"),
                            SubjectDescription = doc.Get("SubjectDescription")
                        });
                    }

                    dataModels = results;
                }
            }

            void AddHighlighter(Query query)
            {
                // Thêm Scorer và Highlighter
                var scorer = new QueryScorer(query);
                var formatter = new SimpleHTMLFormatter("<b>", "</b>");
                var highlighter = new Highlighter(formatter, scorer);
            }
        }
    }
}
