using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BitPayTest
{
    [TestClass]
    public class BitPayTest2
    {
        private BitPay bitpay;
        private static String clientName = "BitPay C# Library Tester on " + System.Environment.MachineName;

        public BitPayTest2()
        {
            try
            {
		        // If this test has never been run before then this test must be run twice in order to pass.
		        // The first time this test runs it will create an identity and emit a client pairing code.
		        // The pairing code must then be authorized in a BitPay account.  Running the test a second
		        // time should result in the authorized client (this test) running to completion.
                bitpay = new BitPay(clientName);        
        
                if (!bitpay.clientIsAuthorized(BitPay.FACADE_MERCHANT))
                {
                    // Get POS facade authorization code.
                    // Obtain a pairingCode from the BitPay server.  The pairingCode must be emitted from
        	        // this device and input into and approved by the desired merchant account.  To
        	        // generate invoices a POS facade is required.
                    String pairingCode = bitpay.requestClientAuthorization(BitPay.FACADE_MERCHANT);

                    // Signal the device operator that this client needs to be paired with a merchant account.
                    System.Diagnostics.Debug.WriteLine("Info: Pair this client with your merchant account using the pairing code: " + pairingCode);
                    throw new BitPayException("Error: client is not yet authorized, pair this client with your BitPay merchant account using the pairing code: " + pairingCode);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldGetInvoiceId()
        {
            try
            {
                Invoice invoice = bitpay.createInvoice(new Invoice(50.0m, "USD"));
                Assert.IsNotNull(invoice.Id, "Invoice created with id=NULL");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldGetBTCLedger()
        {
            try
            {
                Ledger ledger = this.bitpay.getLedger(Ledger.LEDGER_BTC, new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
                Assert.IsTrue(ledger.Entries.Count > 0, "No invoices returned");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
