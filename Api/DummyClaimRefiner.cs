using System.Security.Claims;
using System.Threading.Tasks;
using Infrabel.ICT.Framework.Extended.AspNetCore.Authorization;
using Infrabel.ICT.Framework.Ioc;

namespace ICT.Template.Api
{
  [IoCRegistration(RegistrationLifeTime.Scoped)]
  public class DummyClaimRefiner : IdentityClaimRefinerBase
  {
    public override int Priority => 0;

    public override ValueTask RefineAsync(ClaimsIdentity identity)
    {
      identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
      return new ValueTask(Task.CompletedTask);
    }
  }
}