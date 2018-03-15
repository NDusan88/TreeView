using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfApexJobTest.Class
{
    class SaveXML
    {
        public static void SaveXml(object obj, string fileName)
        {
            XmlSerializer sr = new XmlSerializer(obj.GetType());
            TextWriter writer = new StreamWriter(fileName);
            sr.Serialize(writer, obj);
            writer.Close();
        }
    }
}
