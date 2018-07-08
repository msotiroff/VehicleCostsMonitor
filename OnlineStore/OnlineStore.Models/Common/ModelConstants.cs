namespace OnlineStore.Models.Common
{
    public class ModelConstants
    {
        public const string CategoryListPath = @"OnlineStore.Models\Common\CategoryList.json";

        public const int CategoryNameMinLength = 3;

        public const int FeedbackContentMinLength = 5;
        public const int FeedbackTitleMinLength = 3;
        public const int FeedbackTitleMaxLength = 30;

        public const int CustomerNameMinLength = 2;

        public const int ProductNameMinLength = 3;

        public const string ProductPriceMinValueAsString = "0.01";

        public const string ProductPriceMaxValueAsString = "1000000.0";
    }
}
