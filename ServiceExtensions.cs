using Microsoft.AspNetCore.Identity;

namespace PublicParkingsSofiaWebAPI
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<IdentityUser>(q => q.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, services);
            builder.AddEntityFrameworkStores<ParkingsContext>().AddDefaultTokenProviders();
        }
    }
}
