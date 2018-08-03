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
        public const int PictureMaxHeightSize = 250;
        public const string DecimalNumberFormat = "f2";

        public const string SqlScriptsDirectoryPath = @"wwwroot\seedfiles\sql-scripts";
        public const string ManufacturersListPath = @"wwwroot\seedfiles\vehicles-list.json";
        public const string CostEntryTypesListPath = @"wwwroot\seedfiles\cost-entry-types.json";
        public const string ExtraFuelConsumersListPath = @"wwwroot\seedfiles\extra-fuel-consumers.json";
        public const string FuelEntryTypesListPath = @"wwwroot\seedfiles\fuel-entry-types.json";
        public const string FuelTypesListPath = @"wwwroot\seedfiles\fuel-types.json";
        public const string GearingTypesListPath = @"wwwroot\seedfiles\gearing-types.json";
        public const string RouteTypesListPath = @"wwwroot\seedfiles\route-types.json";
        public const string VehicleTypesListPath = @"wwwroot\seedfiles\vehicle-types.json";
        public const string UsersListPath = @"wwwroot\seedfiles\users-list.json";

        public const int UsersListPageSize = 20;
        public const int LogsListPageSize = 20;
        public const int EntriesListPageSize = 20;
        public const int SearchResultsPageSize = 15;

        public const string BadRequestErrorMsg = "Ops! Something went wrong while processing your request!";
    }
}
