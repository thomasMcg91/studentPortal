using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SMS.Web.Extensions
{

    public class ClassificationTagHelper : TagHelper
    {
        //[HtmlAttributeName("asp-grade")]
        public double Grade { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";    // Replaces <classification> with <span> tag
              
            // ToClassification returns a tuple and we assign both values in the tuple to separate named variables    
            var (classification,badge) = ToClassification(Grade); 
                    
            output.Attributes.SetAttribute("class", "badge badge-" + badge);
            output.Content.SetContent($"({classification})");
        }

        // private utility method to convert grade to Classification
        private (string,string) ToClassification(double grade)
        {
            switch (grade)
            {
                case double s when s >= 70: return ("Distinction","success");
                case double s when s >= 60: return ("Commendation","info");
                case double s when s >= 50: return ("Pass","warning");
                default: return ("Fail", "danger");
            }
        }
    }
}
