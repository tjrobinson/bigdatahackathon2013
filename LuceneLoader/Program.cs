using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace LuceneLoader
{
    class Program
    {
        private static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                var config = new HttpSelfHostConfiguration("http://localhost:8080");

                config.Routes.MapHttpRoute(
                    "API Default", "api/{controller}/{id}",
                    new { id = RouteParameter.Optional });

                using (var server = new HttpSelfHostServer(config))
                {
                    server.OpenAsync().Wait();
                    Console.WriteLine("Press Enter to quit.");
                    Console.ReadLine();
                }
            });

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "exit")
                {
                    return;
                }
                if (input == "create")
                {
                    CreateIndex(ConfigurationManager.AppSettings["IndexPath"]);
                }
            }
        }

        private static void CreateIndex(string indexFileLocation)
        {
            int limit = Convert.ToInt32(ConfigurationManager.AppSettings["Limit"]);
            int processed = 0;

            var dir = FSDirectory.Open(indexFileLocation);

            var analyzer = new StandardAnalyzer(Version.LUCENE_30);

            var indexWriter = new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            var doc = new Document();

            using (var reader = new StreamReader(@"C:\Data\beeradvocate.txt"))
            {
                var beer = new BeerReview();

                string line;
                while ((line = reader.ReadLine()) != null && processed <= limit)
                {
                    string fieldName = line.Split(':').First();

                    if (!string.IsNullOrEmpty(fieldName))
                    {

                        try
                        {
                            switch (fieldName)
                            {
                                case "beer/name":
                                    beer.Beer.BeerName = line.Replace(fieldName + ": ", string.Empty);
                                    break;
                                case "beer/beerId":
                                    beer.Beer.BeerBeerId = int.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "beer/brewerId":
                                    beer.Beer.BeerBrewerId = int.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "beer/ABV":
                                    beer.Beer.BeerAbv = double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "beer/style":
                                    beer.Beer.BeerStyle = line.Replace(fieldName + ": ", string.Empty);
                                    break;
                                case "review/appearance":
                                    beer.Review.ReviewAppearance =
                                    double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "review/aroma":
                                    beer.Review.ReviewAroma = double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "review/palate":
                                    beer.Review.ReviewPalate = double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "review/taste":
                                    beer.Review.ReviewTaste = double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "review/overall":
                                    beer.Review.ReviewOverall =
                                        double.Parse(line.Replace(fieldName + ": ", string.Empty));
                                    break;
                                case "review/time":
                                    beer.Review.ReviewTime = line.Replace(fieldName + ": ", string.Empty);
                                    break;
                                case "review/profileName":
                                    beer.Review.ReviewProfileName = line.Replace(fieldName + ": ", string.Empty);
                                    break;
                                case "review/text":
                                    beer.Review.ReviewText = line.Replace(fieldName + ": ", string.Empty);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    }
                    else
                    {
                        var beerDocument = new Document();

                        beerDocument.Add(new Field("beerName", beer.Beer.BeerName, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.YES));
                        beerDocument.Add(new NumericField("beerBeerId", Field.Store.YES, true).SetIntValue(beer.Beer.BeerBeerId.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("beerBrewerId", Field.Store.YES, true).SetIntValue(beer.Beer.BeerBrewerId.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("beerABV", Field.Store.YES, true).SetDoubleValue(beer.Beer.BeerAbv.GetValueOrDefault()));
                        beerDocument.Add(new Field("beerStyle", beer.Beer.BeerStyle, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        beerDocument.Add(new NumericField("reviewAppearance", Field.Store.YES, true).SetDoubleValue(beer.Review.ReviewAppearance.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("reviewAroma", Field.Store.YES, true).SetDoubleValue(beer.Review.ReviewAroma.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("reviewPalate", Field.Store.YES, true).SetDoubleValue(beer.Review.ReviewPalate.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("reviewTaste", Field.Store.YES, true).SetDoubleValue(beer.Review.ReviewTaste.GetValueOrDefault()));
                        beerDocument.Add(new NumericField("reviewOverall", Field.Store.YES, true).SetDoubleValue(beer.Review.ReviewOverall.GetValueOrDefault()));
                        beerDocument.Add(new Field("reviewTime", beer.Review.ReviewTime, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        beerDocument.Add(new Field("reviewProfileName", beer.Review.ReviewProfileName, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                        beerDocument.Add(new Field("reviewText", beer.Review.ReviewText, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                        
                        indexWriter.AddDocument(beerDocument);
                        Console.WriteLine("Added {0}", beer.Beer.BeerName);
                        processed++;

                        beer = new BeerReview();
                    }
                }
            }

            indexWriter.AddDocument(doc);

            indexWriter.Optimize();
            indexWriter.Dispose();
        }
    }
}
