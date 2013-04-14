using System.Collections.Generic;

namespace ClopyRightWeb.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            MinAppearance = 2;
            MinAroma = 2;
            MinPalate = 2;
            MinTaste = 2;
            MinOverall = 2;
        }

        public string Keyword { get; set; }

        public IEnumerable<BeerRecommendation> BeerRecommendations { get; set; }

        public double MinAppearance { get; set; }

        public double MinAroma { get; set; }

        public double MinPalate { get; set; }

        public double MinTaste { get; set; }

        public double MinOverall { get; set; }
    }
}