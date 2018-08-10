namespace OnlineStore.Services.Implementations
{
    using AutoMapper;
    using OnlineStore.Data;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseService
    {
        public BaseService(OnlineStoreDbContext dbContext, IMapper mapper)
        {
            this.DbContext = dbContext;
            this.Mapper = mapper;
        }

        public OnlineStoreDbContext DbContext { get; }

        public IMapper Mapper { get; }

        protected bool ValidateEntityState(object model)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            return isValid;
        }
    }
}