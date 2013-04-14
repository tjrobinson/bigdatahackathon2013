using System;
using System.Collections;
using System.Reflection;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using Lucene.Net.Util;

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

        public static int GetCardinality(BitArray bitArray)
        {
            var _bitsSetArray256 = new byte[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8 };
            var array = (uint[])bitArray.GetType().GetField("m_array", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bitArray);
            int count = 0;

            for (int index = 0; index < array.Length; index++)
                count += _bitsSetArray256[array[index] & 0xFF] + _bitsSetArray256[(array[index] >> 8) & 0xFF] + _bitsSetArray256[(array[index] >> 16) & 0xFF] + _bitsSetArray256[(array[index] >> 24) & 0xFF];

            return count;
        }

        public BeerReview GetFacets(string searchString)
        {
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "reviewText", new
            StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30)).Parse(searchString);

            // pass in the reader and the names of the facets that you've created using fields in the documents.
            // the facets are determined


            // first get the BitArray result from the genre query
            //var genreQuery = new TermQuery(new Term("genre", genre));
            //var genreQueryFilter = new QueryFilter(genreQuery);
            //BitArray genreBitArray = genreQueryFilter.Bits(searcher.GetIndexReader());


            //Filter filter = new QueryFil

            //Console.WriteLine("There are " + GetCardinality(genreBitArray) + " document with the genre " + genre);




            SimpleFacetedSearch sfs = new SimpleFacetedSearch(indexSearcher.IndexReader, new string[] { "reviewOverall", "reviewAroma" });


            // then pass in the query into the search like you normally would with a typical search class.

            SimpleFacetedSearch.Hits hits = sfs.Search(query, 10);

            // what comes back is different than normal.
            // the result documents & hits are grouped by facets.

            // you'll need to iterate over groups of hits-per-facet.

            long totalHits = hits.TotalHitCount;
            foreach (SimpleFacetedSearch.HitsPerFacet hpg in hits.HitsPerFacet)
            {
                long hitCountPerGroup = hpg.HitCount;
                SimpleFacetedSearch.FacetName facetName = hpg.Name;
                for (int i = 0; i < facetName.Length; i++)
                {
                    string part = facetName[i];
                }
                foreach (Document doc in hpg.Documents)
                {
                    string text = doc.GetField("reviewOverall").StringValue;
                    System.Diagnostics.Debug.WriteLine(">>" + facetName + ": " + text);
                }
            }
            return null;
        }

        public IEnumerable<BeerReview> Get(string reviewText = null, int limit = 10, double minAroma = 0, double minAppearance = 0, double minOverall = 0, double minTaste = 0, double minPalate = 0)
        {
            var query = new BooleanQuery();
            Query reviewTextQuery = new TermQuery(new Term("reviewText", reviewText));
            Query reviewAppearanceQuery = NumericRangeQuery.NewDoubleRange("reviewAppearance", minAppearance, null, minInclusive: true, maxInclusive: true);
            Query reviewAromaQuery = NumericRangeQuery.NewDoubleRange("reviewAroma", minAroma, null, minInclusive: true, maxInclusive: true);
            Query reviewPalateQuery = NumericRangeQuery.NewDoubleRange("reviewPalate", minPalate, null, minInclusive: true, maxInclusive: true);
            Query reviewTasteQuery = NumericRangeQuery.NewDoubleRange("reviewTaste", minTaste, null, minInclusive: true, maxInclusive: true);
            Query reviewOverallQuery = NumericRangeQuery.NewDoubleRange("reviewOverall", minOverall, null, minInclusive: true, maxInclusive: true);

            if (reviewText != null) { query.Add(reviewTextQuery, Occur.MUST); }
            query.Add(reviewAppearanceQuery, Occur.MUST);
            query.Add(reviewAromaQuery, Occur.MUST);
            query.Add(reviewOverallQuery, Occur.MUST);
            query.Add(reviewPalateQuery, Occur.MUST);
            query.Add(reviewTasteQuery, Occur.MUST);

            var hits = indexSearcher.Search(query, limit);

            var beers = new List<BeerReview>();

            for (int i = 0; i < hits.ScoreDocs.Count(); i++)
            {
                beers.Add(BeerReviewFromDoc(hits.ScoreDocs[i]));
            }

            return beers;
        }

        private static double? SafeDoubleParse(string input)
        {
            double output;
            if (double.TryParse(input, out output))
            {
                return output;
            }
            else
            {
                return null;
            }
        }

        private BeerReview BeerReviewFromDoc(ScoreDoc scoreDoc)
        {
            Document doc = indexSearcher.Doc(scoreDoc.Doc);

            double? beerAbv = null;

            int? beerBeerId = null;
            int? beerBrewerId = null;
            
            var beer = new BeerReview
            {
                Beer = new Beer
                {
                    BeerName = doc.Get("beerName"),
                    BeerStyle = doc.Get("beerStyle")
                },
                Review = new Review
                {
                    ReviewAppearance = SafeDoubleParse(doc.Get("reviewAppearance")),
                    ReviewAroma = SafeDoubleParse(doc.Get("reviewAroma")),
                    ReviewPalate = SafeDoubleParse(doc.Get("reviewPalate")),
                    ReviewTaste = SafeDoubleParse(doc.Get("reviewTaste")),
                    ReviewOverall = SafeDoubleParse(doc.Get("reviewOverall")),
                    ReviewTime = doc.Get("reviewTime"),
                    ReviewProfileName = doc.Get("reviewProfileName"),
                    ReviewText = doc.Get("reviewText"),
                },
                Score = scoreDoc.Score
            };

            if (!string.IsNullOrWhiteSpace(doc.Get("beerABV")))
            {
                beer.Beer.BeerAbv = beerAbv;
            }

            if (!string.IsNullOrWhiteSpace(doc.Get("beerBeerId")))
            {
                beer.Beer.BeerBeerId = beerBeerId;
            }

            if (!string.IsNullOrWhiteSpace(doc.Get("beerBrewerId")))
            {
                beer.Beer.BeerBrewerId = beerBrewerId;
            }

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
                beers.Add(BeerReviewFromDoc(hits.ScoreDocs[i]));
            }

            return beers;
        }
    }
}
