using PatrickBotman.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using static System.Net.Mime.MediaTypeNames;

namespace PatrickBotman.Common.Services
{
    public class ImageToTextService : IImageToTextService
    {
        public string GetText(byte[] image)
        {

            var text = new StringBuilder();
            using (var engine = new TesseractEngine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "tesseract-data"), "eng+rus+ukr", EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(image))
                {
                    using (var page = engine.Process(img))
                    {
                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();

                            do
                            {
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            text.Append(iter.GetText(PageIteratorLevel.Word));
                                            text.Append(" ");
 
                                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            } while (iter.Next(PageIteratorLevel.Block));
                        }
                    }
                }
            }

            return text.ToString();
        }
    }
}
