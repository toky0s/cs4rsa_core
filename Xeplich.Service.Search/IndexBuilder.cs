using Cs4rsa.Database.DataProviders;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
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
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            var indexDir = FSDirectory.Open(IndexPath);
            var indexConfig = new Lucene.Net.Index.IndexWriterConfig(AppLuceneVersion, analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
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
                        new TextField("SubjectName", data.SubjectName, Field.Store.YES),
                        new TextField("SubjectCode", data.SubjectCode, Field.Store.YES),
                        new TextField("Discipline", data.Discipline, Field.Store.YES),
                        new TextField("Keyword", data.Keyword, Field.Store.YES),
                        new TextField("SubjectDescription", data.SubjectDescription, Field.Store.YES)
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
        public List<DataModel> Search(string keyword, int maxRecords = 15)
        {
            var IndexPath = Settings.Default.LuceneIndexPath;
            var results = new List<DataModel>();
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            using (var dir = FSDirectory.Open(IndexPath))
            {
                using (var reader = DirectoryReader.Open(dir))
                {
                    var searcher = new IndexSearcher(reader);

                    // Tìm trên nhiều field
                    var fields = new[] { "SubjectName", "SubjectCode", "Discipline", "Keyword", "SubjectDescription" };
                    var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, fields, analyzer);

                    Query query = parser.Parse(keyword);

                    var hits = searcher.Search(query, maxRecords).ScoreDocs;
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

                    return results;
                }
            }
        }
    }
}
