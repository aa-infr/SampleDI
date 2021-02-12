using System;
using System.Collections.Generic;
using System.Text;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  /// <summary>Adds to Container support for:
  /// <list type="bullet">
  /// <item>Open-generic services</item>
  /// <item>Service generics wrappers and arrays using <see cref="Rules.UnknownServiceResolvers"/> extension point.
  /// Supported wrappers include: Func of <see cref="FuncTypes"/>, Lazy, Many, IEnumerable, arrays, Meta, KeyValuePair, DebugExpression.
  /// All wrapper factories are added into collection of <see cref="Wrappers"/>.
  /// unregistered resolution rule.</item>
  /// </list></summary>
  public static class WrappersSupport
  {
    /// <summary>Supported open-generic collection types - all the interfaces implemented by array.</summary>
    public static readonly Type[] SupportedCollectionTypes =
        typeof(object[]).GetImplementedInterfaces().Match(t => t.IsGeneric(), t => t.GetGenericTypeDefinition());
  }
}
