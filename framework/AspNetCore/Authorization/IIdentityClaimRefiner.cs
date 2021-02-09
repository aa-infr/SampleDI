using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Authorization
{
    public interface IIdentityClaimRefiner
    {
        ValueTask RefineAsync(ClaimsIdentity identity);
    }
}