using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

using System.IO;

namespace Xeplich.Service.Search
{
    /// <summary>
    /// Analyzer tuỳ chỉnh: Tokenize chuẩn -> lowercase -> bỏ dấu (fold về ASCII)
    /// Giúp tìm "cong nghe" match được với "Công nghệ".
    /// </summary>
    public class VietnameseAnalyzer : Analyzer
    {
        private readonly LuceneVersion _matchVersion;

        public VietnameseAnalyzer(LuceneVersion matchVersion)
        {
            _matchVersion = matchVersion;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var tokenizer = new StandardTokenizer(_matchVersion, reader);
            TokenStream stream = new StandardFilter(_matchVersion, tokenizer);
            stream = new LowerCaseFilter(_matchVersion, stream);
            stream = new ASCIIFoldingFilter(stream); // bỏ dấu: "ệ" -> "e", "ầ" -> "a", ...
            return new TokenStreamComponents(tokenizer, stream);
        }
    }
}