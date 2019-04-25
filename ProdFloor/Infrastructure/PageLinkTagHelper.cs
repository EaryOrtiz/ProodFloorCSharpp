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
using Microsoft.EntityFrameworkCore;

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

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CustomSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;
        private IJobRepository jobrepository;


        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public CustomSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo, IJobRepository jobrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
            jobrepository = jobrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "SwitchStyle":
                    return new List<string> { "2-Position", "3-Position" }.AsQueryable();
                case "Stage":
                    return new List<string> { "Beginning", "Program", "Logic", "Ending", "Complete" }.AsQueryable();
                case "Label":
                    return new List<string> { "-", "A", "B", "C", "D", "E", "F", "G", "H"}.AsQueryable();
                case "Station":
                    return new List<string> { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", "S11", "S12", "ELEM1", "ELEM2" }.AsQueryable();
                case "Style":
                    return itemsrepository.DoorOperators.Select(s => s.Style).Distinct().AsQueryable();
                case "Battery Brand":
                    return  new List<string> { "HAPS", "R&R", "Other" }.AsQueryable();
                case "JobType":
                    return itemsrepository.JobTypes.Select(d => d.Name).Distinct();
                case "Style":
                    return itemsrepository.DoorOperators.Select(d => d.Style).Distinct();
                case "SwitchStyle":
                    return new List<string> { "2-Position", "3-Position" }.AsQueryable();
                case "CarCode":
                    return new List<string> { "Key", "IMonitor" }.AsQueryable();
                case "SPH":
                    return new List<string> { "80", "120" }.AsQueryable();
                case "Starter":
                    return new List<string> { "Siemens SS : 6/12", "Siemens SS : 3/9", "Sprecher SS : 6/12", "Sprecher SS : 3/9", "ATL", "YD" }.AsQueryable();
                case "Valve Brand":
                    return new List<string> { "Maxton", "Blain", "EECO", "TKE | Dover", "Bucher", "Other" }.AsQueryable();
                case "Battery Brand":
                    List<string> BatteryInHydro = jobrepository.HydroSpecifics.Where(d => d.BatteryBrand != null).Select(d => d.BatteryBrand).Distinct().ToList();
                    List<string> BatteryList = new List<string> { "HAPS", "R&R", "Other" };
                    if(BatteryInHydro.Count > 0) BatteryList.AddRange(BatteryInHydro);

                    return BatteryList.Distinct().AsQueryable();
                default:
                    return new List<string> { "Beginning", "Program", "Logic", "Ending", "Complete" }.AsQueryable();
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
                if (!string.IsNullOrEmpty(SelectedValue))
                {
                    if (option.ToString() == SelectedValue)
                    {
                        tag.Attributes["selected"] = "selected";
                    }
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
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

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
                    return new List<string> { "Selective Collective", "SAPB Single Automatic Pushbutton", "SBC Single Button Collective", "Duplex Operation", "Group Operation" }.AsQueryable();
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
            m_tag.InnerHtml.Append(" --- Please Select one--- ");
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
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
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

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CitySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public CitySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a City");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<City> city = itemsrepository.Cities.AsQueryable();
            foreach (City cities in city)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = cities.CityID.ToString();
                if (cities.CityID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(cities.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class StateSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public StateSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("-- Select State --");
            result.InnerHtml.AppendHtml(m_tag);
            int stateID = 0;
            if (SelectedValue != 0)
            {
                City selectedCity = itemsrepository.Cities.FirstOrDefault(c => c.CityID == SelectedValue);
                State selectedState = itemsrepository.States.FirstOrDefault(s => s.StateID == selectedCity.StateID);
                stateID = selectedCity.StateID;
            }
            IQueryable<State> state = itemsrepository.States.AsQueryable();
            foreach (State states in state)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = states.StateID.ToString();
                if (states.StateID == stateID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(states.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CountrySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public CountrySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public int SelectedDefaultValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }


            
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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Country --");
            result.InnerHtml.AppendHtml(m_tag);
            int countryID = 0;
            if (SelectedValue != 0)
            {
                City selectedCity = itemsrepository.Cities.FirstOrDefault(c => c.CityID == SelectedValue);
                State selectedState = itemsrepository.States.FirstOrDefault(s => s.StateID == selectedCity.StateID);
                countryID = selectedState.CountryID;
            }
            if (SelectedDefaultValue != 0)
            {
                Country selectedCountry = itemsrepository.Countries.FirstOrDefault(s => s.CountryID == SelectedDefaultValue);
                countryID = selectedCountry.CountryID;
            }
            IQueryable<Country> country = itemsrepository.Countries.AsQueryable();
            foreach (Country countries in country)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = countries.CountryID.ToString();
                if (countries.CountryID == countryID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(countries.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class FireCodeSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public FireCodeSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a FireCode");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<FireCode> fireCode = itemsrepository.FireCodes.AsQueryable();
            foreach (FireCode fireCodes in fireCode)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = fireCodes.FireCodeID.ToString();
                if (fireCodes.FireCodeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(fireCodes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class FireCodeInJobsSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public FireCodeInJobsSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a FireCode");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<FireCode> fireCode = itemsrepository.FireCodes.FromSql("select * from dbo.FireCodes " +
                "Where dbo.FireCodes.FireCodeID IN (Select dbo.Jobs.FireCodeID From dbo.Jobs)").AsQueryable();
            foreach (FireCode fireCodes in fireCode)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = fireCodes.FireCodeID.ToString();
                if (fireCodes.FireCodeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(fireCodes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class LandingSysSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public LandingSysSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public int SelectFor { get; set; }

        
        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            var jobtypeName = itemsrepository.JobTypes.FirstOrDefault(m => m.JobTypeID == SelectFor).Name;
            IQueryable<LandingSystem> landigSystems = itemsrepository.LandingSystems.Where(m => m.UsedIn == jobtypeName).AsQueryable();
            foreach (LandingSystem landingSys in landigSystems)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = landingSys.LandingSystemID.ToString();
                if (landingSys.LandingSystemID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(landingSys.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class DoorOperatorSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            IQueryable<DoorOperator> door = itemsrepository.DoorOperators.AsQueryable();
            foreach (DoorOperator doors in door)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = doors.DoorOperatorID.ToString();
                if (doors.DoorOperatorID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(doors.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class DoorOperatorStyleSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorStyleSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a Style");
            result.InnerHtml.AppendHtml(m_tag);
            int doorOperatorID = 0;
            string doorStyle = "";
            if (SelectedValue != 0)
            {
                DoorOperator selectedDoor = itemsrepository.DoorOperators.FirstOrDefault(c => c.DoorOperatorID == SelectedValue);
                doorOperatorID = selectedDoor.DoorOperatorID;
                doorStyle = selectedDoor.Style;
            }
            IQueryable<DoorOperator> door = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style)").AsQueryable();
            foreach (DoorOperator doors in door)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = doors.Style;
                if (doors.Style == doorStyle)
                {
                    tag.Attributes["selected"] = doors.Style;
                }
                tag.InnerHtml.Append(doors.Style);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class DoorOperatorBrandSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorBrandSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a Brand");
            result.InnerHtml.AppendHtml(m_tag);
            int doorOperatorID = 0;
            if (SelectedValue != 0)
            {
                DoorOperator selectedDoor = itemsrepository.DoorOperators.FirstOrDefault(c => c.DoorOperatorID == SelectedValue);
                doorOperatorID = selectedDoor.DoorOperatorID;
            }
            IQueryable<DoorOperator> door = itemsrepository.DoorOperators.AsQueryable();
            foreach (DoorOperator doors in door)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = doors.DoorOperatorID.ToString();
                if (doors.DoorOperatorID == doorOperatorID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(doors.Brand);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; } = 0;

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a JobType");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<JobType> jobType = itemsrepository.JobTypes.AsQueryable();
            foreach (JobType jobTypes in jobType)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypes.JobTypeID.ToString();
                if (SelectedValue != 0 && jobTypes.JobTypeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeInJobSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeInJobSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a JobType");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<JobType> jobType = itemsrepository.JobTypes.FromSql("select * from dbo.JobTypes " +
                "Where dbo.JobTypes.JobTypeID IN (Select dbo.Jobs.JobTypeID From dbo.Jobs)").AsQueryable();
            foreach (JobType jobTypes in jobType)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypes.JobTypeID.ToString();
                if (jobTypes.JobTypeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeAddInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeAddInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Select a Type of operation");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> jobTypeAdd = ijobrepository.JobsExtensions.Select( j => j.JobTypeAdd ).Distinct().AsQueryable();
            foreach (string jobTypesAdd in jobTypeAdd)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypesAdd;
                if (jobTypesAdd == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypesAdd);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeMainiInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeMainiInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Select a Type of operation #2");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> jobTypeMain = ijobrepository.JobsExtensions.Select(j => j.JobTypeMain).Distinct().AsQueryable();
            foreach (string jobTypesMain in jobTypeMain)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypesMain;
                if (jobTypesMain == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypesMain);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class ValveBrandniInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public ValveBrandniInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Select a Valve Brand");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> valveBrand = ijobrepository.HydroSpecifics.Select(h => h.ValveBrand).Distinct().AsQueryable();
            foreach (string valveBrands in valveBrand)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = valveBrands;
                if (valveBrands == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(valveBrands);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class LandingSysInJobSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public LandingSysInJobSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.InnerHtml.Append("Please select a Landing System");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<LandingSystem> landingSys = itemsrepository.LandingSystems.FromSql("select * from dbo.LandingSystems " +
                "Where dbo.LandingSystems.LandingSystemID IN (Select dbo.HoistWayDatas.LandingSystemID From dbo.HoistWayDatas)").AsQueryable();
            foreach (LandingSystem landingsSys in landingSys)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = landingsSys.LandingSystemID.ToString();
                if (landingsSys.LandingSystemID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(landingsSys.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);

            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class StarterTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public StarterTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts":
                    return new List<string> { "208", "240", "380/415", "460", "480", "575" }.AsQueryable();
                case "Brand":
                    return new List<string> { "Siemens", "Sprecher Schuh" }.AsQueryable();
                case "Type":
                    return new List<string> { "ATL", "YD","3/9", "6/12" }.AsQueryable();
                case "OverloadTable":
                    return new List<string> { "1", "2", "3", "4", "5", "N/A" }.AsQueryable();
                default:
                    return new List<string> { "Error" }.AsQueryable();
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
            m_tag.InnerHtml.Append("Select a option");
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
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons1SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons1SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public int SelectedValue { get; set; }
        public int SelectedInR3Value { get; set; }
        public int SelectedInR2Value { get; set; }
        public int SelectedInR4Value { get; set; }
        public int SelectedInR5Value { get; set; }


        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Reason 1 --");
            result.InnerHtml.AppendHtml(m_tag);
            int reason1ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR3Value != 0)
            {
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == SelectedInR3Value);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR2Value != 0)
            {
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == SelectedInR2Value);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else
            {
                reason1ID = SelectedValue;
            }
            IQueryable<Reason1> reason1s = repository.Reasons1.AsQueryable();
            foreach (Reason1 item in reason1s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason1ID.ToString();
                if (item.Reason1ID == reason1ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons2SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons2SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR5Value { get; set; }
        public int SelectedInR4Value { get; set; }
        public int SelectedInR3Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Reason 2 --");
            result.InnerHtml.AppendHtml(m_tag);
            int reason2ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            if (SelectedInR3Value != 0)
            {
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == SelectedInR3Value);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            IQueryable<Reason2> reason2s = repository.Reasons2.AsQueryable();
            foreach (Reason2 item in reason2s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason2ID.ToString();
                if (item.Reason2ID == reason2ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons3SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons3SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR4Value { get; set; }
        public int SelectedInR5Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Reason 3 --");
            result.InnerHtml.AppendHtml(m_tag);
            int reason3ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                reason3ID = selectedR3.Reason3ID;
            }
            if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                reason3ID = selectedR3.Reason3ID;
            }
            IQueryable<Reason3> reason3s = repository.Reasons3.AsQueryable();
            foreach (Reason3 item in reason3s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason3ID.ToString();
                if (item.Reason3ID == reason3ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons4SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons4SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR5Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Reason 4 --");
            result.InnerHtml.AppendHtml(m_tag);
            int reason4ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                reason4ID = selectedR4.Reason4ID;
            }
            IQueryable<Reason4> reason4s = repository.Reasons4.AsQueryable();
            foreach (Reason4 item in reason4s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason4ID.ToString();
                if (item.Reason4ID == reason4ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons5SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons5SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

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
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Reason 5 --");
            result.InnerHtml.AppendHtml(m_tag);
            int reason5ID = 0;
            if (SelectedValue != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedValue);
                reason5ID = selectedR5.Reason5ID;
            }
            IQueryable<Reason5> reason5s = repository.Reasons5.AsQueryable();
            foreach (Reason5 item in reason5s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason5ID.ToString();
                if (item.Reason5ID == reason5ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class TriggerSelectTagHelper : TagHelper
    public class Trigger2SelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public TriggerSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        public Trigger2SelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "TriggerFeature":
                    return new List<string> {"Overlay", "Group","PC de Cliente", "Brake Coil Voltage > 10","EMBrake Module","EMCO Board","R6 Regen Unit","Local","Short Floor"
                                            ,"Custom","MRL","CTL2","Tarjeta CPI Incluida","Door Control en Cartop","Canada","Ontario","Manual Doors","Duplex","Serial Halls Calls"
                                            ,"Edge-LS","Rail-LS", "mView","iMonitor","HAPS Battery","2+ Starters", "MOD Door Operator"}.AsQueryable();
                case "TriggerCustom":
                    return new List<string> {"Contractor", "Fire Code","City", "VCI","Valve Brand","Switch Style","Landing System", "State" }.AsQueryable();
                default:
                    return new List<string> {"Overlay", "Group","PC de Cliente", "Brake Coil Voltage > 10","EMBrake Module","EMCO Board","R6 Regen Unit","Local","ShortFloor"
                                            ,"Custom","MRL","CTL2","Tarjeta CPI Incluida","Door Control en Cartop","Canada","Ontario","Manual Doors","Duplex","Serial Halls Calls"
                                            ,"Edge-LS","Rail-LS", "mView","iMonitor","HAPS Battery","2+ Starters", "MOD Door Operator"}.AsQueryable();
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
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

}
