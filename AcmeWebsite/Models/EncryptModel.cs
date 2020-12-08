using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcmeWebsite.Models
{
    public class EncryptModel
    {
        [Required]
        //    [DataType(DataType.Password)]
        [Display(Name = "Key Password")]
        public string KeyPassword { get; set; }
        [Display(Name = "Plain Text")]
        public string PlainText { get; set; }
        [Display(Name = "Encrypted Text")]
        public string CipherText { get; set; }
        [Display(Name = "Hash Code")]
        public string HashCode { get; set; }
    }
}
