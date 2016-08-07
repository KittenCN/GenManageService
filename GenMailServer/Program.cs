﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;
using System.Threading;

namespace GenMailServer
{
    class Program
    {
        public static string GenLinkString = "./DB/GenMailServer.accdb";
        public static string GenCheckStr = "MailQueues";
        public static string LinkCheckStr = "MailTrans";
        public static string LinkString1;
        public static string LinkString2;
        public static int EmailRete = 10;
        public static string strLocalAdd = ".\\Config.xml";
        public static Boolean boolClockShow = false;
        public static Timer t;
        public static Timer tClock;
        public static Boolean boolProcess = false;
        public static int intMainRate = 60;
        public static int intSecondShow = 60;
        public static int intEmailTestFlag = 0;
        public static string strEmailTestAddress = "owdely@163.com";
        public static Boolean boolSilentTimeShow = false;
        public static int intSilentTime = 10;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WindowWidth = 120;
            Console.WindowHeight = 33;
            Console.WriteLine("Welcome to GMS-General Mail Server");
            Console.BackgroundColor = ConsoleColor.Blue;            
            Console.WriteLine("");
            for(int i=0;i<10;i++)
            {
                if(i%2==0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\rDO NOT CLICK THE INTERFACE AND DO NOT PRESS ANYKEY WHEN THE RUNNING CONSOLE IS IN THE FOREGROUND!!");
                    Thread.Sleep(500);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("\rDO NOT CLICK THE INTERFACE AND DO NOT PRESS ANYKEY WHEN THE RUNNING CONSOLE IS IN THE FOREGROUND!!");
                    Thread.Sleep(500);
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\rDO NOT CLICK THE INTERFACE AND DO NOT PRESS ANYKEY WHEN THE RUNNING CONSOLE IS IN THE FOREGROUND!!");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("");
            if (File.Exists(strLocalAdd))
            {
                try
                {
                    Console.WriteLine("Reading Config File ...");
                    XmlDocument xmlCon = new XmlDocument();
                    xmlCon.Load(strLocalAdd);
                    XmlNode xnCon = xmlCon.SelectSingleNode("Config");
                    GenLinkString = "./DB/GenMailServer.accdb";
                    GenCheckStr = "MailQueues";
                    LinkCheckStr = "MailTrans";
                    LinkString1 = xnCon.SelectSingleNode("LinkString1").InnerText;
                    LinkString2 = xnCon.SelectSingleNode("LinkString2").InnerText;
                    EmailRete = int.Parse(xnCon.SelectSingleNode("EmailRate").InnerText);
                    intEmailTestFlag = int.Parse(xnCon.SelectSingleNode("EmailTestFlag").InnerText);
                    intMainRate=int.Parse(xnCon.SelectSingleNode("MainRate").InnerText);
                    strEmailTestAddress = xnCon.SelectSingleNode("EmailTestAddress").InnerText;
                    Console.WriteLine("Reading Config File Successfully...");

                    intSilentTime = EmailRete;
                    intSecondShow = intMainRate;
                    Console.WriteLine("Begin Timer Methods...");
                    t = new Timer(TimerCallback, null, 0, intMainRate * 1000);
                    tClock = new Timer(TimerClockShow, null, 0, 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Error:Config File Lost!");
            }
            Console.ReadLine();
        }
        private static void TransToLocal(DataTable dt,int intFlag)
        {
            AccessHelper.AccessHelper ah = new AccessHelper.AccessHelper(GenLinkString);
            foreach (DataRow row in dt.Rows)
            {
                string strSQL = "insert into MailQueues(MailSubject,MailBody,MailTargetAddress,MailDateTime,Flag) ";
                strSQL = strSQL + " values('" + row["MailSubject"].ToString() + "','" + row["MailBody"].ToString() + "','" + row["MailTargetAddress"].ToString() + "',#" + DateTime.Now.ToString() + "#," + intFlag + ") ";
                ah.ExecuteNonQuery(strSQL);
            }
        }
         
        private static void TimerClockShow(object o)
        {
            if (intSecondShow > 0)
            {
                intSecondShow--;
            }
            else
            {
                intSecondShow = intMainRate;
            }
            //if (intSilentTime > 0)
            //{
            //    intSilentTime--;
            //}
            //else
            //{
            //    intSilentTime = EmailRete;
            //}
            if (!boolProcess)
            {
                if (!boolClockShow)
                {
                    Console.WriteLine("");
                    Console.Write("\rNow is :" + DateTime.Now.ToString() + " ...");
                    boolClockShow = true;
                }
                else
                {
                    Console.Write("\rNow is :" + DateTime.Now.ToString() + " , and " + intSecondShow + " seconds to the next execution.");
                }
            }
            //else if(boolProcess && boolSilentTimeShow)
            //{
            //    Console.WriteLine("");
            //    Console.Write("\rSilent Time : " + intSilentTime + " Sec Left...");
            //}
            GC.Collect();
        }

        private static void TimerCallback(Object o)
        {
            // Display the date/time when this method got called.
            //Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Force a garbage collection to occur for this demo.
            Console.WriteLine("");
            Console.WriteLine("Running Main Method...");

            //boolClockShow = false;
            boolProcess = true;
            bool boolstatus = false;
            emailHelper.emailHelper eh = new emailHelper.emailHelper();
            try
            {
                if (intEmailTestFlag == 1)
                {
                    Console.WriteLine("Debug Mode Open...");
                    for(int i=1; i<=2; i++)
                    {
                        string strDebugResult = emailHelper.emailHelper.SendEmail("TestSubject", "TestBody", strEmailTestAddress, i);
                        if (strDebugResult == "Success!")
                        {
                            Console.WriteLine("The Debug Mail With LinkString[" + i + "] has been sent successfully!");
                            Thread.Sleep(EmailRete * 1000);
                        }
                        else
                        {
                            Console.WriteLine("LinkString[" + i + "] had Error:" + strDebugResult);
                        }
                    }
                }

                if (AccessHelper.AccessHelper.CheckDB(GenLinkString, GenCheckStr) && AccessHelper.AccessHelper.CheckDB(LinkString1, LinkCheckStr) && AccessHelper.AccessHelper.CheckDB(LinkString2, LinkCheckStr))
                {
                    boolstatus = true;
                }
                else
                {
                    boolstatus = false;
                }
            }
            catch (Exception ex)
            {
                boolstatus = false;
                Console.WriteLine("Error:" + ex.ToString());
            }
            if (boolstatus)
            {
                try
                {
                    Console.WriteLine("Trans Data to Local DB from LinkString1...");
                    AccessHelper.AccessHelper ah = new AccessHelper.AccessHelper(LinkString1);
                    string strSQL = "select * from " + LinkCheckStr;
                    DataTable dtSQL = ah.ReturnDataTable(strSQL);
                    TransToLocal(dtSQL,1);
                    strSQL = "delete from " + LinkCheckStr;
                    ah.ExecuteNonQuery(strSQL);

                    Console.WriteLine("Trans Data to Local DB from LinkString2...");
                    ah = new AccessHelper.AccessHelper(LinkString2);
                    strSQL = "select * from " + LinkCheckStr;
                    dtSQL = ah.ReturnDataTable(strSQL);
                    TransToLocal(dtSQL,2);
                    strSQL = "delete from " + LinkCheckStr;
                    ah.ExecuteNonQuery(strSQL);

                    Console.WriteLine("Processing the Local Mail Queues...");
                    ah = new AccessHelper.AccessHelper(GenLinkString);
                    strSQL = "select * from " + GenCheckStr;
                    dtSQL = ah.ReturnDataTable(strSQL);
                    int i = 0;
                    foreach (DataRow row in dtSQL.Rows)
                    {
                        i++;
                        string strMailResult = emailHelper.emailHelper.SendEmail(row["MailSubject"].ToString(), row["MailBody"].ToString(), row["MailTargetAddress"].ToString(),int.Parse(row["Flag"].ToString()));
                        if (strMailResult == "Success!")
                        {
                            Console.WriteLine("The " + i + " Mail has been sent successfully!");
                            string strInSQL = "insert into MailHistory(MailSubject,MailBody,MailTargetAddress,MailDateTime,SendDateTime,Flag) ";
                            strInSQL = strInSQL + " values('" + row["MailSubject"].ToString() + "','" + row["MailBody"].ToString() + "','" + row["MailTargetAddress"].ToString() + "',#" + row["MailDateTime"].ToString() + "#,#" + DateTime.Now.ToString() + "#," + int.Parse(row["Flag"].ToString()) + ") ";
                            ah.ExecuteNonQuery(strInSQL);
                            strInSQL = "delete from " + GenCheckStr + " where id=" + row["ID"].ToString() + " ";
                            ah.ExecuteNonQuery(strInSQL);
                            Console.WriteLine("The " + i + " Mail has been processed successfully!");
                            Thread.Sleep(EmailRete * 1000);
                        }
                        else
                        {
                            Console.WriteLine("Error:" + strMailResult);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Error:" + "Some Boolean Values is False!");
            }
            boolProcess = false;
            Console.WriteLine("End Running...");
            GC.Collect();
        }
    }
}
