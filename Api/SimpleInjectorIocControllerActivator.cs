using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleInjector;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ICT.Template.Api
{
  public class SimpleInjectorIocControllerActivator : IControllerActivator
  {

    private readonly Container container;

    public SimpleInjectorIocControllerActivator(Container container)
    {
      if (container == null)
      {
        throw new ArgumentNullException("container");
      }

      this.container = container;
    }
    public object Create(ControllerContext context)
    {
      Type type = context.ActionDescriptor.ControllerTypeInfo.AsType();
      return this.container.GetInstance(type);
    }

    public void Release(ControllerContext context, object controller)
    {


    }
  }
}
