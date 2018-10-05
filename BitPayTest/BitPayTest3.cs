using BitPayAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BitPayTest
{
    [TestClass]
    public class BitPayTest3
    {
        private BitPay bitpay;
        private static String clientName = "BitPay C# Library Tester on " + System.Environment.MachineName;

        public BitPayTest3()
        {
            try
            {
                bitpay = new BitPay(clientName);
        
                if (!bitpay.clientIsAuthorized(BitPay.FacadePayroll))
                {
                    // Get PAYROLL facade authorization.
                    // Obtain a pairingCode from your BitPay account administrator.  When the pairingCode
                    // is created by your administrator it is assigned a facade.  To generate payout batches a
                    // PAYROLL facade is required.

                    // As an alternative to this client outputting a pairing code, the BitPay account owner
                    // may interactively generate a pairing code via the BitPay merchant dashboard at
                    // https://[test].bitpay.com/dashboard/merchant/api-tokens.  This client can subsequently
                    // accept the pairing code using the following call.

                    // bitpay.authorizeClient(pairingCode);

                    String pairingCode = bitpay.requestClientAuthorization(BitPay.FacadePayroll);

                    // Signal the device operator that this client needs to be paired with a merchant account.
                    System.Diagnostics.Debug.WriteLine("Info: Pair this client with your merchant account using the pairing code: " + pairingCode);
                    System.Console.WriteLine("Info: Pair this client with your merchant account using the pairing code: " + pairingCode);
                    throw new BitPayException("Error: client is not yet authorized, pair this client with your BitPay merchant account using the pairing code: " + pairingCode);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldSubmitPayoutBatch()
        {
            try
            {
                DateTime date = DateTime.Now;
                DateTime threeDaysFromNow = date.AddDays(3);

                DateTime effectiveDate = threeDaysFromNow;
                String reference = "My test batch";
                String bankTransferId = "My bank transfer id";
                String currency = "USD";
                List<PayoutInstruction> instructions = new List<PayoutInstruction>() {
                    new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
                    new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
                };

                PayoutBatch batch = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
                batch = this.bitpay.submitPayoutBatch(batch);

                Assert.IsNotNull(batch.Id, "Batch created with id=NULL");
                Assert.IsTrue(batch.Instructions.Count == 2);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void testShouldSubmitGetAndDeletePayoutBatch()
        {
            try
            {
                DateTime date = DateTime.Now;
                DateTime threeDaysFromNow = date.AddDays(3);

                DateTime effectiveDate = threeDaysFromNow;
                String reference = "My test batch";
                String bankTransferId = "My bank transfer id";
                String currency = "USD";
                List<PayoutInstruction> instructions = new List<PayoutInstruction>() {
                    new PayoutInstruction(100.0, "mtHDtQtkEkRRB5mgeWpLhALsSbga3iZV6u", "Alice"),
                    new PayoutInstruction(200.0, "mvR4Xj7MYT7GJcL93xAQbSZ2p4eHJV5F7A", "Bob")
                };

                PayoutBatch batch0 = new PayoutBatch(currency, effectiveDate, bankTransferId, reference, instructions);
                batch0 = this.bitpay.submitPayoutBatch(batch0);

                Assert.IsNotNull(batch0.Id, "Batch (0) created with id=NULL");
                Assert.IsTrue(batch0.Instructions.Count == 2);

                PayoutBatch batch1 = this.bitpay.getPayoutBatch(batch0.Id);

                Assert.IsNotNull(batch1.Id, "Batch (1) created with id=NULL");
                Assert.IsTrue(batch1.Instructions.Count == 2);

                this.bitpay.cancelPayoutBatch(batch0.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
