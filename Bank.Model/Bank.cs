using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Bank
    {
        public float Profits { get; set; }
        [MaxLength(45)]
        public string BankId { get; set; }
        [MaxLength(45)]
        public string BankName { get; set; }

        public float sRTGSCharge { get; set; }

        public float sIMPSCharge { get; set; }

        public float oRTGSCharge { get; set; }

        public float oIMPSCharge { get; set; }

        /*

        public Bank(string bankName,float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            this.BankName = bankName;
            this.BankId = $"{bankName.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}";
            this.sRTGSCharge = sRTGS;
            this.sIMPSCharge = sIMPS;
            this.oRTGSCharge = oRTGS;
            this.oIMPSCharge = oIMPS;

        }

        public Bank(string bankName)
        {
            this.BankName = bankName;
            this.BankId = $"{bankName.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}";
            this.sRTGSCharge = 0;
            this.sIMPSCharge = 5;
            this.oRTGSCharge = 2;
            this.oIMPSCharge = 6;

        }*/
    }
}
