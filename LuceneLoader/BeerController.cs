using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LuceneLoader
{
    public class BeerController : ApiController
    {
        private readonly IndexSearcher indexSearcher;

        public BeerController()
        {
            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(ConfigurationManager.AppSettings["IndexPath"]);

            indexSearcher = new IndexSearcher(dir);
        }

        public BeerReview GetById(string beerId)
        {
            var searchTerm = new Term("beerBeerId", beerId);
            Query query = new TermQuery(searchTerm);

            var hits = indexSearcher.Search(query, 1);

            var hit = hits.ScoreDocs.FirstOrDefault();

            if (hit == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var beer = BeerReviewFromDoc(hit);

            return beer;
        }

        private BeerReview BeerReviewFromDoc(ScoreDoc scoreDoc)
        {
            Document doc = indexSearcher.Doc(scoreDoc.Doc);

            var beer = new BeerReview
            {
                Beer = new Beer
                {
                    BeerName = doc.Get("beerName"),
                    BeerBeerId = doc.Get("beerBeerId"),
                    BeerBrewerId = doc.Get("beerBrewerId"),
                    BeerAbv = doc.Get("beerABV"),
                    BeerStyle = doc.Get("beerStyle")
                },
                Review = new Review
                {
                    ReviewAppearance = doc.Get("reviewAppearance"),
                    ReviewAroma = doc.Get("reviewAroma"),
                    ReviewPalate = doc.Get("reviewPalate"),
                    ReviewTaste = doc.Get("reviewTaste"),
                    ReviewOverall = doc.Get("reviewOverall"),
                    ReviewTime = doc.Get("reviewTime"),
                    ReviewProfileName = doc.Get("reviewProfileName"),
                    ReviewText = doc.Get("reviewText"),
                },
                Score = scoreDoc.Score
            };
            return beer;
        }

        public IEnumerable<BeerReview> Get(string keyword, int limit = 10)
        {
            var searchTerm = new Term("reviewText", keyword);
            Query query = new TermQuery(searchTerm);

            var hits = indexSearcher.Search(query, limit);

            var beers = new List<BeerReview>();

            for (int i = 0; i < hits.ScoreDocs.Count(); i++)
            {
                Document mydoc = indexSearcher.Doc(hits.ScoreDocs[i].Doc);

                beers.Add(BeerReviewFromDoc(mydoc));
            }

            return beers;
        }
    }
}
