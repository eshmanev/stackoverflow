using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace WebApplication2.Controllers
{
    public class ChangeFieldValueModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                var json = await reader.ReadToEndAsync();
                var doc = JsonSerializer.Deserialize<JsonDocument>(json);
                if (!doc.RootElement.TryGetProperty("SomeId", out JsonElement someIdElement))
                    throw new BadHttpRequestException("No SomeId in the request.");

                var model = doc.Deserialize(bindingContext.ModelType);
                TryToUpdateSomeId(model);
                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }

        private void TryToUpdateSomeId(object? model)
        {
            if (model == null) return;
            var propInfo = model.GetType().GetProperty("SomeId");
            if (propInfo == null)
                return;

            var getMethod = propInfo.GetGetMethod();
            var setMethod = propInfo.GetSetMethod();
            if (getMethod == null || setMethod == null)
                return;

            var currentValue = (long)getMethod.Invoke(model, null);
            setMethod?.Invoke(model, new object[] { currentValue + 100 });
        }
    }
}
