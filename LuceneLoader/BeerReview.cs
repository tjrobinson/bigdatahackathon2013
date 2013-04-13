namespace LuceneLoader
{
    public class BeerReview
    {
        public BeerReview()
        {
            Beer = new Beer();
            Review = new Review();
        }
        public Beer Beer { get; set; }
        public Review Review { get; set; }
        public float Score { get; set; }
    }
}
