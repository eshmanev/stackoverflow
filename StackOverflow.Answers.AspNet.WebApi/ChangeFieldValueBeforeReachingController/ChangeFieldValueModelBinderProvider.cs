using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StackOverflow.Answers.AspNet.WebApi.ChangeFieldValueBeforeReachingController
{
    public class ChangeFieldValueModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            return new ChangeFieldValueModelBinder();
        }
    }
}
