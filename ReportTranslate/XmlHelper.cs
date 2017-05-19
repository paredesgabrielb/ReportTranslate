using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReportTranslate
{
   public class XmlHelper
    {

        public void TranslateReportOracleToSql(string RutaArchivo, string RutaSalida)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(RutaArchivo);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("r", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            //nsmgr.AddNamespace("cl", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition");

            //limpiar commandtext
            XmlNodeList query = xmlDoc.SelectNodes("//r:Query", nsmgr);
            foreach (XmlNode item in query)
            {
                item.InnerXml = item.InnerXml
                    .Replace("<CommandText>", "<CommandText> ")
                    .Replace("<CommandText xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition\">", "<CommandText xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition\"> ");
            }

            // Console.WriteLine(xmlDoc.OuterXml);
            XmlNodeList parametros = xmlDoc.SelectNodes("//r:QueryParameter", nsmgr);
            XmlNodeList CommandTexts = xmlDoc.SelectNodes("//r:CommandText", nsmgr);
            foreach (XmlNode xmlNode in CommandTexts)
            {
                xmlNode.InnerText = xmlNode.InnerText

                    .Replace("||", "+")
                    .Replace("NVL", "ISNULL")
                    .Replace(':', '@')
                    .Replace("substr", "SUBSTRING")

                    //ID VERSION 
                    .Replace(" v.id_version+'-'", " rtrim(v.id_version)+'-'")
                    .Replace(" id_version+'-'", " rtrim(id_version)+'-'")
                    .Replace(" ID_VERSION+'-'", " rtrim(ID_VERSION)+'-'")

                    .Replace(" v.id_version + '-'", " rtrim(v.id_version)+'-'")
                    .Replace(" id_version + '-'", " rtrim(id_version)+'-'")
                    .Replace(" ID_VERSION + '-'", " rtrim(ID_VERSION)+'-'")

                    .Replace(" v.id_version +'-'", " rtrim(v.id_version)+'-'")
                    .Replace(" id_version +'-'", " rtrim(id_version)+'-'")
                    .Replace(" ID_VERSION +'-'", " rtrim(ID_VERSION)+'-'")

                    .Replace(" v.id_version+ '-'", " rtrim(v.id_version)+'-'")
                    .Replace(" id_version+ '-'", " rtrim(id_version)+'-'")
                    .Replace(" ID_VERSION+ '-'", " rtrim(ID_VERSION)+'-'")

                    .Replace("from dual", " ");

                foreach (XmlNode item in parametros)
                {
                    var paramName = item.Attributes["Name"].Value;
                    var paramNameNew = item.Attributes["Name"].Value.Replace(':', '@');
                    xmlNode.InnerText = xmlNode.InnerText.Replace(paramName, paramNameNew);

                    item.Attributes["Name"].Value = paramNameNew;
                    // Console.WriteLine(item.Attributes["Name"].Value);
                }
                //Console.WriteLine(xmlNode.InnerText);
                //Console.WriteLine(xmlNode.Attributes["Name"].Value);
            }

            xmlDoc.Save(RutaSalida);
        }


        public void ReplaceDatasource(string RutaArchivo) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(RutaArchivo);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("r", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            #region MODIFICAR PRIMER DATASOURCE
            //Creando connection nueva del DataSource
            XmlElement elem = xmlDoc.CreateElement("DataSourceReference", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            elem.InnerText = "/Formulacion 2017/Formulacion 2017 Data Source";
            
            //Se selecciona el primer DataSOurce
            XmlNode dataSource = xmlDoc.SelectSingleNode("//r:DataSource", nsmgr);

            if (dataSource != null && dataSource.FirstChild != null)
            {
                //Modificando DataSource
                if (dataSource.FirstChild.LocalName == "DataSourceReference" || dataSource.FirstChild.LocalName == "ConnectionProperties")
                {
                    dataSource.ReplaceChild(elem, dataSource.FirstChild);
                }
                else
                {
                    dataSource.AppendChild(elem);
                }
            }
            #endregion

            xmlDoc.Save(RutaArchivo);
        }


        public void ReplaceServer(string RutaArchivo)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(RutaArchivo);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("r", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            nsmgr.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            #region MODIFICAR PRIMER DATASOURCE

            //Creando connection nueva del DataSource
            XmlElement elem = xmlDoc.CreateElement("rd:ReportServerUrl", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            elem.InnerText = "http://wk12dgpetl01/ReportServer";


            //Se selecciona el primer DataSOurce
            XmlNode servidorUrl = xmlDoc.SelectSingleNode("//rd:ReportServerUrl", nsmgr);

            if (servidorUrl != null)
            {
                //Modificando server
                servidorUrl.InnerText.Replace(servidorUrl.InnerText, elem.InnerText);
            }
            else
            {
                XmlNode report = xmlDoc.SelectSingleNode("//r:Report", nsmgr);

                report.AppendChild(elem);
            }

            #endregion

            xmlDoc.Save(RutaArchivo);
        }





        public RDLAttributes ReadFile(string filename)
        {
            RDLAttributes rptAtt = new RDLAttributes();
            List<RDLParam> l = new List<RDLParam>();

            // Create an XmlReader
            using (XmlReader reader = XmlTextReader.Create(filename))
            {
                reader.ReadToFollowing("DataSourceName");
                rptAtt.DataSourceName = "DataSourceName: " + reader.ReadElementContentAsString();

                reader.ReadToFollowing("CommandText");
                rptAtt.CommandText = "CommandText: " + reader.ReadElementContentAsString();

                string temp = "";
                reader.ReadToFollowing("ReportParameter");
                reader.MoveToAttribute("Name");
                temp = reader.Value;
                reader.ReadToFollowing("DataType");
                if (reader.NodeType != XmlNodeType.None)
                {
                    RDLParam rp = new RDLParam();
                    rp.ParameterName = temp;
                    rp.Datatype = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("Prompt");
                    rp.Prompt = reader.ReadElementContentAsString();
                    l.Add(rp);
                }

                rptAtt.Parameters = l;

            }

            return rptAtt;
        }
    }

    //Parameter Class
    public class RDLParam
    {
        public String ParameterName { get; set; }
        public String Datatype { get; set; }
        public String Prompt { get; set; }
    }

    //Report Attributes Class
    public class RDLAttributes
    {
        public List<RDLParam> Parameters { get; set; }
        public String DataSourceName { get; set; }
        public String CommandText { get; set; }
    }
}
