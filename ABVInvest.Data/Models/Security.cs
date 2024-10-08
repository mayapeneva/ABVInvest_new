using ABVInvest.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class Security : BaseEntity<int>
    {
        private int typeCodeStartIndex = 2;
        private int typeCodeLenght = 2;

        [Required]
        public virtual Issuer Issuer { get; set; }
        public int IssuerId { get; set; }

        public SecuritiesType? SecuritiesType { get; private set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string BfbCode { get; set; }

        public virtual Currency Currency { get; set; }

        public void SetSecuritiesType()
        {
            if (this.ISIN.StartsWith("BG"))
            {
                var typeCode = this.ISIN.Substring(this.typeCodeStartIndex, this.typeCodeLenght);
                if (typeCode == "11")
                {
                    this.SecuritiesType = (SecuritiesType)1;
                }
                else if (typeCode == "40")
                {
                    this.SecuritiesType = (SecuritiesType)2;
                }
                else if (typeCode == "21")
                {
                    this.SecuritiesType = (SecuritiesType)3;
                }
                else if (typeCode == "90")
                {
                    this.SecuritiesType = (SecuritiesType)4;
                }
                else if (typeCode == "92")
                {
                    this.SecuritiesType = (SecuritiesType)5;
                }
                else
                {
                    this.SecuritiesType = (SecuritiesType)6;
                }
            }
        }
    }
}
