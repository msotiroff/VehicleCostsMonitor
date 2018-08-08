namespace VehicleCostsMonitor.Web
{
    public class WebConstants
    {
        public const string AdministratorRole = "Admin";
        public const string AdminEmail = "admin@justmonitor.com";
        public const string AdminUserName = "Administrator";
        public const string AdminPassword = "admin123";

        public const string AdministratorArea = "Admin";
        public const string UserArea = "User";
        public const string VehicleArea = "Vehicle";

        public const string PaginationPartial = @"~/Views/Shared/_PaginationElementsPartial.cshtml";

        public const string VehicleImagePathBase = @"\images\vehicles\{0}";
        public const string DecimalNumberFormat = "f2";

        public const int PictureMaxHeightSize = 250;
        public const int PictureSizeLimit = 10485760;
        public const int ConsumptionHistogramRangesCount = 5;
        public const int MileageByDateItemsCount = 12;
        
        public const string ManufacturersListPath = @"Resourses\seedfiles\vehicles-list.json";
        public const string CostEntryTypesListPath = @"Resourses\seedfiles\cost-entry-types.json";
        public const string ExtraFuelConsumersListPath = @"Resourses\seedfiles\extra-fuel-consumers.json";
        public const string FuelEntryTypesListPath = @"Resourses\seedfiles\fuel-entry-types.json";
        public const string FuelTypesListPath = @"Resourses\seedfiles\fuel-types.json";
        public const string GearingTypesListPath = @"Resourses\seedfiles\gearing-types.json";
        public const string RouteTypesListPath = @"Resourses\seedfiles\route-types.json";
        public const string VehicleTypesListPath = @"Resourses\seedfiles\vehicle-types.json";
        public const string UsersListPath = @"Resourses\seedfiles\users-list.json";
        public const string CurrenciesListPath = @"Resourses\seedfiles\currencies.json";

        public const int UsersListPageSize = 20;
        public const int LogsListPageSize = 20;
        public const int EntriesListPageSize = 20;
        public const int SearchResultsPageSize = 15;

        public const int StaticElementsCacheExpirationInDays = 7;

        public const string BadRequestErrorMsg = "Ops! Something went wrong while processing your request!";
    }
}
