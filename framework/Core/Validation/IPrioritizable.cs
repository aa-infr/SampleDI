using System;

namespace Infrabel.ICT.Framework.Validation
{
  public interface IPrioritizable : IComparable<IPrioritizable>
  {
    int Priority { get; }
  }
}