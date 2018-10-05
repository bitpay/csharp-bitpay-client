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
        
                if (!bitpay.clientIsAuthorized(BitPay.FacadeMerchant))
                {
                    // Get POS facade authorization code.
                    // Obtain a pairingCode from the BitPay server.  The pairingCode must be emitted from
        	        // this device and input into and approved by the desired merchant account.  To
        	        // generate invoices a POS facade is required.
                    String pairingCode = bitpay.requestClientAuthorization(BitPay.FacadeMerchant);

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
                Invoice invoice = bitpay.createInvoice(new Invoice(1.0, "USD"), BitPay.FacadeMerchant);
                invoice = bitpay.getInvoice(invoice.Id, BitPay.FacadeMerchant);
                Assert.IsNotNull(invoice.Id, "Invoice created with id=NULL");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldGetInvoices()
        {
            try
            {
                List<Invoice> invoices = bitpay.getInvoices(new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
                Assert.IsTrue(invoices.Count > 0, "No invoices retrieved");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldGetBTCLedger()
        {
            Ledger ledger = null;
            try
            {
                ledger = this.bitpay.getLedger(Ledger.LEDGER_BTC, new DateTime(2014, 8, 1), new DateTime(2014, 8, 31));
                Assert.IsTrue(ledger.Entries.Count > 0, "Ledger is empty");
            }
            catch (Exception ex1)
            {
                if (!ex1.Message.Contains("Ledger is empty"))
                {
                    Assert.Fail(ex1.Message);
                }
                else
                {
                    try
                    {
                        Assert.IsTrue(ledger.Entries.Count == 0);
                    }
                    catch (Exception ex2)
                    {
                        Assert.Fail(ex2.Message);
                    }
                }
            }
        }

        [TestMethod]
        public void testShouldGetUSDLedger()
        {
            Ledger ledger = null;
            try
            {
                ledger = this.bitpay.getLedger(Ledger.LEDGER_USD, new DateTime(2014, 1, 1), new DateTime(2014, 1, 31));
                Assert.IsTrue(ledger.Entries.Count > 0, "Ledger is empty");
            }
            catch (Exception ex1)
            {
                if (!ex1.Message.Contains("Ledger is empty"))
                {
                    Assert.Fail(ex1.Message);
                }
                else
                {
                    try
                    {
                        Assert.IsTrue(ledger.Entries.Count == 0);
                    }
                    catch (Exception ex2)
                    {
                        Assert.Fail(ex2.Message);
                    }
                }
            }
        }
    }
}
