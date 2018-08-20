namespace VehicleCostsMonitor.Tests.Helpers
{
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using VehicleCostsMonitor.Models;

    public class MockGenerator
    {
        public static Mock<UserManager<User>> UserManagerMock
            => new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        public static Mock<RoleManager<IdentityRole>> RoleManagerMock
            => new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
    }
}
