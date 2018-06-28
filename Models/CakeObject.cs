using System;
using System.Xml;
using System.Xml.Serialization;

namespace Cake.Models
{
    public class CakeObject
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public String EditorName { get; set; }
        public String Source { get; set; }
        public String Type { get; set; }
        public String ObjectPath { get; set; }
        public String ImagePath { get; set; }
        public String TexturePath { get; set; }
        public String JsonStr { get; set; }
        public String SavedSize { get; set; }
        public String MinSize { get; set; }
        public String MaxSize { get; set; }
        public String StepSize { get; set; }
        public String CakeSize { get; set; }
        public double NormalScale { get; set; }
        public Guid AdderID { get; set; }
        public String Keywords { get; set; }
        public ObjectProperties Properties { get; set; }
    }

    [XmlRoot("ObjectProperties")]
    public class ObjectProperties
    {
        [XmlElement("styles")]
        public styles styles { get; set; }

        [XmlElement("type")]
        public string type { get; set; }
    }

    public class styles
    {
        [XmlElement("scale")]
        public decimal scale { get; set; }

        [XmlElement("color")]
        public string color { get; set; }

        [XmlElement("coords")]
        public string coords { get; set; }
    }
}