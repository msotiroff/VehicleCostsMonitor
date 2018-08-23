namespace VehicleCostsMonitor.Common.Notifications
{
    public class NotificationMessages
    {
        public const string InvalidOperation = "Invalid operation!";

        public const string UserAddedToRole = "User \"{0}\" successfully added to role \"{1}\"";
        public const string UserRemovedFromRole = "User \"{0}\" successfully removed from role \"{1}\"";
        public const string UnableToRemoveSelf = "You can not remove yourself from role {0}!";

        public const string VehicleDoesNotExist = "Vehicle you've searched does not exist!";
        public const string VehicleUpdatedSuccessfull = "Vehicle edited successfully";
        public const string VehicleDeletedSuccessfull = "Vehicle deleted successfully";

        public const string ManufacturerDoesNotExist = "Manufacturer with id \"{0}\" does not exist!";
        public const string ManufacturerCreatedSuccessfull = "Manufacturer \"{0}\" created successfully";
        public const string ManufacturerUpdatedSuccessfull = "Manufacturer \"{0}\" updated successfully";
        public const string ManufacturerDeletedSuccessfull = "Manufacturer \"{0}\" deleted successfully";
        
        public const string ModelCreatedSuccessfull = "Model \"{0}\" created successfully";
        public const string ModelUpdatedSuccessfull = "Model \"{0}\" updated successfully";
        public const string ModelDeletedSuccessfull = "Model \"{0}\" deleted successfully";

        public const string NoFileChosen = "No file chosen! Please, select a file!";
        public const string FileIsTooLarge = "The file you are trying to upload is too large. Maximum file size is {0} MB";
        public const string PictureUpdatedSuccessfull = "Vehicle picture updated successfully!";
        public const string PictureUploadFailed = "Picture uploading failed";
        public const string InvalidFileFormat = "Invalid file format!";

        public const string LogDoesNotExist = "Log with id \"{0}\" does not exist!";

        public const string CostEntryUpdateFailed = "Cost entry update failed";
        public const string CostEntryDeleteFailed = "Cost entry delete failed";
        public const string CostEntryAddedSuccessfull = "You successfully add new cost entry to your vehicle";
        public const string CostEntryUpdatedSuccessfull = "You successfully update the cost entry";
        public const string CostEntryDeletedSuccessfull = "You successfully delete the cost entry";
        public const string CostEntryDoesNotExist = "Cost entry you are trying to access does not exist!";

        public const string FuelEntryUpdateFailed = "Fuel entry update failed";
        public const string FuelEntryDeleteFailed = "Fuel entry delete failed";
        public const string FuelEntryAddedSuccessfull = "You successfully add new Fuel entry to your vehicle";
        public const string FuelEntryUpdatedSuccessfull = "You successfully update the Fuel entry";
        public const string FuelEntryDeletedSuccessfull = "You successfully delete the Fuel entry";
        public const string FuelEntryDoesNotExist = "Fuel entry you are trying to access does not exist!";
    }
}
