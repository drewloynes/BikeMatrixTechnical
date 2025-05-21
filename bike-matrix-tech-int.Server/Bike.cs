using bike_matrix_tech_int.Server.Validations;
using System.ComponentModel.DataAnnotations;

namespace bike_matrix_tech_int.Server
{
    public class Bike
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, BrandValidation]
        public string Brand { get; set; } = "";

        [Required, ModelValidation]
        public string Model { get; set; } = "";

        [Required, YearValidation]
        public string Year { get; set; } = "";
    }
}
