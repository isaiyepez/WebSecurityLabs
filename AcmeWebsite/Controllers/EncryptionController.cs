using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcmeWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using SecurityUtility;

namespace AcmeWebsite.Controllers
{
    public class EncryptionController : Controller
    {
        public IActionResult Index()
        {
            var model = new EncryptModel() { CipherText = "", PlainText = "", HashCode = "" };
            return View(model);
        }

        
        public IActionResult Encrypt(EncryptModel model)
        {
            var enc = new Encryptor(model.KeyPassword);
            model.CipherText = enc.Encrypt(model.PlainText);
            return View("Index", model);
        }
        public IActionResult Decrypt(EncryptModel model)
        {
            var enc = new Encryptor(model.KeyPassword);
            model.PlainText = enc.Decrypt(model.CipherText);
            return View("Index", model);
        }
        public IActionResult Hash(EncryptModel model)
        {
            var enc = new Encryptor(model.KeyPassword);
            model.HashCode = enc.Hash(model.PlainText, Encoding.Default.GetBytes("MrSalty"));
            return View("Index", model);
        }


    }
}