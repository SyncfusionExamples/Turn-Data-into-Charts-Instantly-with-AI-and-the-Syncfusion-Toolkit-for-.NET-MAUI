using ChartGenerater;
using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.DataSource.Extensions;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace AssistViewMAUI
{
    public class XmlFileCreator
    {
        private string xmlFilePath;

        public XmlFileCreator()
        {
#if ANDROID
            xmlFilePath = Path.Combine(FileSystem.AppDataDirectory, "ChatHistory.xml");
#elif IOS
            xmlFilePath = Path.Combine(FileSystem.AppDataDirectory, "ChatHistory.xml");
#else
            xmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChatHistory.xml");
#endif
        }
        
        private ChartConfig ParseChartConfig(XElement chartConfigElement)
        {
            if (chartConfigElement == null) return null;

            return new ChartConfig
            {
                ChartType = (ChartTypeEnum)Enum.Parse(typeof(ChartTypeEnum), (string)chartConfigElement.Element("ChartType")),
                Title = (string)chartConfigElement.Element("Title"),
                ShowLegend = (bool)chartConfigElement.Element("ShowLegend"),
                XAxis = new ObservableCollection<AxisConfig>(
                    chartConfigElement.Element("XAxis").Elements("Axis").Select(ax => new AxisConfig
                    {
                        Title = (string)ax.Element("Title"),
                        Type = (string)ax.Element("Type")
                    })),
                YAxis = new ObservableCollection<AxisConfig>(
                    chartConfigElement.Element("YAxis").Elements("Axis").Select(ay => new AxisConfig
                    {
                        Title = (string)ay.Element("Title"),
                        Type = (string)ay.Element("Type")
                    })),
                Series = new ObservableCollection<SeriesConfig>(
                    chartConfigElement.Element("Series").Elements("SeriesConfig").Select(sc => new SeriesConfig
                    {
                        Type = (SeriesType)Enum.Parse(typeof(SeriesType), (string)sc.Element("Type")),
                        XPath = (string)sc.Element("XPath"),
                        Tooltip = (bool)sc.Element("Tooltip"),
                        DataSource = new ObservableCollection<DataModel>(
                            sc.Element("DataSource").Elements("DataModel").Select(dm => new DataModel
                            {
                                xvalue = (string)dm.Element("xvalue"),
                                yvalue = (double)dm.Element("yvalue")
                            }))
                    }))
            };
        }

        // Load data from XML file
        public ObservableCollection<ChatHistoryModel> LoadFromXml()
        {
            if (!System.IO.File.Exists(xmlFilePath))
            {
                return new ObservableCollection<ChatHistoryModel>();
            }

            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            return xmlDoc.Root.Elements("ChatHistory")
                .Select(ch => new ChatHistoryModel
                {
                    Title = (string)ch.Element("Title"),
                    ConversationCreatedDate = (DateTime)ch.Element("ConversationCreatedDate"),
                    Message = (string)ch.Element("Message"),
                    Messages = new ObservableCollection<IAssistItem>(
                        ch.Element("Messages").Elements("AssistItem")
                        .Select(ai => ParseAssistItem(ai))
                    )
                }).ToObservableCollection();
        }


        private IAssistItem ParseAssistItem(XElement aiElement)
        {
            var chartConfig = aiElement.Element("ChartConfig");

            if(chartConfig != null)
            {
                var config = ParseChartConfig(chartConfig);

                if(config.ChartType == ChartTypeEnum.Cartesian)
                {
                    return new CartesianAssistItem
                    {

                        ChartConfig = config

                    };
                }
                else
                {
                    return new CircularAssistItem
                    {

                        ChartConfig = config

                    };
                }
                   
            }

            string text = (string)aiElement.Element("Text");
            bool isRequested = (bool)aiElement.Element("IsRequested");
            XElement requestItemElement = aiElement.Element("RequestItem");
            DateTime dateTime = (DateTime)aiElement.Element("DateTime");
            string source = (string)aiElement.Element("ImageSource");
            bool? isLiked = false;// (bool?)aiElement.Element("IsLiked");
            bool showAssistItemFooter = false;// (bool)aiElement.Element("ShowAssistItemFooter");


            IAssistItem requestItem = null;
            if (requestItemElement != null)
            {
                requestItem = ParseAssistItem(requestItemElement.Element("AssistItem"));
            }
            if (string.IsNullOrEmpty(source))
            {
                return new AssistItem() { Text = text, IsRequested = isRequested, RequestItem = requestItem, DateTime = dateTime, IsLiked = isLiked, ShowAssistItemFooter = showAssistItemFooter };
            }
            else
            {
                var imageSource = XmlToImageSource(aiElement.Element("ImageSource"));
                return new AssistImageItem() { Text = text, IsRequested = isRequested, RequestItem = requestItem, DateTime = dateTime, IsLiked = isLiked, ShowAssistItemFooter = showAssistItemFooter, Source = imageSource };
            }
        }

        public ImageSource XmlToImageSource(XElement imageElement)
        {
            // Check if imageElement has a valid Base64 string
            if (imageElement == null || string.IsNullOrEmpty(imageElement.Value))
            {
                throw new ArgumentException("Invalid XML element or empty Base64 string");
            }

            // Decode the Base64 string back into bytes
            byte[] imageBytes = Convert.FromBase64String(imageElement.Value);

            return ConvertByteArrayToImageSource(imageBytes);
        }
        // Save data to XML file
        public void SaveToXml(ObservableCollection<ChatHistoryModel> chatHistories)
        {
            XElement xmlData = new XElement("ChatHistories",
                chatHistories.Select(ch => new XElement("ChatHistory",
                    new XElement("Title", ch.Title),
                    new XElement("ConversationCreatedDate", ch.ConversationCreatedDate),
                    new XElement("Message", ch.Message),
                    new XElement("Messages",
                from message in ch.Messages
                select AssistItemToXml(message)
                ))));

            xmlData.Save(xmlFilePath);
        }

        public async Task<XElement> ImageSourceToXml(ImageSource imageSource)
        {
            // Read the stream into a byte array
            byte[] imageBytes = await ConvertImageSourceToByteArray(imageSource);

            // Convert the byte array to a Base64 string
            string base64Image = Convert.ToBase64String(imageBytes);

            // Create and return the XElement with the Base64 encoded image data
            return new XElement("ImageSource", base64Image);
        }



         private XElement ChartAssistItemToXml(IAssistItem item)
        {
            ChartConfig chartConfig = new ChartConfig();
            if ((item is CartesianAssistItem cartesianAssistItem))
            {
                chartConfig = cartesianAssistItem.ChartConfig;
                
              
            }
            else if(item is CircularAssistItem circularAssistItem)
            {
                chartConfig = circularAssistItem.ChartConfig;
            }

            XElement chartConfigElement = new XElement("ChartConfig",
                new XElement("ChartType", chartConfig.ChartType),
                new XElement("Title", chartConfig.Title),
                new XElement("ShowLegend", chartConfig.ShowLegend),
                new XElement("XAxis",
                    from xAxis in chartConfig.XAxis
                    select new XElement("Axis",
                        new XElement("Title", xAxis.Title),
                        new XElement("Type", xAxis.Type),
                        new XElement("Minimum", xAxis.Minimum),
                        new XElement("Maximum", xAxis.Maximum)
                    )
                ),
                new XElement("YAxis",
                    from yAxis in chartConfig.YAxis
                    select new XElement("Axis",
                        new XElement("Title", yAxis.Title),
                        new XElement("Type", yAxis.Type),
                        new XElement("Minimum", yAxis.Minimum),
                        new XElement("Maximum", yAxis.Maximum)
                    )
                ),
                new XElement("Series",
                    from series in chartConfig.Series
                    select new XElement("SeriesConfig",
                        new XElement("Type", series.Type),
                        new XElement("XPath", series.XPath),
                        new XElement("Tooltip", series.Tooltip),
                        new XElement("DataSource",
                            from data in series.DataSource
                            select new XElement("DataModel",
                                new XElement("xvalue", data.xvalue),
                                new XElement("yvalue", data.yvalue),
                                new XElement("date", data.date),
                                new XElement("xval", data.xval)
                            )
                        )
                    )
                )
            );

            return new XElement("AssistItem",
               
                chartConfigElement
               
            );
        }


        private XElement AssistItemToXml(IAssistItem item)
        {
            var carteisanItem = item as CartesianAssistItem;
            var circularItem = item as CircularAssistItem;

            if (carteisanItem != null || circularItem != null)
            {
                return ChartAssistItemToXml(item);
            }

            var AssistImageItem = item as AssistImageItem;

            return new XElement("AssistItem",
                AssistImageItem != null ? ImageSourceToXml(AssistImageItem.Source).Result : null,
                new XElement("Text", item.Text),
                new XElement("IsRequested", item.IsRequested),
                new XElement("DateTime", item.DateTime),
                new XElement("IsLiked", item.IsLiked),
                new XElement("ShowAssistItemFooter", item.ShowAssistItemFooter),
                item.RequestItem != null ? new XElement("RequestItem", AssistItemToXml(item.RequestItem as IAssistItem)) : null
            );
        }

        // Add a new chat history
        public void AddChatHistory(ChatHistoryModel chatHistory)
        {
            var chatHistories = LoadFromXml();
            //chatHistories.Add(chatHistory);
            chatHistories.Insert(0, chatHistory);
            SaveToXml(chatHistories);
        }

        // Update an existing chat history
        public void UpdateChatHistory(ChatHistoryModel updatedChatHistory)
        {
            var chatHistories = LoadFromXml();
            var chatHistory = chatHistories.FirstOrDefault(c => c.ConversationCreatedDate.Date == updatedChatHistory.ConversationCreatedDate.Date && c.Title == updatedChatHistory.Title);
            if (chatHistory != null)
            {
                chatHistory.ConversationCreatedDate = updatedChatHistory.Messages[updatedChatHistory.Messages.Count - 1].Text.Length == 1 ? updatedChatHistory.ConversationCreatedDate : System.DateTime.Now;
                chatHistory.Messages = updatedChatHistory.Messages;
                chatHistory.Title = updatedChatHistory.Title;
                chatHistory.Message = updatedChatHistory.Message;
                SaveToXml(chatHistories);
            }
        }

        // Update title for chat history

        public void UpdateChatHistory(string title, ChatHistoryModel currentChatHistory)
        {
            var chatHistories = LoadFromXml();
            var chatHistory = chatHistories.FirstOrDefault(c => c.ConversationCreatedDate.Date == currentChatHistory.ConversationCreatedDate.Date);
            if (chatHistory != null)
            {
                chatHistory.Title = title;
                SaveToXml(chatHistories);
            }
        }

        // Delete a chat history
        public void DeleteChatHistory(ChatHistoryModel deleteChatHistory)
        {
            var chatHistories = LoadFromXml();
            var chatHistory = chatHistories.FirstOrDefault(c => c.Title == deleteChatHistory.Title && c.ConversationCreatedDate.Date == deleteChatHistory.ConversationCreatedDate.Date);
            if (chatHistory != null)
            {
                chatHistories.Remove(chatHistory);
                SaveToXml(chatHistories);
            }
        }


        #region Image source conversion
        public static ImageSource ConvertByteArrayToImageSource(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null; // or return a default ImageSource
            }

            return new StreamImageSource
            {
                Stream = cancellationToken => Task.FromResult<Stream>(new MemoryStream(imageData))
            };
        }

        public static async Task<byte[]> ConvertImageSourceToByteArray(ImageSource imageSource)
        {
            Stream stream = await ConvertImageSourceToStream(imageSource);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private static async Task<Stream> ConvertImageSourceToStream(ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource)
            {
                return new FileStream(fileImageSource.File, FileMode.Open, FileAccess.Read);
            }
            else if (imageSource is UriImageSource uriImageSource)
            {
                var httpClient = new System.Net.Http.HttpClient();
                return await httpClient.GetStreamAsync(uriImageSource.Uri);
            }
            else if (imageSource is StreamImageSource streamImageSource)
            {
                return await streamImageSource.Stream(default);
            }
            else
            {
                throw new NotSupportedException("Unsupported ImageSource type");
            }
        }
        #endregion
    }
}
