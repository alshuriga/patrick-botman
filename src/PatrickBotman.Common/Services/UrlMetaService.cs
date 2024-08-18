using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Services
{
    public class UrlMetaService : IUrlMetaService
    {
        private readonly ILogger<UrlMetaService> _logger;

        public UrlMetaService (ILogger<UrlMetaService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> GetMetaForUrl(Uri url)
        {
            var web = new HtmlWeb();

            HtmlDocument? doc = null;
            try
            {
                doc = await web.LoadFromWebAsync(url.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            

            if(doc?.DocumentNode == null)
            {
                return null;
            }

            var metaNodes = doc.DocumentNode?.SelectNodes("//meta")?.Where(n => n.Attributes["property"] != null && n.Attributes["content"] != null) ?? Enumerable.Empty<HtmlNode>();

            var titleNode = metaNodes.FirstOrDefault(n => n.Attributes["property"].Value.Contains("title"))?.Attributes["content"]?.Value;

            var sb = new StringBuilder();

            if(titleNode != null)
            {
                sb.AppendLine(titleNode);
            }


            if (sb.Length == 0)
            {
                var title = doc.DocumentNode?.SelectSingleNode("//title")?.GetDirectInnerText();
                if(title != null)
                {
                    sb.AppendLine(title);
                    return sb.ToString();
                }
                else
                {
                    return null;
                }    
            };

            return sb.ToString();
        }
    }
}
