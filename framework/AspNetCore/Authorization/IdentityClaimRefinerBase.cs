using Infrabel.ICT.Framework.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Authorization
{
  public abstract class IdentityClaimRefinerBase : IIdentityClaimRefiner
  {
    public abstract int Priority { get; }

    public int CompareTo(IPrioritizable other)
    {
      return Priority.CompareTo(other?.Priority ?? int.MaxValue);
    }

    public abstract ValueTask RefineAsync(ClaimsIdentity identity);
  }
}