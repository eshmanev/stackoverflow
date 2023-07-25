using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication2.Controllers
{
    public class ChangeFieldValueModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            return new ChangeFieldValueModelBinder();
        }
    }
}
