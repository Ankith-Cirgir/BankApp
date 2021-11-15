using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model
{
    public class Bank
    {
        public float Profits { get; set; } //EXTRA
        public string Id { get; set; }
        public string Name { get; set; }

        public float sRTGSCharge { get; set; }

        public float sIMPSCharge { get; set; }

        public float oRTGSCharge { get; set; }

        public float oIMPSCharge { get; set; }


        public Bank(string bankName,float sRTGS, float sIMPS, float oRTGS, float oIMPS)
        {
            this.Name = bankName;
            this.Id = $"{bankName.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}";
            this.sRTGSCharge = sRTGS;
            this.sIMPSCharge = sIMPS;
            this.oRTGSCharge = oRTGS;
            this.oIMPSCharge = oIMPS;

        }

        public Bank(string bankName)
        {
            this.Name = bankName;
            this.Id = $"{bankName.Substring(0, 3)}{DateTime.Now.ToString("ddMMyyyy")}";
            this.sRTGSCharge = 0;
            this.sIMPSCharge = 5;
            this.oRTGSCharge = 2;
            this.oIMPSCharge = 6;

        }
    }
}
