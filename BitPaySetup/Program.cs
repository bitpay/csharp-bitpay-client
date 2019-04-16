using System;
using BitPayAPI;

namespace BitPaySetup
{
    class Program
    {
        private static EcKey ecKey = null;
        private static BitPay bitpay = new BitPay();
        private static string env = Env.TestUrl;
        private static string pairingCode = "";
        private static string facade = "";
        private static string notification = "";

        static void Main(string[] args)
        {
            SelectEnvironment();
            int maxMenuItems = 10;
            int selector = 0;
            bool valid;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                DrawMenu(maxMenuItems);
                valid = int.TryParse(Console.ReadLine(), out selector);
                if (valid)
                {
                    switch (selector)
                    {
                        case 1:
                            GenerateKeyPair(ecKey);
                            break;
                        case 2:
                            GetPairingCode();
                            break;
                        case 3:
                            SelectEnvironment();
                            break;
                        default:
                            if (selector != maxMenuItems)
                            {
                                SetNotification();
                            }
                            break;
                    }
                }
                else
                {
                    SetNotification();
                }
            }
        }

        private static void SelectEnvironment()
        {
            int maxMenuItems = 3;
            int selector = 0;
            bool valid;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Select the working environment:");
                Console.WriteLine(" 1. Test");
                Console.WriteLine(" 2. Production");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                valid = int.TryParse(Console.ReadLine(), out selector);

                if (valid)
                {
                    switch (selector)
                    {
                        case 1:
                            env = Env.Test;
                            bitpay = new BitPay(envUrl: Env.TestUrl);
                            SetNotification(" Current environment: " + env);
                            return;
                        case 2:
                            env = Env.Prod;
                            bitpay = new BitPay();
                            SetNotification(" Current environment: " + env);
                            return;
                    }
                }
                else
                {
                    SetNotification();
                }
            }
        }

        private static void GenerateKeyPair(EcKey ecKey)
        {
            ecKey = KeyUtils.CreateEcKey();
            SetNotification(" New key pair generated successfully with public key: " + ecKey.PublicKeyHexBytes);
        }

        private static void GetPairingCode()
        {
            SetFacade();

            if (!bitpay.ClientIsAuthorized(facade))
            {
                var pairingCodeObj = bitpay.RequestClientAuthorization(facade);

                pairingCode = pairingCodeObj.Result;

                SetNotification(" New pairing code for " + facade + " facade: " + pairingCode);

            }
            else
            {
                SetNotification(" This client is already been authorized");
            }
        }

        private static void SetFacade()
        {
            int maxMenuItems = 3;
            int selector = 0;
            bool valid = false;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Select a facade:");
                Console.WriteLine(" 1. Pos");
                Console.WriteLine(" 2. Merchant");
                Console.WriteLine(" 3. payroll");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                valid = int.TryParse(Console.ReadLine(), out selector);

                if (valid)
                {
                    switch (selector)
                    {
                        case 1:
                            facade = Facade.PointOfSale;
                            return;
                        case 2:
                            facade = Facade.Merchant;
                            return;
                        case 3:
                            facade = Facade.Payroll;
                            return;
                    }
                }
                else
                {
                    SetNotification();
                }
            }
        }

        private static void SetNotification(string message = " Typing error")
        {
            notification = message;
        }

        private static void DrawTitle()
        {
            Console.WriteLine("\n   ___   _  __   ___             \n  / _ ) (_)/ /_ / _ \\ ___ _ __ __\n / _  |/ // __// ___// _ `// // /\n/____//_/ \\__//_/    \\_,_/ \\_, / \n                          /___/");
            Console.WriteLine("                          Setup");
            Console.WriteLine();
            if (string.IsNullOrEmpty(notification)) return;
            Console.WriteLine(notification);
            Console.WriteLine();
        }
        private static void DrawMenu(int maxitems)
        {
            Console.WriteLine(" Select one of the following options:");
            Console.WriteLine(" 1. Generate Key pair");
            Console.WriteLine(" 2. Get pairing code");
            Console.WriteLine(" 3. Select a different environment");

            Console.WriteLine(" 10. Close");
            Console.WriteLine();
            Console.Write(" Select an option: type 1, 2,...\r\n or {0} for exit... ", maxitems);
        }
        
//        private static string autocomplete()
//        {
//            var path = new DirectoryInfo("/");
//            var builder = new StringBuilder();
//            var input = Console.ReadKey(intercept:true);
//            var fileList = path.GetFiles();
//
//            while (input.Key != ConsoleKey.Enter)
//            {
//                var currentInput = builder.ToString();
//                
//                if (input.KeyChar == '/' && Directory.Exists(currentInput))
//                {
//                    path = new DirectoryInfo(currentInput);
//                    fileList = path.GetFiles();
//                }
//                
//                if (input.Key == ConsoleKey.Tab)
//                {
//                    
//                    var match = fileList.FirstOrDefault(item => item.Name != currentInput && item.Name.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
//                    if (string.IsNullOrEmpty(match.ToString()))
//                    {
//                        input = Console.ReadKey(intercept: true);
//                        continue;
//                    }
//
//                    ClearCurrentLine();
//                    builder.Clear();
//
//                    Console.Write(match);
//                    builder.Append(match);
//                }
//                else
//                {
//                    if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
//                    {
//                        builder.Remove(builder.Length - 1, 1);
//                        ClearCurrentLine();
//
//                        currentInput = currentInput.Remove(currentInput.Length - 1);
//                        Console.Write(currentInput);
//                    }
//                    else
//                    {
//                        var key = input.KeyChar;
//                        builder.Append(key);
//                        Console.Write(key);
//                    }
//                }
//
//                input = Console.ReadKey(intercept:true);
//            }
//            
//            return builder.ToString();
//        }
//
//
//        private static void ClearCurrentLine()
//        {
//            var currentLine = Console.CursorTop;
//            Console.SetCursorPosition(0, Console.CursorTop);
//            Console.Write(new string(' ', Console.WindowWidth));
//            Console.SetCursorPosition(0, currentLine);
//        }
    }
}