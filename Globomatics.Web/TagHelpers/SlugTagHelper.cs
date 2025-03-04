using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Globomatics.Web.TagHelpers;

[HtmlTargetElement("url-with-slug")] // tells that if we find an html element with this exact syntax it/s going to use this tag helper.
public class SlugTagHelper : AnchorTagHelper // inherit from the AnchorTaghelper
{
    public SlugTagHelper(IHtmlGenerator generator) : base(generator) { } //constructor has to take IHtmlGenrator

    //properties that we want to populate based on the html attribute,
    /* if dfined for ticket name the idea is that this should generate a slug inside an  anchor
     */
    [HtmlAttributeName("for-product-id")]
    public Guid ProductId { get; set; }

    [HtmlAttributeName("for-ticket-name")]
    public required string TicketTitle { get; set; }

    //override Process in Anchortaghelerp
    public override void Process(TagHelperContext context,
        TagHelperOutput output)
    {
        output.TagName = "a"; //anchor
        output.TagMode = TagMode.StartTagAndEndTag; //set a specific tag mode

        var slug = Regex.Replace(TicketTitle, @"[^a-zA-Z0-9-]+", " "); //using regex replace to generate a slug based on the ticket title
        slug = slug.Trim().Replace(" ", "-").ToLower(); // trim and replicate any space with a - then lowercasing

        RouteValues.Add("slug", slug); // adding a slug as a route value

        RouteValues.Add("productId", ProductId.ToString()); // adding productId as a route value 

        base.Process(context, output); //return to the base method
    }
}