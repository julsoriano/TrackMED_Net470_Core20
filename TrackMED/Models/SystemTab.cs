using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrackMED.Models
{
    public class SystemTab: Equipment
    {
        [Required(ErrorMessage = "A system description is required")]
        [JsonProperty(PropertyName = "systemsDescriptionID")]
        public string SystemsDescriptionID { get; set; }

        [JsonProperty(PropertyName = "locationID")]
        public string LocationID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yy}")]
        [Display(Name = "Deployment Date")]
        [JsonProperty(PropertyName = "deploymentDate")]
        public DateTime? DeploymentDate { get; set; }

        [Display(Name = "Reference No.")]
        [JsonProperty(PropertyName = "referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty(PropertyName = "systemsDescription")]
        public virtual SystemsDescription SystemsDescription { get; set; }

        [JsonProperty(PropertyName = "location")]
        public virtual Location Location { get; set; }

        public List<string> leftComponents { get; set; }
        public List<string> rightComponents { get; set; }
    }
}