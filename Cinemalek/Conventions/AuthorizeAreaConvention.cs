
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Cinemalek.Conventions
{
    public class AuthorizeAreaConvention : IControllerModelConvention
    {
        private string _v;
        private string _roles;

        public AuthorizeAreaConvention(string v , string roles)
        {
            _v = v;
            _roles = roles;
        }

        public void Apply(ControllerModel controller)
        {
            if (controller.RouteValues.TryGetValue("area", out var routeValues) && routeValues == _v)
            {
                if (!string.IsNullOrEmpty(_roles))
                    controller.Filters.Add(new AuthorizeFilter());
                else
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireRole( _roles )
                        .Build();

                    controller.Filters.Add(new AuthorizeFilter(policy));
                }
                
            }
        }
    }
}
