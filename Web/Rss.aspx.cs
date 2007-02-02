using System;
using System.Collections;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Core.Service;
using Cuyahoga.Core.Util;
using Cuyahoga.Web.Util;
using Cuyahoga.Web.Components;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Web
{
	/// <summary>
	/// Summary description for Rss.
	/// </summary>
	public class Rss : CuyahogaPage
	{
		private CoreRepository _coreRepository;
		private ModuleLoader _moduleLoader;

		/// <summary>
		/// Module loader (injected)
		/// </summary>
		public ModuleLoader ModuleLoader
		{
			set { this._moduleLoader = value; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Context.Response.Clear();
			Context.Response.ContentType = "text/xml";
			if (Context.Request.QueryString["SectionId"] != null)
			{
				int sectionId = Int32.Parse(Context.Request.QueryString["SectionId"]);
				string pathInfo = Context.Request.PathInfo;
				string cacheKey = String.Format("RSS_{0}_{1}", sectionId, pathInfo);
				string content = null;

				if (Context.Cache[cacheKey] == null)
				{
					// Get the data for the RSS feed because it's not in the cache yet.
					// Use the same cache duration for the RSS feed as the Section.
					this._coreRepository = (CoreRepository)HttpContext.Current.Items["CoreRepository"];
					Section section = (Section)this._coreRepository.GetObjectById(typeof(Section), sectionId);

					this._moduleLoader.NHibernateModuleAdded += new EventHandler(ModuleLoader_ModuleAdded);
					ModuleBase module = this._moduleLoader.GetModuleFromSection(section);

					module.ModulePathInfo = pathInfo;
					ISyndicatable syndicatableModule = module as ISyndicatable;
					if (syndicatableModule != null)
					{
						RssChannel channel = syndicatableModule.GetRssFeed();
						// Rss feed writer code from http://aspnet.4guysfromrolla.com/articles/021804-1.aspx
						// Use an XmlTextWriter to write the XML data to a string...
						StringWriter sw = new StringWriter();
						XmlTextWriter writer = new XmlTextWriter(sw);
						writer.Formatting = Formatting.Indented;

						// write out 
						writer.WriteStartElement("rss");
						writer.WriteAttributeString("version", "2.0");
						writer.WriteAttributeString("xmlns:dc","http://purl.org/dc/elements/1.1/");

						// write out 
						writer.WriteStartElement("channel");

						// write out -level elements
						writer.WriteElementString("title", channel.Title);
						writer.WriteElementString("link", Util.UrlHelper.GetFullUrlFromSection(section) + pathInfo);
						writer.WriteElementString("description", channel.Description);
						writer.WriteElementString("language", channel.Language);
						writer.WriteElementString("pubDate", channel.PubDate.ToUniversalTime().ToString("r"));
						writer.WriteElementString("lastBuildDate", channel.LastBuildDate.ToUniversalTime().ToString("r"));
						writer.WriteElementString("generator", channel.Generator);
						writer.WriteElementString("ttl", channel.Ttl.ToString());
						
						// Regular expression to find relative urls
						string expression = String.Format(@"=[""']{0}", UrlHelper.GetApplicationPath());
						Regex regExUrl = new Regex(expression, RegexOptions.Singleline|RegexOptions.CultureInvariant|RegexOptions.Compiled);

						foreach (RssItem item in channel.RssItems)
						{
							// replace inline relative hyperlinks with full hyperlinks
							if (item.Description != null)
							{
								item.Description = regExUrl.Replace(item.Description, String.Format(@"=""{0}/", UrlHelper.GetSiteUrl()));
							}

							// write out 
							writer.WriteStartElement("item");

							// write out -level information
							writer.WriteElementString("title", item.Title);
							// TODO: Only supports ID's in the pathinfo now...
							writer.WriteElementString("link", Util.UrlHelper.GetFullUrlFromSection(section) + "/" + item.ItemId);
							writer.WriteElementString("description", item.Description);
							writer.WriteElementString("dc:creator", item.Author);
							writer.WriteElementString("pubDate", item.PubDate.ToUniversalTime().ToString("r"));
							writer.WriteElementString("category", item.Category);

							// write out 
							writer.WriteEndElement();
						}

						// write out 
						writer.WriteEndElement();

						// write out 
						writer.WriteEndElement();

						content = sw.ToString();
						// save the string in the cache
						Cache.Insert(cacheKey, content, null, DateTime.Now.AddSeconds(section.CacheDuration), TimeSpan.Zero);

						writer.Close();
					}
					else
					{
						throw new Exception(String.Format("The module {0} doesn't implement ISyndicatable", module.GetType().FullName));
					}
				}
				else
				{
					content = Context.Cache[cacheKey].ToString();
				}
				Context.Response.Write(content);
			}

			Context.Response.End();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		private void ModuleLoader_ModuleAdded(object sender, EventArgs e)
		{
			// A module that uses NHibernate was loaded for the first time.
			// This is handled by a simple reload of the page. 
			// TODO: handle more elegantly?
			if (Context.Items["VirtualUrl"] != null)
			{
				Context.Response.Redirect(Context.Items["VirtualUrl"].ToString());
			}
			else
			{
				Context.Response.Redirect(Context.Request.RawUrl);
			}
		}
	}
}
