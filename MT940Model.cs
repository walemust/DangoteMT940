using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DangoteMT940
{
    public static class MT940Model
    {
        private static Object LockObject = new Object();
        private static string fileName = System.Configuration.ConfigurationManager.AppSettings["LogFile"];
        private static string filePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
        private static string fileSize = System.Configuration.ConfigurationManager.AppSettings["LogSize"];
        public static void WriteToFile(string x)
        {
            FileInfo f = new FileInfo(fileName);

            if (File.Exists(fileName))
            {
                long s1 = f.Length;
                if (s1 > Convert.ToInt32(fileSize))
                {
                    string filename = Path.GetFileNameWithoutExtension(fileName) + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".txt";
                    //filename = GetNextFilename(filename);
                    if (File.Exists(Path.Combine(filePath, filename)))
                        File.Delete(Path.Combine(filePath, filename));

                    File.Move(fileName, Path.Combine(filePath, filename));

                    f.Delete();
                }
            }
            lock (LockObject)
            {
                string sError = DateTime.Now.ToString() + ": " + x;
                StreamWriter sw = new StreamWriter(fileName, true, Encoding.ASCII);
                sw.WriteLine(sError);
                sw.Close();
            }

        }

        public static string DecryptPswd(string pwd)
        {
            if (!string.IsNullOrEmpty(pwd))
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(pwd);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            return "";
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
    public class MT940
    {

        /// <summary>
        /// Sender's Reference (M) Statement Marker
        /// :20:Ref254 (16x) :20:YYYYMMDD
        /// </summary>
        public string F20 { get; set; }

        /// <summary>
        /// Account Identification (M) account number
        /// :25:Ref254 (35x)
        /// </summary>
        public string F25 { get; set; }

        /// <summary>
        /// StatementNumber/SequenceNumber (M)
        /// :28C:1/1 5n[/5n] 
        /// This field contains the sequential number of the statement, optionally followed by the sequence number of the message
        ///  within that statement when more than one message is sent for one statement. 
        /// The statement number should be reset to 1 on 1 January of each year. 
        /// The sequence number always starts with 001. When several messages are sent to convey information about a single statement, 
        /// the first message must contain '/001' in Sequence Number.
        ///  One SWIFT message may contain up to 2000 characters.The sequence number must be incremented by one for each additional message
        /// e.g. :28C:00027/001
        /// </summary>
        public string F28C { get; set; }

        /// <summary>
        /// OpeningBalance_First (M) (1!a6!n3!a15d)
        /// :60F:Debit/Credit | Last Statement Date (YYMMDD)  | Currency (ISO) | Amount C210909NGNxxxxxxxxx,68
        /// </summary>
        public string F60F { get; set; }

        public List<MT940B> Body { get; set; }

        /// <summary>
        /// ClosingBalance_BookedFunds_First (M) (1!a6!n3!a15d)
        /// :62F:Debit/Credit | Last Statement Date (YYMMDD)  | Currency (ISO) | Amount 
        /// </summary>
        public string F62F { get; set; }

        /// <summary>
        /// ClosingAvailableBalance_AvailableFunds (O) (1!a6!n3!a15d)
        /// :62M:Debit Balance /Credit Balance |  Date (YYMMDD)  | Currency (ISO) | Amount 
        /// </summary>
        public string F64 { get; set; }

        public string GenerateRTGS(MT940 mt, string B1, string B2, string AccountNo)
        {
            string white = "";
            //var errorhandler = new ErrorLogHandler();
            MT940Model.WriteToFile(string.Format("From GenerateRTGS "));
            Random rand = new Random();
            string m20, m25, m28C, m60F, m61, m86, m62F, m64;
            StringBuilder sb = new StringBuilder();
            try
            {
                string _1, _2;

                if (mt != null)
                {
                    MT940Model.WriteToFile(string.Format("MT is not null"));
                    m20 = string.Format(":20:{0}", mt.F20);
                    //Bank Operation Code bOperationCode
                    m25 = string.Format(":25:{0}\r\n", mt.F25);
                    //Transaction Type Code tTypeCode
                    m28C = string.Format(":28C:{0}\r\n", mt.F28C);
                    //string.Format(":71A:{0}", chargeOption(SHA,BEN,OUR));
                    m60F = string.Format(":60F:{0}", mt.F60F);

                    _1 = string.Format("{{1:{0}}}", B1);
                    _2 = string.Format("{{2:{0}}}", B2);

                    var fileName = AccountNo + "-1" + ".213";


                    sb.Append(_1);
                    sb.Append(_2);
                    sb.AppendFormat("{{4:\r\n{0}{1}{2}{3}}}", m20, m25, m28C, m60F).ToString();
                    var index = sb.ToString().LastIndexOf('}');
                    if (index >= 0)
                        sb.Remove(index, 1);
                    MT940Model.WriteToFile(string.Format("MT body count {0}", mt.Body.Count));
                    if (mt.Body.Count > 0)
                    {

                        foreach (MT940B bitem in mt.Body)
                        {
                            m61 = string.Format(":61:{0}", bitem.F61);
                            m86 = string.Format(":86:{0}", bitem.F86);

                            sb.AppendFormat("{0}{1}", m61, m86);
                        }
                    }
                    m62F = string.Format(":62F:{0}", mt.F62F);
                    m64 = string.Format(":64:{0}", mt.F64);
                    sb.AppendFormat("{0}{1}{2}", m62F, m64, "-}");

                    if (sb.Length < 510)
                    {
                        int length = 510 - sb.Length;
                        char empty = ' ';
                        white = white.PadRight(length, empty);
                        sb.Append(white);
                    }
                    else if (sb.Length > 510 && sb.Length < 1022)
                    {
                        int length = 1022 - sb.Length;
                        char empty = ' ';
                        white = white.PadRight(length, empty);
                        sb.Append(white);
                    }
                    else if (sb.Length > 1022 && sb.Length < 2046)
                    {
                        int length = 2046 - sb.Length;
                        char empty = ' ';
                        white = white.PadRight(length, empty);
                        sb.Append(white);
                    }
                    else if (sb.Length > 2046 && sb.Length < 4094)
                    {
                        int length = 4094 - sb.Length;
                        char empty = ' ';
                        white = white.PadRight(length, empty);
                        sb.Append(white);
                    }


                }
            }
            catch (Exception ex)
            {
                MT940Model.WriteToFile(string.Format("Error from GenerateRTGS {0}", ex.Message == null ? ex.InnerException.Message : ex.Message));
            }
            //return sb.ToString().Trim();
            return sb.ToString();
        }

    }

    public class MT940B
    {

        /// <summary>
        /// Statement Line (O) (6!n[4!n]2a[1!a]15d1!a3!c16x[//16x] /r/n [34x] )
        /// :60F: 6!n Value Date (YYMMDD) [4!n] Entry Date(MMDD) 2a Debit/Credit Mark
        /// [1!a] Funds Code(3rd character of the currency code, if needed) 15d Amount 
        /// 1!a3!c Transaction Type Identification Code 16x Customer Reference [//16x] Bank Reference 
        /// [34x] Supplementary Details (this will be on a new/separate line)
        /// </summary>
        public string F61 { get; set; }

        /// <summary>
        /// Information to Account Owner (0)
        /// :60F:Ref254 (6x65x)
        /// </summary>
        public string F86 { get; set; }

    }
    public class admConnectionSetup
    {
        public int Itbid { get; set; }
        public string Name { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string Databasename { get; set; }
        public int Userid { get; set; }
        public string DateCreated { get; set; }
        public string Status { get; set; }

    }

    public class DangoteCollection
    {
        public long Itbid { get; set; }
        public string Account_no { get; set; }
        public string Account_name { get; set; }
        public string Account_type { get; set; }
        public string customer_number { get; set; }
        public string customer_name { get; set; }
        public string amount { get; set; }
        public int deposit_slip_no { get; set; }
        public string date_created { get; set; }

    }

    public class DangoteMt940config
    {
        public int Itbid { get; set; }
        public int StatementNumber { get; set; }
        public int SequentialNumber { get; set; }
        public string SmtpServer { get; set; }
        public short SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public bool SslRequred { get; set; }
        public string MT940MailSubjectFormat { get; set; }
        public string MT940MailBodyFormat { get; set; }
        public int Userid { get; set; }
        public DateTime DateCreated { get; set; }

    }
    public class AutoTransactionRef
    {
        public string transref { get; set; }
        public int errorcode { get; set; }
        public string errormsg { get; set; }
    }
    public class admBankServiceSetUp
    {
        public int Itbid { get; set; }
        public string SmtpServer { get; set; }
        public short SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail2 { get; set; }
        public string FromEmail { get; set; }
        public bool SslRequred { get; set; }
        public short DatabasePort { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string Databasename { get; set; }
        public string RtgsMT103ContentMailFormat { get; set; }
        public string RtgsMT103PdfName { get; set; }
        public string RtgsMT103MessagesMailSubject { get; set; }
        public string RtgsMT103MessagesMailFormat { get; set; }
        public string RtgsMT102PdfName { get; set; }
        public string RtgsMT102MessagesMailSubject { get; set; }
        public string RtgsMT102MessagesMailFormat { get; set; }
        public string SwiftMT103ContentMailFormat { get; set; }
        public string SwiftMT103PdfName { get; set; }
        public string SwiftMT103MessagesMailSubject { get; set; }
        public string SwiftMT103MessagesMailFormat { get; set; }
        public int Userid { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
        public string SwiftMT103DebitAdviceFormat { get; set; }

        public string SwiftMT103DebitAdviceSubject { get; set; }
    }

    public class companyProfile
    {
        public int acctno { get; set; }
        public string CompanyCoode { get; set; }
        public string CompanyName { get; set; }
        public string address { get; set; }
        public long phoneno { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime SessionIdleTime { get; set; }
        public int LoginAttempts { get; set; }
        public string BankingSystem { get; set; }
        public string EmailAddress { get; set; }
        public string AllowAuth { get; set; }
        public string ImageCss { get; set; }
        public string VerifierMailFormat { get; set; }
        public string ApproverMailFormat { get; set; }
        public int BankUserId { get; set; }
        public int BankAuthId { get; set; }
        public string AccountStatus { get; set; }
        public int ClassCode { get; set; }
        public string AcctType { get; set; }
        public int CustNo { get; set; }
        public string NotificationMedium { get; set; }
        public string Status { get; set; }
        public int LocalCrncyUserLimit { get; set; }
        public int FxCrncyUserLimit { get; set; }
        public int LocalCrncyTransactionLimit { get; set; }
        public string ExtensionPeriod { get; set; }
        public string ExtensionPeriodTime { get; set; }
        public string MT940GenerationDate { get; set; }
        public string MT940GenerationCount { get; set; }
    }

    public class MT940Setup
    {
        public int AcctNo { get; set; }
        public int StateAcctNo { get; set; }
        public string TimeToDownload { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string SenderSwiftCode { get; set; }
        public string ReceiverSwiftCode { get; set; }
    }
}
