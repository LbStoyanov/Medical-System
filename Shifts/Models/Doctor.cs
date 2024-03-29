using System.ComponentModel.DataAnnotations;

namespace Shifts.Models
{
    public class Doctor
    {
        public Doctor()
        {
            this.DoctorSpecialties = new List<DoctorSpecialties>();
            this.Shifts = new List<Shift>();
        }

        [Key]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "You should enter your first name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "You should enter your last name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "You should enter your address")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "You should enter your phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "You should enter your email address")]
        [EmailAddress(ErrorMessage = "This is not a valid email address")]
        public string Email { get; set; } = null!;

        [Display(Name = "Working hours from")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime WorkingHoursFrom { get; set; }

        [Display(Name = "Working hours to")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime WorkingHoursTo { get; set; }

        public List<DoctorSpecialties> DoctorSpecialties { get; set; }

        public List<Shift> Shifts { get; set; }
    }
}