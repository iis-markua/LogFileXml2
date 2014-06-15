using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Microsoft.DistributedAutomation.Logger;

namespace ErrorDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string suiteId = null;
            string caseId = null;
            string errorMes = null;
            string descrip = null;
            FileStream str = null;
            Byte[] xmlStartUp = new UTF8Encoding(true).GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Errors>\r\n</Errors>\r\n");
            XmlDocument xmlDoc = new XmlDocument();
            using (str = File.Create(Environment.ExpandEnvironmentVariables("%SystemDrive%\\myLog\\result.xml")))
            {
                str.Write(xmlStartUp, 0, xmlStartUp.Length);
            }
            //string[] logFiles = Directory.GetFiles(Environment.ExpandEnvironmentVariables("%SystemRoot%\\system32\\webtest\\tools\\suites"));
            string[] logFiles = Directory.GetFiles(Environment.ExpandEnvironmentVariables("%SystemDrive%\\myLog"));
            TestLogger tl = new TestLogger();
            Exception e = new Exception("System cannot find the file specified");
            bool a = tl.CheckResult(e, "System cannot find the file specified");
            string allText = null;
            for(int i=0; i< logFiles.Length; i++)
            {
                if (logFiles[i].Contains(".log"))
                {
                    suiteId = logFiles[i].Substring(9, logFiles[i].IndexOf(".log")-9);
                    allText = File.ReadAllText(logFiles[i]);
                    while(allText.Contains("*** ") || allText.Contains("treated as a failure"))
                    {
                        caseId = allText.Substring(allText.IndexOf("*** ") + 10);
                        if (caseId.Contains('['))
                            errorMes = caseId.Substring(caseId.IndexOf("] ") + 2, caseId.IndexOf('[') - 11);
                        else
                            errorMes = caseId.Substring(caseId.IndexOf("] ") + 2, caseId.IndexOf("END RUN") - 7);
                        if (caseId.Contains("END TEST"))
                        {
                            caseId = caseId.Substring(caseId.IndexOf("END TEST"));
                            caseId = caseId.Substring(caseId.IndexOf("] ") + 2, caseId.IndexOf(" [") - caseId.IndexOf("] ") - 2);
                            allText = allText.Substring(allText.IndexOf("*** ")+4);
                        }
                        else if(!caseId.Contains("END TEST"))
                        {
                            int a1 = caseId.IndexOf("CaseId");
                            int b1 = caseId.IndexOf("was");                            
                            caseId = caseId.Substring(caseId.IndexOf("CaseId") + 7, caseId.IndexOf(" was not run") - caseId.IndexOf("CaseId") - 7);
                            allText = allText.Substring(allText.IndexOf("Treated as a Failure.")+21);
                        }
                            

                        Console.WriteLine("suite: " + suiteId + "\r\n");
                        Console.WriteLine("case: " + caseId + "\r\n");
                        Console.WriteLine("Error: " + errorMes + "\r\n");

                        if (!(args[0] == "-uc"))
                        {
                            Console.WriteLine("Add more info here");
                            descrip = Console.ReadLine();
                        }
                        xmlDoc.Load(Environment.ExpandEnvironmentVariables("%SystemDrive%\\myLog\\result.xml"));
                        var root = xmlDoc.DocumentElement;
                        XmlNode newNode = xmlDoc.CreateNode("element", "error", "" );
                        
                        //
                        newNode.InnerText = "suite: " + suiteId + "\r\ncase: " + caseId + "\r\nError: " + errorMes + "Advice:" + descrip + "\r\n";
                        XmlNode suiteSubNode = xmlDoc.CreateNode("element", "Suite", "");
                        suiteSubNode.InnerText = "suite: " + suiteId;
                        XmlNode caseSubNode = xmlDoc.CreateNode("element", "CaseId", "");
                        caseSubNode.InnerText = "caseId: " + caseId;
                        XmlNode errorSubNode = xmlDoc.CreateNode("element", "ErrorMessage", "");
                        errorSubNode.InnerText = "Error: " + errorMes;
                        XmlNode adviceSubNode = xmlDoc.CreateNode("element", "Acvice", "");
                        adviceSubNode.InnerText = "Advice: " + descrip;
                        newNode.AppendChild(suiteSubNode);
                        newNode.AppendChild(caseSubNode);
                        newNode.AppendChild(errorSubNode);
                        newNode.AppendChild(adviceSubNode);
                        root.AppendChild(newNode);
                        xmlDoc.Save(Environment.ExpandEnvironmentVariables("%SystemDrive%\\myLog\\result.xml"));
                    }
                }
            }
        }
    }
}
