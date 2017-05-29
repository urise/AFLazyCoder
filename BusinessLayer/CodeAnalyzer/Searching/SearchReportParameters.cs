using System;
using System.Xml.Serialization;
using BusinessLayer.Common;

namespace BusinessLayer.CodeAnalyzer.Searching
{
    [Serializable, XmlType("report")]
    public class SearchReportParameters
    {
        [XmlElement("isdetailed")]
        public bool IsDetailed { get; set; }

        [XmlElement("order")]
        public string OrderString { get; set; }

        [XmlElement("quantitylimit")]
        public int QuantityLimit { get; set; }

        [XmlElement("outputfilename")]
        public string OutputFileName { get; set; }

        public TextSearcherReportOrder Order
        {
            get
            {
                try
                {
                    return (TextSearcherReportOrder)Enum.Parse(typeof(TextSearcherReportOrder), OrderString);
                }
                catch
                {
                    throw new CustomException(OrderString + " is wrong report order");
                }
            }
        }

        public SearchReportParameters()
        {
            IsDetailed = true;
            OrderString = "Quantity";
            QuantityLimit = 2;
        }

        public SearchReportParameters(bool isDetailed, TextSearcherReportOrder order, int quantityLimit)
        {
            IsDetailed = isDetailed;
            OrderString = order.ToString();
            QuantityLimit = quantityLimit;
        }
    }
}
