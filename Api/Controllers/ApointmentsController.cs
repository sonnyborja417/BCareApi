using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Api.Hubs;
using Api.Models;
using Api.Models.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Api.Controllers
{
    // https://www.c-sharpcorner.com/article/aes-encryptiondecryption-with-angular-7/
    //TO DO! AES https://searchsecurity.techtarget.com/definition/Advanced-Encryption-Standard
    // https://www.c-sharpcorner.com/UploadFile/4d9083/encrypt-in-javascript-and-decrypt-in-C-Sharp-with-aes-algorithm-i/

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Apointments")]
    public class ApointmentsController : Controller
    {
        private bcareContext context;
        private IHubContext<NotificationHub> hubContext;

        public ApointmentsController(bcareContext context, IHubContext<NotificationHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }


        [HttpGet]
        public IActionResult Apointments()
        {
            var apointments = this.context.Apointment
                .Include(e => e.Diagnosis)
                .Include(e => e.FkPatient)
                .ToList();

            return Ok(new ResponseResult<List<Apointment>>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointments
            });
        }

        //rejected , approved filter done in UI
        [HttpGet("open")]
        public IActionResult OpenApointments()
        {
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day);

            var apointments = this.context.Apointment
                .Include(e => e.Diagnosis)
                .Include(e => e.FkPatient)
                .Where(e =>  e.IsClosed == false )
                //.Where(e => e.IsClosed == false && (e.ApointmentDate >= date))
                //.OrderByDescending(e => e.ApointmentDate)
                .OrderBy(e => e.ApointmentDate)
                .ToList();

            return Ok(new ResponseResult<List<Apointment>>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointments
            });
        }

        [HttpGet("{id}")]
        public IActionResult Apointments(Guid id)
        {
            var apointment = this.context.Apointment
                .Include(e => e.Diagnosis)
                .Include(e => e.FkPatient)
                .Include(e=> e.ApointmentUpdates)
                .FirstOrDefault(e => e.PkApointmentId == id);

            return Ok(new ResponseResult<Apointment>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointment
            });
        }

        [HttpGet("Patient/{id}")]
        public IActionResult PatientApointments(Guid id)
        {
            var apointments = this.context.Apointment
                .Include(e => e.Diagnosis)
                .Include(e => e.FkPatient)
                .Include( e => e.ApointmentUpdates)
                .Where(e => e.FkPatientId == id)
                .OrderBy(e => e.ApointmentDate)
                //.OrderByDescending(e => e.ApointmentDate)
                .ToList();

            return Ok(new ResponseResult<List<Apointment>>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointments
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Apointment apointment)
        {
            this.context.Apointment.Add(apointment);
            this.context.SaveChanges();

            this.hubContext.Clients.All.SendAsync("AppointmentCreated", new
            {
                AppointmentId = apointment.PkApointmentId,
                PatientId = apointment.FkPatientId
            });

            return Ok(new ResponseResult<Guid>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointment.PkApointmentId
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] Apointment apointment)
        {
            if ( apointment.IsClosed)
                apointment.ClosingDate = DateTime.Now;

            this.context.Apointment.Update(apointment);
            this.context.SaveChanges();

            return Ok(new ResponseResult<Guid>
            {
                Status = ResponseResultStatus.Ok,
                Body = apointment.PkApointmentId
            });
        }

        [HttpPost("Sms")]
        public IActionResult SendSMS([FromBody] MessageModel fkMessage)
        {
            fkMessage.PkMessageId = Guid.NewGuid();
              
            var message = new CryptoAES().OpenSSLDecrypt(fkMessage.Message, "helloworld");

            //TR-SONNY136939_EMLE4
            try
            {
                foreach(var personid in fkMessage.Contactlist)
                {
                    var person = this.context.Person.Where(p => p.PkPersonId == personid).FirstOrDefault();
                    if ( person != null && !string.IsNullOrEmpty(person.ContactNo))
                    {
                        var thisnumber = person.ContactNo.Replace("-", "").Replace("+63","0").Replace(" ","");
                        //itexmo(thisnumber, fkMessage.Message);

                        itexmo(thisnumber, message);

                        //if ( IsPhoneNumber(thisnumber))
                        //{
                        //    itexmo(thisnumber, fkMessage.Message);
                        //}
                    }
                }

                return Ok(new ResponseResult<Guid>
                {
                    Status = ResponseResultStatus.Ok,
                    Body = fkMessage.PkMessageId,
                    Message = message
                });
            }
            catch(Exception ex)
            {
                return Ok(new ResponseResult<Guid>
                {
                    Status = ResponseResultStatus.Ok,
                    Body = fkMessage.PkMessageId,
                    Message = ex.Message
                });
            }

            
        }

        //public object itexmo(string Number, string Message, string API_CODE = "TR-SONNY136939_EMLE4")
        //{
        //    object functionReturnValue = null;
        //    using (System.Net.WebClient client = new System.Net.WebClient())
        //    {
        //        System.Collections.Specialized.NameValueCollection parameter = new System.Collections.Specialized.NameValueCollection();
        //        string url = "https://www.itexmo.com/php_api/api.php";
        //        parameter.Add("1", Number);
        //        parameter.Add("2", Message);
        //        parameter.Add("3", API_CODE);
        //        dynamic rpb = client.UploadValues(url, "POST", parameter);
        //        functionReturnValue = (new System.Text.UTF8Encoding()).GetString(rpb);
        //    }
        //    return functionReturnValue;
        //}

        public object itexmo(string Number, string Message, string ApiCode = "TR-MOLSK684525_5GQHX" , string ApiPassword = "!f6zfhdq1&")
        {
            object functionReturnValue = null;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                System.Collections.Specialized.NameValueCollection parameter = new System.Collections.Specialized.NameValueCollection();
                string url = "https://www.itexmo.com/php_api/api.php";
                parameter.Add("1", Number);
                parameter.Add("2", Message);
                parameter.Add("3", ApiCode);
                parameter.Add("passwd", ApiPassword);
                dynamic rpb = client.UploadValues(url, "POST", parameter);
                functionReturnValue = (new System.Text.UTF8Encoding()).GetString(rpb);
            }
            return functionReturnValue;
        }


        public bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\[0-9]{11})$").Success;
        }

    }

    public class CryptoAES
    {                

        //Decryption
        public string OpenSSLDecrypt(string encrypted, string passphrase)
        {
            // base 64 decode
            byte[] encryptedBytesWithSalt = Convert.FromBase64String(encrypted);
            // extract salt (first 8 bytes of encrypted)
            byte[] salt = new byte[8];
            byte[] encryptedBytes = new byte[encryptedBytesWithSalt.Length - salt.Length - 8];
            Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length);
            Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);
            // get key and iv
            byte[] key, iv;
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            return DecryptStringFromBytesAes(encryptedBytes, key, iv);
        }

        private static void DeriveKeyAndIV(string passphrase, byte[] salt, out byte[] key, out byte[] iv)
        {
            // generate key and iv
            List<byte> concatenatedHashes = new List<byte>(48);
            byte[] password = Encoding.UTF8.GetBytes(passphrase);
            byte[] currentHash = new byte[0];
            MD5 md5 = MD5.Create();
            bool enoughBytesForKey = false;
            // See http://www.openssl.org/docs/crypto/EVP_BytesToKey.html#KEY_DERIVATION_ALGORITHM
            while (!enoughBytesForKey)
            {
                int preHashLength = currentHash.Length + password.Length + salt.Length;
                byte[] preHash = new byte[preHashLength];
                Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                Buffer.BlockCopy(password, 0, preHash, currentHash.Length, password.Length);
                Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + password.Length, salt.Length);
                currentHash = md5.ComputeHash(preHash);
                concatenatedHashes.AddRange(currentHash);
                if (concatenatedHashes.Count >= 48)
                    enoughBytesForKey = true;
            }
            key = new byte[32];
            iv = new byte[16];
            concatenatedHashes.CopyTo(0, key, 0, 32);
            concatenatedHashes.CopyTo(32, iv, 0, 16);
            md5.Clear();
            md5 = null;
        }
       
        static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;
            // Declare the string used to hold
            // the decrypted text.
            string plaintext;
            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            srDecrypt.Close();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return plaintext;
        }
    }

}