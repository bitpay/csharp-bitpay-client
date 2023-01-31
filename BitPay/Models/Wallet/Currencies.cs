using Newtonsoft.Json;

namespace BitPay.Models.Wallet
{
    public class Currencies
    {
        public string Code { get; set; }
        public string WithdrawalFee { get; set; }

        [JsonProperty(PropertyName = "p2p")]
        public bool P2P { get; set; }

        [JsonProperty(PropertyName = "dappBrowser")]
        public bool DappBrowser { get; set; }

        [JsonProperty(PropertyName = "walletConnect")]
        public bool WalletConnect { get; set; }

        [JsonProperty(PropertyName = "payPro")]
        public bool PayPro { get; set; }
        
        public Qr Qr { get; set; }
        
        public string Image { get; set; }

        public bool ShouldSerializeCode()
        {
            return !string.IsNullOrEmpty(Code); 
        }

        public bool ShouldSerializeWithdrawalFee()
        {
            return !string.IsNullOrEmpty(WithdrawalFee); 
        }

        public bool ShouldSerializeP2P()
        {
            return false; 
        }

        public bool ShouldSerializeDappBrowser()
        {
            return false; 
        }

        public bool ShouldSerializeWalletConnect()
        {
            return false; 
        }

        public bool ShouldSerializePayPro()
        {
            return false;
        }

        public bool ShouldSerializeQr()
        {
            return false;
        }
    }
}
