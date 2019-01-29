using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ProdFloor.Infrastructure
{
    [HtmlTargetElement("page-doorop", Attributes = "page-model")]
    public class PageLinkTagHelperDoorOp : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelperDoorOp(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("select-countries", Attributes = "page-model")]
    public class PageLinkTagHelperSelectCountries : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        private IItemRepository repository;

        public PageLinkTagHelperSelectCountries(IUrlHelperFactory helperFactory, IItemRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public string CurrentCountry { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("select");
            foreach (Country country in repository.Countries)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["Value"] = country.Name;
                if (country.Name == CurrentCountry)
                {
                    tag.InnerHtml.Append("Selected");
                }
                result.InnerHtml.Append(tag.ToString());
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("page-all", Attributes = "page-model")]
    public class PageLinkTagHelperAll : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelperAll(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public bool DashBoardEnabled { get; set; } = false;
        public PagingInfo AdditionalPageModel { get; set; }
        public string MainUrlValue { get; set; }
        public string AddUrlValue { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (DashBoardEnabled)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues[MainUrlValue] = i;
                    PageUrlValues[AddUrlValue] = AdditionalPageModel.CurrentPage;
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                    }
                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
            else
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues["jobPage"] = i;
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                    }
                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }

    public class MySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public MySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            foreach (string style in itemsrepository.DoorOperators.Select(d => d.Style).Distinct())
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = style;
                if (style == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(style);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class CustomSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public CustomSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "FireCode":
                    return itemsrepository.FireCodes.Select(d => d.Name).Distinct();
                case "State":
                    return itemsrepository.States.Select(d => d.Name).Distinct();
                case "Country":
                    return itemsrepository.Countries.Select(d => d.Name).Distinct();
                case "City":
                    return itemsrepository.Cities.Select(d => d.Name).Distinct();
                case "DoorOperatorName":
                    return itemsrepository.DoorOperators.Select(d => d.Name).Distinct();
                case "DoorOperatorBrand":
                    return itemsrepository.DoorOperators.Select(d => d.Brand).Distinct();
                case "DoorOperatorStyle":
                    return itemsrepository.DoorOperators.Select(d => d.Style).Distinct();
                case "LandingSystems":
                    return itemsrepository.LandingSystems.Select(d => d.Name).Distinct();
                case "JobType":
                    return itemsrepository.JobTypes.Select(d => d.Name).Distinct();
                case "SwitchStyle":
                    return new List<string> { "2-Position", "3-Position" }.AsQueryable();
                default:
                    return itemsrepository.JobTypes.Select(d => d.Name).Distinct();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options2 = CaseFor(SelectFor);
            foreach (string option in options2)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option.ToString();
                if (option.ToString() == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class IndicatorsTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public IndicatorsTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts_Value":
                    return new List<string> { "24", "48", "120", "Other" }.AsQueryable();
                case "Volts_Type":
                    return new List<string> { "AC", "DC" }.AsQueryable();
                case "CallType":
                    return new List<string> { "LED", "Incandescent" }.AsQueryable();
                case "EPContact":
                    return new List<string> { "NO", "NC" }.AsQueryable();
                case "EPCars":
                    return new List<string> { "1", "2", "3", "4" }.AsQueryable();
                case "PIDriver":
                    return new List<string> { "CE Electronics", "Emotive", "Discrete" }.AsQueryable();
                case "CarPIDiscreteType":
                    return new List<string> { "Multi-light", "One line per floor", "Binary 00", "Binary 01" }.AsQueryable();
                case "Monitoring":
                    return new List<string> { "MView Complete", "MView Interface", "IMonitor Complete", "IMonitor Interface", "IDS Liftnet" }.AsQueryable();
                case "PIType":
                    return new List<string> { "Chime", "Gong" }.AsQueryable();
                case "AccessSWLocation":
                    return new List<string> { "Front", "Rear" }.AsQueryable();
                case "INCPButtons":
                    return new List<string> { "Using top/bottom car calls", "Using up/down buttons" }.AsQueryable();
                case "JobType":
                    return new List<string> { "Simplex", "Duplex", "Group" }.AsQueryable();
                case "JobType2":
                    return new List<string> { "Selective Collective", "Duplex Selective Collective", "Group Operation" }.AsQueryable();
                default:
                    return new List<string> { "Chime", "Gong" }.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("---");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options = CaseFor(SelectFor);
            foreach (string option in options)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option;
                if (option == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class VoltsTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public VoltsTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        private IQueryable<int> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts_Value":
                    return new List<int> { 24, 48, 120 }.AsQueryable();
                default:
                    return new List<int> { 24, 48, 120 }.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("---");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<int> options = CaseFor(SelectFor);
            foreach (int option in options)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option.ToString();
                if (option == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
