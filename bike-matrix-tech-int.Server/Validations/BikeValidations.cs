using System.ComponentModel.DataAnnotations;

namespace bike_matrix_tech_int.Server.Validations
{
    
    // Define some bike data to support - Ideally this would come from databases so it could easily be updated
    public static class BikeValidationData
    {
        public static readonly Dictionary<string, Dictionary<string, string[]>> BrandModelYearMap = new()
        {
            {
                "Canyon", new Dictionary<string, string[]>
                {
                    { "Dude", new[] { "2020", "2024"} },
                    { "Exceed", new[] { "2020", "2024" } }
                }
            },
            {
                "Giant", new Dictionary<string, string[]>
                {
                    { "Defy", new[] { "2020", "2024" } },
                    { "Escape", new[] { "2020", "2024" } }
                }
            },
            {
                "Trek", new Dictionary<string, string[]>
                {
                    { "Boone", new[] { "2020", "2024" } },
                    { "District", new[] { "2020", "2024" } }
                }
            }
        };
    }

    public class BrandValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Check brand is in validation data
            if (value is string brand && BikeValidationData.BrandModelYearMap.ContainsKey(brand))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Brand must be one of: {string.Join(", ", BikeValidationData.BrandModelYearMap.Keys)}");
        }
    }

    public class ModelValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value is not string model)
            {
                return new ValidationResult("Model must be a string.");
            }

            var brandProperty = validationContext.ObjectType.GetProperty("Brand");
            if (brandProperty == null)
            {
                return new ValidationResult("Brand property not found.");
            }

            // Check the brand exists before checking the model
            var brandValue = brandProperty.GetValue(validationContext.ObjectInstance) as string;
            if (string.IsNullOrEmpty(brandValue) || !BikeValidationData.BrandModelYearMap.ContainsKey(brandValue))
            {
                return new ValidationResult("Invalid or missing brand.");
            }

            if (!BikeValidationData.BrandModelYearMap[brandValue].ContainsKey(model))
            {
                var validModels = BikeValidationData.BrandModelYearMap[brandValue].Keys;
                return new ValidationResult($"Model for brand '{brandValue}' must be one of: {string.Join(", ", validModels)}");
            }

            return ValidationResult.Success;
        }
    }

    public class YearValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string year)
            {
                return new ValidationResult("Year must be a string.");
            }

            var brandProp = validationContext.ObjectType.GetProperty("Brand");
            var modelProp = validationContext.ObjectType.GetProperty("Model");
            if (brandProp == null || modelProp == null)
            {
                return new ValidationResult("Brand or Model property not found.");
            }

            var brand = brandProp.GetValue(validationContext.ObjectInstance) as string;
            var model = modelProp.GetValue(validationContext.ObjectInstance) as string;
            if (string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) ||
                !BikeValidationData.BrandModelYearMap.TryGetValue(brand, out var models) ||
                !models.TryGetValue(model, out var validYears))
            {
                return new ValidationResult("Invalid brand or model for year validation.");
            }

            // Using the previously gotten validYears - Validate the year for the brand / model
            if (!validYears.Contains(year))
            {
                return new ValidationResult($"Year for {brand} {model} must be one of: {string.Join(", ", validYears)}");
            }

            return ValidationResult.Success;
        }
    }
}
