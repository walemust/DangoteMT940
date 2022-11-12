using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DangoteMT940
{
    public class MT940Mapper
    {
        private static string GenerateReference4Lenght()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 10000000;
            return string.Format("{0:D7}", random);
        }
        public static string Generate940String(System.Data.DataTable ds)
        {
            string statement = string.Empty;
            MT940 mt940 = new MT940();
            mt940.Body = new List<MT940B>();
            //{1:F01ZEIBNGLAXXXX0000000000}{2:I940ZEIBNGLAXXXXN}{4:
            //:20:2021091005101660
            //:25:X01xxxx57x
            //: 28C: 2318 / 1
            //:60F:D210909NGNxxxxxxxxx,68
            //:61:2109090909CN5508000,00NCOL3554X269//355496X269
            //: 86:355X960269 - CHICANO DYNAMIC RESOURCES NIG.- 00010XX368
            //: 61:2109090909CN5508000,00NCMI4451930997//4451930997
            //: 86:4451930997 - ETHELBART IFIONU - 0001006103
            //:62F:D210909NGNxxxxxxxxx,68
            //:64:D210909NGNxxxxxxxxx,68
            //-}
            try
            {
                string DateString = ((DateTime)DateTime.Now).ToString("yyMMdd");
                //string append = "ZES";
                Random rand = new Random();//ZESLSLFR
                string b1 = string.Format("{0}{1}{2}{3}", "F", "01", "ZESLSLFR" + "XXXX", "0000000000");
                string b2 = string.Format("{0}{1}{2}{3}", "I", "940", "ZESLSLFR", "N");
                int Seqnum = 1;
                int Stmtnum = 0;
                DateTime todaysdate = DateTime.Now;
                mt940.F20 = string.Format("{0}\r\n", DateTime.Now.ToString("yyyyMMddhhmmss") + rand.Next(10, 99).ToString()); //DateTime + RandNumber 16x
                mt940.F25 = ds.Rows[0]["AcctNo"].ToString();// [0].AccountNumber; //
                #region tablecheck
                //var sql = "select * from DangoteMt940config";
                //using var db = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["defaultCon"].ToString());
                //db.Open();
                //var con = db.Query<DangoteMt940config>(sql).FirstOrDefault();
                //todaysdate = DateTime.Now;
                //if (con != null)
                //{
                //    //Reset Statement number at the beginning of the year
                //    if (todaysdate.Month == 1 && todaysdate.Day == 1)
                //    {
                //        if (con.DateCreated.ToString("yyyy/MM/dd") != DateTime.Now.ToString("yyyy/MM/dd"))
                //        {
                //            Stmtnum = 1;
                //            Seqnum = 1;
                //        }
                //        else
                //        {
                //            Seqnum = con.SequentialNumber + 1;
                //            Stmtnum = 1;
                //        }
                //        var parameters = new { SequentialNumber = Seqnum, StatementNumber = Stmtnum, DateCreated = DateTime.Now };
                //        var sql2 = "update DangoteMt940config set SequentialNumber =@SequentialNumber, StatementNumber = @StatementNumber, DateCreated = @DateCreated";
                //        var con2 = db.Execute(sql2, parameters);

                //    }
                //    else
                //    {
                //        if (con.DateCreated.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
                //        {
                //            Seqnum = con.SequentialNumber + 1;
                //            Stmtnum = con.StatementNumber;
                //        }
                //        else
                //        {
                //            Stmtnum = con.StatementNumber + 1;
                //            Seqnum = 1;
                //        }
                //        var parameters = new { SequentialNumber = Seqnum, StatementNumber = Stmtnum, DateCreated = todaysdate };
                //        var sql2 = "update DangoteMt940config set SequentialNumber =@SequentialNumber, StatementNumber = @StatementNumber, DateCreated = @DateCreated";
                //        var con2 = db.Execute(sql2, parameters);
                //    }
                //}
                //else
                //{
                //    Seqnum = 1;
                //    Stmtnum = 1;
                //    var parameters = new { SequentialNumber = 1, StatementNumber = 1, DateCreated = DateTime.Now };
                //    var sql2 = "insert into DangoteMt940config(StatementNumber,SequentialNumber,DateCreated) values (@SequentialNumber, @StatementNumber, @DateCreated)";
                //    var con2 = db.Execute(sql2, parameters);
                //}
                #endregion
                string StatNo = todaysdate.DayOfYear.ToString("000");
                mt940.F28C = StatNo + "/" + Seqnum;
                //60F:D210909NGNxxxxxxxxx,68
                foreach (DataRow row in ds.Rows)
                {
                    string crncy = row["Currency"].ToString().Substring(2, 1);
                    //if (Convert.ToBoolean(row["FTag_60"]))
                    if (row["FTag_60"].ToString() == "Y")
                    {
                        string drcr = row["DebitCredit"].ToString();
                        string valdate = row["ValueDate"].ToString();//.("yyMMdd");
                        string Oamount = row["TransactionAmount"].ToString().Replace(".", ",");
                        string currency = row["Currency"].ToString();
                        if (!Oamount.Contains(","))
                        {
                            Oamount += ",00";
                        }
                        mt940.F60F = string.Format("{0}{1}{2}{3}\r\n", drcr, valdate, currency, Oamount); //Opening Balance
                    }
                    else if (row["FTag_62"].ToString() == "Y")
                    {
                        //62F:D210909NGNxxxxxxxxx,68
                        string drcr = row["DebitCredit"].ToString();
                        string valdate = row["ValueDate"].ToString();//.("yyMMdd");
                        string Oamount = row["TransactionAmount"].ToString().Replace(".", ",");
                        string currency = row["Currency"].ToString();
                        if (!Oamount.Contains(","))
                        {
                            Oamount += ",00";
                        }
                        mt940.F62F = string.Format("{0}{1}{2}{3}\r\n", drcr, valdate, currency, Oamount); //Closing Balance
                    }
                    else if (row["FTag_64"].ToString() == "Y")
                    {
                        string drcr = row["DebitCredit"].ToString();
                        string valdate = row["ValueDate"].ToString();//.("yyMMdd");
                        string Oamount = row["TransactionAmount"].ToString().Replace(".", ",");
                        string currency = row["Currency"].ToString();
                        if (!Oamount.Contains(","))
                        {
                            Oamount += ",00";
                        }
                        mt940.F64 = string.Format("{0}{1}{2}{3}\r\n", drcr, valdate, currency, Oamount); //Available Balance
                    }
                    else if (row["FTag_61"].ToString() == "Y")
                    {
                        //61:2109090909CN5508000,00NCOL3554X269//355496X269
                        //: 86:355X960269 - CHICANO DYNAMIC RESOURCES NIG.- 00010XX368
                        MT940B item = new MT940B();
                        string drcr = row["DebitCredit"].ToString();
                        string valdate = row["ValueDate"].ToString();//.("yyMMdd");
                        string seconddtpart = valdate.Substring(2, 4);
                        string tranamt = row["TransactionAmount"].ToString().Replace(".", ",");
                        string trantype = row["TransactionType"].ToString();
                        string transref = row["ReferenceNo"].ToString();
                        string custname = row["T86_CustName"].ToString();
                        string custcode = row["T86_CustCode"].ToString();
                        string slash = @"//";
                        if (!tranamt.Contains(","))
                        {
                            tranamt += ",00";
                        }

                        item.F61 = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}\r\n", valdate, seconddtpart, drcr, crncy, tranamt, trantype, transref, slash, transref);
                        item.F86 = string.Format("{0}{1}{2}{3}{4}\r\n", transref, "-", custname, "-", custcode);

                        mt940.Body.Add(item);

                    }
                }
                statement = mt940.GenerateRTGS(mt940, b1, b2, ds.Rows[0]["AcctNo"].ToString());

            }
            catch (Exception ex)
            {
                MT940Model.WriteToFile(string.Format("rror: {0}", ex.Message == null ? ex.InnerException.Message : ex.Message));
            }

            return statement;

        }
    }
}
