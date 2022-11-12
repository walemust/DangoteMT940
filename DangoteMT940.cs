using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace DangoteMT940
{
    public partial class DangoteMT940
    {
        Timer timer = new Timer();
        //public DangoteMT940()
        //{
        //    InitializeComponent();
        //}
        protected void OnStart(string[] args)
        {
            int timeinterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["timeint"]);
            MT940Model.WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10000; //1hour 1000 is 1sec  
            //timer.Interval = 36000000; //10hours for deploy  
            timer.Enabled = true;
        }
        protected void OnStop()
        {
            MT940Model.WriteToFile("Service is stopped at " + DateTime.Now);

        }

        //public bool ExecuteSybaseScriptSingleParam(admConnectionSetup con, DateTime dt, string SqlString)
        //{
        //    string connstring = System.Configuration.ConfigurationManager.AppSettings["sybcon"].ToString();
        //    connstring = connstring.Replace("{{Data Source}}", con.DatabaseServer);//172.29.6.134
        //    connstring = connstring.Replace("{{port}}", con.DatabasePort);
        //    connstring = connstring.Replace("{{database}}", con.Databasename);
        //    connstring = connstring.Replace("{{uid}}", con.DatabaseUserName);
        //    connstring = connstring.Replace("{{pwd}}", MT940Model.DecryptPswd(con.DatabasePassword));
        //    using AseConnection theCons = new AseConnection(connstring);
        //    DataSet ds = new DataSet();


        //    try
        //    {
        //        theCons.Open();
        //        AseCommand cmdb = new AseCommand(SqlString, theCons);
        //        cmdb.Connection = theCons;
        //        cmdb.CommandTimeout = 0;
        //        cmdb.CommandType = CommandType.StoredProcedure;
        //        AseParameter param = cmdb.CreateParameter();
        //        param.ParameterName = "@pdtLastStmtDt";
        //        param.AseDbType = AseDbType.DateTime;
        //        param.Direction = ParameterDirection.Input;
        //        param.Value = dt;
        //        cmdb.Parameters.Add(param);
        //        DateTime LatestProcessedDate;
        //        Object res = cmdb.ExecuteScalar();
        //        if (res != null)
        //        {
        //            LatestProcessedDate = Convert.ToDateTime(res);
        //            theCons.Close();
        //            return true;
        //        }
        //        else
        //        {
        //            MT940Model.WriteToFile("no new data to process");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MT940Model.WriteToFile(ex.Message == null ? ex.InnerException.Message : ex.Message);
        //        //timer.Enabled = true;
        //    }
        //    return false;
        //}
        public void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //var path = System.Configuration.ConfigurationManager.AppSettings["MsgFilePath"];
            //string conrow = System.Configuration.ConfigurationManager.AppSettings["defaultCon"].ToString();
            //string conrowz = System.Configuration.ConfigurationManager.AppSettings["sybCon2Name"].ToString();
            //string acctno = System.Configuration.ConfigurationManager.AppSettings["acctno"].ToString();
            //string accttype = System.Configuration.ConfigurationManager.AppSettings["accttype"].ToString();
            try
            {
                var db1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["defaultCon"].ToString());
                db1.Open();
                string dt = string.Empty;
                //var parameter = new { Conrow = conrow };
                //var parameterz = new { Conrowz = conrowz };
                //string sql1 = "check last generated date from Mt940config";
                //var LastStmtDate = db1.ExecuteScalar<DateTime>(sql1);
                //MT940Model.WriteToFile("selected last processed date as " + LastStmtDate);
                var sql = "select * from BankServiceSetup";
                var con = db1.Query<admBankServiceSetUp>(sql).FirstOrDefault();
                var bank = "";
                //var sql2 = "select * from admConnectionSetup where name = @Conrowz";
                //var con2 = db1.Query<admConnectionSetup>(sql2, parameterz).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //
            }
        }
        //    var path = System.Configuration.ConfigurationManager.AppSettings["MsgFilePath"];
        //    string conrow = System.Configuration.ConfigurationManager.AppSettings["sybConName"].ToString();
        //    string conrowz = System.Configuration.ConfigurationManager.AppSettings["sybCon2Name"].ToString();
        //    string acctno = System.Configuration.ConfigurationManager.AppSettings["acctno"].ToString();
        //    string accttype = System.Configuration.ConfigurationManager.AppSettings["accttype"].ToString();
        //    try
        //    {
        //        var db1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["defaultCon"].ToString());
        //        db1.Open();
        //        string dt = string.Empty;
        //        var parameter = new { Conrow = conrow };
        //        var parameterz = new { Conrowz = conrowz };
        //        string sql1 = "check last generated date from Mt940config";
        //        var LastStmtDate = db1.ExecuteScalar<DateTime>(sql1);
        //        MT940Model.WriteToFile("selected last processed date as " + LastStmtDate);
        //        var sql = "select * from admConnectionSetup where name = @Conrow";
        //        var con = db1.Query<admConnectionSetup>(sql, parameter).FirstOrDefault();
        //        var sql2 = "select * from admConnectionSetup where name = @Conrowz";
        //        var con2 = db1.Query<admConnectionSetup>(sql2, parameterz).FirstOrDefault();
        //        if (LastStmtDate != null)
        //        {
        //            bool ReadyForProcessing = ExecuteSybaseScriptSingleParam(con, LastStmtDate, "isp_IB_Acct94StmtValDt");
        //            if (ReadyForProcessing)
        //            {
        //                timer.Enabled = false;

        //                //Load dp_history zenbase all data the first time, new data subsequent times
        //                List<AseParameter> aseparameters = GetAseParametersMini();
        //                DataTable res = ExecuteSybaseScript(con2, aseparameters, "Isp_InsertDpHistory", "2");

        //                //load selected columns from Dangote Collections
        //                string sqlmaxitbid = "select MaxItbid from DangoteMt940config";
        //                Int64 maxid = db1.ExecuteScalar<Int64>(sqlmaxitbid);
        //                SqlCommand cmd = new SqlCommand();
        //                cmd.Connection = db1;
        //                var sql3 = "select a.itbid, a.cr_account_no, a.cr_account_name, a.cr_account_type, a.customer_number, a.customer_name, a.amount, isnull(a.deposit_slip_no,a.teller_no) as deposit_slip_no, a.date_created from DangoteCollectionTransaction a, CBSTransaction b where a.CBSTransId = b.ItbId and b.reversal = 0 and b.PostingErrorCode = 0 and a.cr_account_no = @Acctno and a.cr_account_type= @Accttype and a.itbid > @Itbid and a.trans_error_code = @Error";
        //                cmd.CommandText = sql3;
        //                cmd.Parameters.Add("@Acctno", SqlDbType.VarChar).Value = acctno;
        //                cmd.Parameters.Add("@Accttype", SqlDbType.VarChar).Value = accttype;
        //                cmd.Parameters.Add("@Error", SqlDbType.VarChar).Value = 0;
        //                cmd.Parameters.Add("@Itbid", SqlDbType.VarChar).Value = maxid;

        //                DataTable dangoteCollection = new DataTable();
        //                dangoteCollection.Load(cmd.ExecuteReader());
        //                int o = InsertDataZenbaseTable(dangoteCollection, con2);
        //                if (o == 222)
        //                {
        //                    //List<AseParameter> aseParam = GetAseParameters();
        //                    //DataTable statementData = ExecuteSybaseScript(con, aseParam, "isp_Dangote940Stmt", "1");
        //                    if (statementData != null)
        //                    {
        //                        //var loadresult = InsertDatatoTPMTable(statementData);
        //                        string statementstring = MT940Mapper.Generate940String(statementData);
        //                        string filename = string.Format("DangoteStatement_{0}.txt", string.Format("{0:dd_MMM_yyyy}", DateTime.Now.AddDays(-1)));
        //                        //string filename = string.Format("{0}{1}", "DangoteStatement_", string.Format("{0:dd_MMM_yyyy}", DateTime.Now.AddDays(-1)),".txt");
        //                        string path2 = path + filename;
        //                        File.WriteAllText(path2, statementstring);
        //                        if (File.Exists(path2))
        //                        {
        //                            var emailsent = SendStatementEmail(path2);
        //                            string date = ExecuteSybaseGetlasttodate(con);
        //                            var dateprocessed = new { Dateproc = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture) };
        //                            var sqlstr = "update DangoteMt940config set LastGeneratedDate = @Dateproc";
        //                            var setid = db1.Execute(sqlstr, dateprocessed);
        //                        }
        //                        db1.Close();
        //                        timer.Enabled = true;
        //                    }
        //                    //DataSetObj.Tables["Table_Name"].Rows[rowIndex]["column_name"]
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        MT940Model.WriteToFile(string.Format("rror: {0}", ex.Message == null ? ex.InnerException.Message : ex.Message));
        //    }
        //    finally
        //    {
        //        timer.Enabled = true;
        //    }

        //}

        public static DataTable ListToDataTable<MT940Setup>(IEnumerable<MT940Setup> list)
        {
            var properties = typeof(MT940Setup).GetProperties();
            var dataTable = new DataTable();
            foreach (var info in properties)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType)
                   ?? info.PropertyType);
            foreach (var entity in list)
                dataTable.Rows.Add(properties.Select(p => p.GetValue(entity)).ToArray());

            return dataTable;
        }

        public static DataTable GetMT940(int acct)
        {

            List<MT940Setup> atrlist = new List<MT940Setup>();

            try
            {
                using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["defaultCon"].ToString()))
                {

                    for (int i = 0; i < acct; i++)
                    {
                        con.Open();
                        MT940Setup atr = new MT940Setup();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "isp_IB_Acct94StmtValDt";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter Channel = new SqlParameter("psChannel", SqlDbType.VarChar) { Value = "DANGOTE" };

                            SqlParameter AcctNo = new SqlParameter("@rsAccountNo", SqlDbType.Int); AcctNo.Direction = ParameterDirection.Output;
                            SqlParameter StateAcctNo = new SqlParameter("@rnStateAccountNo", SqlDbType.Int); StateAcctNo.Direction = ParameterDirection.Output;
                            SqlParameter TimeToDownload = new SqlParameter("@rsTimeToDownload", SqlDbType.VarChar, 200); TimeToDownload.Direction = ParameterDirection.Output;

                            cmd.Parameters.Add(Channel);
                            cmd.Parameters.Add(AcctNo);
                            cmd.Parameters.Add(StateAcctNo);
                            cmd.Parameters.Add(TimeToDownload);
                            var dr = cmd.ExecuteReader();

                            dr.Close();
                            con.Close();

                            atr.AcctNo = Convert.ToInt32(cmd.Parameters["@rsAccountNo"].Value);
                            atr.StateAcctNo = Convert.ToInt32(cmd.Parameters["@rnStateAccountNo"].Value);
                            atr.TimeToDownload = cmd.Parameters["@rsTimeToDownload"].Value.ToString();



                        }
                        atrlist.Add(atr);
                    }
                    var ret = ListToDataTable<MT940Setup>(atrlist);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                MT940Model.WriteToFile(string.Format("Automatic Transaction Ref Error: {0}", ex.Message == null ? ex.InnerException.Message : ex.Message));
                return null;

            }

        }

        private bool SendStatementEmail(string path)
        {
            try
            {

                var db = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["defaultCon"].ToString());
                db.Open();
                var sql = "select * from DangoteMt940config";
                var smtpdetails = db.Query<DangoteMt940config>(sql).FirstOrDefault();
                //string froemail = smtpdetails.FromEmail;
                Attachment attachment;
                attachment = new System.Net.Mail.Attachment(path);
                MailMessage message = new MailMessage(smtpdetails.FromEmail, smtpdetails.ToEmail);
                message.Subject = smtpdetails.MT940MailSubjectFormat;
                smtpdetails.MT940MailBodyFormat = smtpdetails.MT940MailBodyFormat.Replace("{{ACCOUNTNAME}}", "DANGOTE CEMENT SIERRALEONE");
                message.Body = smtpdetails.MT940MailBodyFormat;
                message.IsBodyHtml = true;
                message.Attachments.Add(attachment);
                SmtpClient client = new SmtpClient(smtpdetails.SmtpServer, smtpdetails.SmtpPort);
                smtpdetails.SmtpPassword = String.IsNullOrEmpty(smtpdetails.SmtpPassword) ? null : MT940Model.Decrypt(smtpdetails.SmtpPassword);
                client.Credentials = new System.Net.NetworkCredential(smtpdetails.FromEmail, smtpdetails.SmtpPassword);
                client.EnableSsl = (bool)smtpdetails.SslRequred;
                
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    MT940Model.WriteToFile("Exception caught in mail sending " + ex.ToString());
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

//        private List<AseParameter> GetAseParameters()
//        {
//            string acctno = System.Configuration.ConfigurationManager.AppSettings["acctno"].ToString();
//            string accttype = System.Configuration.ConfigurationManager.AppSettings["accttype"].ToString();
//            DateTime Fromdate = DateTime.Now.AddDays(-1);
//            string frmdate = string.Format("{0:yyyyMMdd}", Fromdate);
//            decimal frmAmount = 0;
//            decimal toAmount = 9999999999999999999;
//            MT940Model.WriteToFile("Service is recall at " + DateTime.Now);
//            List<AseParameter> sp = new List<AseParameter>()
//{
//                        new AseParameter() { ParameterName = "@psAcctNo", AseDbType = AseDbType.VarChar, Value = acctno },
//                        new AseParameter() { ParameterName = "@psAcctType", AseDbType = AseDbType.VarChar, Value = accttype },
//                        new AseParameter() { ParameterName = "@pdtStartDate", AseDbType = AseDbType.Date, Value =   frmdate },
//                        new AseParameter() { ParameterName = "@pdtEndDate", AseDbType = AseDbType.Date, Value =     frmdate },
//                        new AseParameter() { ParameterName = "@pnFromAmount", AseDbType = AseDbType.Decimal, Value = frmAmount } ,
//                        new AseParameter() { ParameterName = "@pnToAmount", AseDbType = AseDbType.Decimal, Value = toAmount }
//                        };
//            return sp;
//        }
//        private List<AseParameter> GetAseParametersMini()
//        {
//            string acctno = System.Configuration.ConfigurationManager.AppSettings["acctno"].ToString();
//            string accttype = System.Configuration.ConfigurationManager.AppSettings["accttype"].ToString();

//            MT940Model.WriteToFile("Service is recall at " + DateTime.Now);
//            List<AseParameter> sp = new List<AseParameter>()
//                        {
//                        new AseParameter() { ParameterName = "@psAcctNo", AseDbType = AseDbType.VarChar, Value = acctno },
//                        new AseParameter() { ParameterName = "@psAcctType", AseDbType = AseDbType.VarChar, Value = accttype }
//                        };
//            return sp;
//        }

        //private void DownloadMT940(List<AccountStatementDownloadModel> mt940, string AccountNo)
        //{
        //    //List<AccountStatementDownloadModel> mt940 = new List<AccountStatementDownloadModel>();
        //    // mt940 = 
        //    try
        //    {
        //        string MT940Msg = MT940Mapper.Generate(mt940);

        //        MemoryStream ms = new MemoryStream();
        //        TextWriter tw = new StreamWriter(ms);
        //        tw.WriteLine(MT940Msg);
        //        tw.Flush();
        //        byte[] bytes = ms.ToArray();
        //        ms.Close();

        //        System.Web.HttpContext.Current.Response.Charset = "";
        //        HttpContext.Current.Response.Clear();
        //        HttpContext.Current.Response.ContentType = "application/octet-stream";
        //        System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("{0}_{1}", AccountNo, string.Format("{0:dd_MMM_yyyy}", DateTime.Now)));
        //        HttpContext.Current.Response.BinaryWrite(bytes);
        //        HttpContext.Current.Response.End();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    //Mapper.Generat
        //    //Get your string and pass to a stream 
        //    //and download as txt

        //}
    }
}
