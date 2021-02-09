using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ICT.Template.Api
{
  public class DryIocControllerActivator : IControllerActivator
  {

    private readonly IContainer container;

    public DryIocControllerActivator(IContainer container)
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
      return this.container.Resolve(type);
    }

    public void Release(ControllerContext context, object controller)
    {
        //???
        //this.container.Release(controller);
    }
  }
}
