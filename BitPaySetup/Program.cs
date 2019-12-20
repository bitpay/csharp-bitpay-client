using System;
using System.IO;
using BitPaySDK;
using BitPaySetup.Models;
using Newtonsoft.Json;

namespace BitPaySetup
{
    internal class Program
    {
        private static BitPayConfigurationModel appConfig;
        private static bool confInitiated;
        private static string confFilePath;
        private static string ecKeyFilePath;
        private static readonly EcKey ecKey = null;
        private static string env;
        private static string pairingCode = "";
        private static string facade = "";
        private static string notification = "";
        private static string envUrl = "";
        private static ConsoleColor notificationColor;

        private static void Main()
        {
            if (string.IsNullOrEmpty(confFilePath))
            {
                LoadConfFile();
            }

            DrawTitle();
            if (string.IsNullOrEmpty(env))
            {
                SelectEnvironment();
            }

            GenerateKeyPair(ecKey);
            GetPairingCodeAndToken();

            SetNotification(
                " Congratulations, you are all setup!\n You can now move the configuration and private key files\n to a secure location away from being shared or exposed.\n " +
                "In case this files could be compromised, we strongly recommend to run\n this utility to replace them by new ones.",
                1);

            Console.Clear();
            DrawTitle();
            Console.Write(" Press any key to close this application.");
            Console.ReadKey();
        }

        private static void SelectEnvironment()
        {
            var maxMenuItems = 3;
            var selector = 0;
            bool valid = false;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();

                env = appConfig.BitPayConfiguration.Environment;
                if (!string.IsNullOrEmpty(env))
                {
                    Console.WriteLine(" The current set environment is: " + env);
                    Console.WriteLine(" Would you like to change it? [yes|no] (default: no)");
                    Console.WriteLine();
                    Console.Write(" > ");
                    var answer = Console.ReadLine();
                    while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                        answer = Console.ReadLine();

                    if (answer.ToLower() == "no" || answer.ToLower() == "")
                    {
                        SetNotification(" Selected environment: " + env, 1);

                        return;
                    }
                }

                Console.WriteLine(" Select the working environment (default: Production)");
                Console.WriteLine(" 1. Test");
                Console.WriteLine(" 2. Production");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    valid = true;
                }
                else if (char.IsDigit(key.KeyChar))
                {
                    valid = int.Parse(key.KeyChar.ToString()) == 1 || int.Parse(key.KeyChar.ToString()) == 2;
                }

                if (valid)
                {
                    switch (key.KeyChar)
                    {
                        case '1':
                            Console.WriteLine(key.KeyChar.ToString());
                            env = Env.Test;
                            SetNotification(" Selected environment: " + env, 1);

                            break;
                        default:
                            Console.WriteLine(key.KeyChar.ToString());
                            env = Env.Prod;
                            GenerateConfFile(confFilePath);
                            SetNotification(" Selected environment: " + env, 1);

                            break;
                    }

                    appConfig.BitPayConfiguration.Environment = env;
                    GenerateConfFile(confFilePath);

                    return;
                }

                SetNotification(notificationColorCode: 2);
            }
        }

        private static void GenerateKeyPair(EcKey ecKey)
        {
            Console.Clear();
            DrawTitle();

            if (env == Env.Test) ecKeyFilePath = appConfig.BitPayConfiguration.EnvConfig.Test.PrivateKeyPath;
            if (env == Env.Prod) ecKeyFilePath = appConfig.BitPayConfiguration.EnvConfig.Prod.PrivateKeyPath;

            if (!string.IsNullOrEmpty(ecKeyFilePath))
            {
                Console.WriteLine(" The current private key file defined is: " + ecKeyFilePath);
                Console.WriteLine(" Would you like to change it? [yes|no] (default: no)");
                Console.WriteLine();
                Console.Write(" > ");
                var answer = Console.ReadLine();
                while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                    answer = Console.ReadLine();

                if (answer.ToLower() == "no" || answer.ToLower() == "")
                {
                    if (File.Exists(ecKeyFilePath))
                    {
                        SetNotification(" Selected private key file: " + ecKeyFilePath, 1);

                        return;
                    }

                    SetNotification(" The private key file does not longer exists in: \n \"" + ecKeyFilePath +
                                    "\"\n Please, proceed with the following instructions.", 2);
                    Console.Clear();
                    DrawTitle();
                }
            }

            Console.WriteLine(" Enter the full path for the private key file where this will loaded from:");
            Console.WriteLine(" If click Enter, a file named \"bitpay_private_" + env.ToLower() +
                              " will be generated in the root of this application and");
            Console.WriteLine(" any file with the same name in this directory will be overwritten.");
            Console.WriteLine();
            Console.Write(" > ");
            var newEcKeyPath = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(newEcKeyPath))
            {
                string ecKeyFileName = @"bitpay_private_" + env.ToLower() + ".key";

                try
                {
                    if (!Directory.Exists(ecKeyFilePath))
                    {
                        DirectoryInfo dir = Directory.CreateDirectory("output");
                        ecKeyFilePath = Path.Combine(dir.FullName, ecKeyFileName);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (KeyUtils.PrivateKeyExists(ecKeyFilePath))
                {
                    SetNotification(" A file with the same name already exists: \n \"" + ecKeyFilePath +
                                    "\"\n Make sure you want to modify it, then delete it manually before trying again" +
                                    "\"\n For security reasons we won't delete a private key.", 2);

                    GenerateKeyPair(ecKey);
                    return;
                }

                ecKey = KeyUtils.CreateEcKey();
                KeyUtils.PrivateKeyExists(ecKeyFilePath);
                KeyUtils.SaveEcKey(ecKey);

                if (KeyUtils.PrivateKeyExists(ecKeyFilePath))
                {
                    SetNotification(" New private key generated successfully with public key:\n " +
                                    ecKey.PublicKeyHexBytes +
                                    "\n in: \"" + ecKeyFilePath + "\"", 1);
                }
                else
                {
                    SetNotification(" Something went wrong when creating the file: \n \"" + newEcKeyPath +
                                    "\"\n Make sure the directory exists and you have the right permissions, then trying again.",
                        2);

                    GenerateKeyPair(ecKey);
                    return;
                }
            }
            else
            {
                if (!File.Exists(newEcKeyPath))
                {
                    SetNotification(" The file entered not found in: \n \"" + newEcKeyPath + "\"", 2);
                    Console.Clear();
                    DrawTitle();
                    Console.WriteLine(" Would you like to provide a different file path? [yes|no] (default: no)");
                    Console.WriteLine(
                        " If 'no', a new file will be generated in the entered location with the given name.");
                    Console.WriteLine();
                    Console.Write(" > ");
                    var answer = Console.ReadLine();
                    while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                        answer = Console.ReadLine();

                    if (answer.ToLower() == "yes") GenerateKeyPair(ecKey);

                    ecKeyFilePath = Path.GetFullPath(newEcKeyPath);
                }
                else
                {
                    if (KeyUtils.PrivateKeyExists(newEcKeyPath))
                    {
                        SetNotification(" A file with the same name already exists: \n \"" + newEcKeyPath +
                                        "\"\n Make sure you want to modify it, then delete it manually before trying again" +
                                        "\"\n For security reasons we won't delete a private key.", 2);

                        GenerateKeyPair(ecKey);
                        return;
                    }
                }

                try
                {
                    ecKey = KeyUtils.CreateEcKey();
                    KeyUtils.PrivateKeyExists(ecKeyFilePath);
                    KeyUtils.SaveEcKey(ecKey);

                    if (KeyUtils.PrivateKeyExists(ecKeyFilePath))
                    {
                        SetNotification(" New private key generated successfully with public key:\n " +
                                        ecKey.PublicKeyHexBytes +
                                        "\n in: \"" + ecKeyFilePath + "\"", 1);
                    }
                    else
                    {
                        throw new Exception(" Could not store the file: \n \"" + newEcKeyPath + "\"");
                    }
                }
                catch (Exception e)
                {
                    SetNotification(
                        " An error occurred, please, check if the you have the right\n permissions to write in the specified directory.\n Error Details: " +
                        e.Message, 2);

                    GenerateKeyPair(ecKey);
                }

                SetNotification(" New Private key generated successfully with public key: " + ecKey.PublicKeyHexBytes +
                                " in: \n \"" + newEcKeyPath + "\"", 1);
            }

            if (env == Env.Test) appConfig.BitPayConfiguration.EnvConfig.Test.PrivateKeyPath = ecKeyFilePath;
            if (env == Env.Prod) appConfig.BitPayConfiguration.EnvConfig.Prod.PrivateKeyPath = ecKeyFilePath;

            GenerateConfFile(confFilePath);
        }

        private static void GetPairingCodeAndToken()
        {
            string newToken = "";
            
            SetFacade();
            if (String.IsNullOrEmpty(facade))
            {
                return;
            }

            Console.Clear();
            DrawTitle();
            Console.WriteLine(" Would you like to request a token for the " + facade.ToUpperInvariant() +
                              " facade? [yes|no] (default: yes)");
            Console.WriteLine();
            Console.Write(" > ");
            var answer = Console.ReadLine();
            while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                answer = Console.ReadLine();

            if (answer.ToLower() == "no")
            {
                GetPairingCodeAndToken();
                return;
            }

            if (facade == Facade.Payroll)
            {
                SetNotification(
                    " In order to get access to the Payroll facade, you need to contact Support at support@bitpay.com",
                    3);
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Did you contact and receive confirmation yet? [yes|no] (default: yes)");
                Console.WriteLine(
                    " If 'no', a new file will be generated in the entered location with the given name.");
                Console.WriteLine();
                Console.Write(" > ");
                answer = Console.ReadLine();
                while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                    answer = Console.ReadLine();

                if (answer.ToLower() == "no")
                {
                    GetPairingCodeAndToken();
                    return;
                }
            }

            var apiTokens = new ApiTokens();
            if (env == Env.Test) apiTokens = appConfig.BitPayConfiguration.EnvConfig.Test.ApiTokens;
            if (env == Env.Prod) apiTokens = appConfig.BitPayConfiguration.EnvConfig.Prod.ApiTokens;

            if (TokenCurrentlyExists(facade, apiTokens))
            {
                SetNotification(" A token for this Facade (" + facade + ") on this Environment (" + env +
                                ") already exists.", 3);
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Would you like to replace it for a new one? [yes|no] (default: no)");
                Console.WriteLine();
                Console.Write(" > ");
                answer = Console.ReadLine();
                while (answer.ToLower() != "yes" && answer.ToLower() != "no")
                    answer = Console.ReadLine();

                if (answer.ToLower() == "no" || answer.ToLower() == "") return;
            }

            var retry = true;
            while (retry)
            {
                try
                {
                    var bitpay = new BitPay(confFilePath);

                    pairingCode = bitpay.RequestClientAuthorization(facade).Result;

                    newToken = bitpay.GetTokenByFacade(facade);
                }
                catch (Exception e)
                {
                    SetNotification(
                        " An error occurred while generating a new token pair.\n Make sure you have created a/o selected a private key.\n Error Details: " +
                        e.Message, 2);

                    GetPairingCodeAndToken();
                    return;
                }

                switch (facade)
                {
                    case Facade.Merchant:
                        apiTokens.merchant = newToken;
                        break;
                    case Facade.Payroll:
                        apiTokens.payroll = newToken;
                        break;
                }

                if (env == Env.Test)
                {
                    appConfig.BitPayConfiguration.EnvConfig.Test.ApiTokens = apiTokens;
                    envUrl = "https://test.bitpay.com/";
                }

                if (env == Env.Prod)
                {
                    appConfig.BitPayConfiguration.EnvConfig.Prod.ApiTokens = apiTokens;
                    envUrl = "https://bitpay.com/";
                }

                GenerateConfFile(confFilePath);

                SetNotification(" New pairing code for " + facade + " facade: " + pairingCode +
                                "\n Please, copy the above pairing code and approve on your BitPay Account at the following link:\n \"" +
                                envUrl +
                                "dashboard/merchant/api-tokens\".\n Once this Pairing Code is approved, press Enter to run some tests.\n",
                    1);

                Console.Clear();
                DrawTitle();
                Console.Write(" If you approved the pairing code on your Dashboard, press Enter to continue: ");
                var input = Console.ReadKey();
                while (input.Key != ConsoleKey.Enter)
                    input = Console.ReadKey();

                while (retry = !TestTokenSuccess(facade))
                {
                    SetNotification(" Something went wrong\n Please, make sure you approved the pairing code: " +
                                    pairingCode + " for " + facade +
                                    " facade\n Copy the above pairing code and approve on your BitPay Account at the following link:\n \"" +
                                    envUrl +
                                    "dashboard/merchant/api-tokens\".\n Once this Pairing Code is approved, press Enter to run some tests again.\n\n" +
                                    "* NOTE: If you approved the pairing code and a token has been stored in the configuration file, you can\n" +
                                    "then ignore this message and close this application.\n" +
                                    "If your integration does not work with the generated files, please contact support at support@bitpay.com\n" +
                                    "or report it as an issue on the GitHub repository for this tool.\n",
                        2);

                    Console.Clear();
                    DrawTitle();
                    Console.Write(
                        " If you approved the pairing code on your Dashboard, press Enter to try again, N to generate a new one or X to cancel: ");
                    input = Console.ReadKey();
                    while (input.Key != ConsoleKey.Enter && input.Key != ConsoleKey.N && input.Key != ConsoleKey.X)
                        input = Console.ReadKey();

                    if (input.Key == ConsoleKey.X)
                    {
                        GetPairingCodeAndToken();

                        return;
                    }

                    if (input.Key == ConsoleKey.N)
                    {
                        retry = true;
                        break;
                    }
                }
            }

            SetNotification(
                " Token tested successfully!\n A new token for the " + facade +
                " facade has been added to the configuration file.", 1);

            Console.Clear();
            DrawTitle();
            Console.WriteLine(" Would you like to generate a token for a different facade? [yes|no] (default: no)");
            Console.WriteLine();
            Console.Write(" > ");
            var redo = Console.ReadLine();
            while (redo.ToLower() != "yes" && redo.ToLower() != "no" && redo.ToLower() != "")
                redo = Console.ReadLine();

            if (redo.ToLower() == "no" || redo.ToLower() == "") return;

            GetPairingCodeAndToken();
        }

        private static void SetFacade()
        {
            var maxMenuItems = 2;
            var selector = 0;
            var valid = false;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                Console.WriteLine(
                    " Select a facade for which you want to generate a new token pair (Press Enter to skip)");
                Console.WriteLine(" 1. Merchant");
                Console.WriteLine(" 2. Payroll");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    valid = true;
                }
                else if (char.IsDigit(key.KeyChar))
                {
                    valid = int.Parse(key.KeyChar.ToString()) > 0 && int.Parse(key.KeyChar.ToString()) < 4;
                }

                if (valid)
                    switch (key.KeyChar)
                    {
                        case '1':
                            facade = Facade.Merchant;
                            return;
                        case '2':
                            facade = Facade.Payroll;
                            return;
                        default:
                            facade = null;
                            return;
                    }

                SetNotification(notificationColorCode: 2);
            }
        }

        private static bool TokenCurrentlyExists(string facade, ApiTokens apiTokens)
        {
            var tokenExists = true;
            switch (facade)
            {
                case Facade.Merchant:
                    if (string.IsNullOrEmpty(apiTokens.merchant)) tokenExists = false;
                    break;
                case Facade.Payroll:
                    if (string.IsNullOrEmpty(apiTokens.payroll)) tokenExists = false;
                    break;
            }

            return tokenExists;
        }

        private static bool TestTokenSuccess(string facade)
        {
            try
            {
                var bitpay = new BitPay(confFilePath);
                if (facade == Facade.Merchant)
                {
                    var response = bitpay.GetInvoice("1", facade).Result;
                }
                else if (facade == Facade.Payroll)
                {
                    var response = bitpay.GetPayoutBatch("1").Result;
                }

                return true;
            }
            catch (Exception e)
            {
                if (e.InnerException.Message.ToLower().Contains("object not found"))
                {
                    return true;
                }

                return false;
            }
        }

        private static void SetNotification(string message = " Typing error", int notificationColorCode = 0)
        {
            notification = message;
            switch (notificationColorCode)
            {
                case 1:
                    notificationColor = ConsoleColor.Green;
                    break;
                case 2:
                    notificationColor = ConsoleColor.Red;
                    break;
                case 3:
                    notificationColor = ConsoleColor.Blue;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        private static void DrawTitle()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(
                "\n   ___   _  __   ___             \n  / _ ) (_)/ /_ / _ \\ ___ _ __ __\n / _  |/ // __// ___// _ `// // /\n/____//_/ \\__//_/    \\_,_/ \\_, / \n                          /___/  ");
            Console.WriteLine("                          Setup  ");
            Console.ResetColor();
            Console.WriteLine();
            if (string.IsNullOrEmpty(notification)) return;
            Console.ForegroundColor = notificationColor;
            Console.WriteLine(notification);
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DrawMenu(int maxitems)
        {
            Console.WriteLine(" Select one of the following options:");
            Console.WriteLine(" 1. Generate Private Key");
            Console.WriteLine(" 2. Set client description for environment");
            Console.WriteLine(" 3. Get pairing code and token");
            Console.WriteLine(" 4. Select a different environment");

            Console.WriteLine(" 5. Close");
            Console.WriteLine();
            Console.Write(" Select an option: type 1, 2,...\r\n or {0} for exit... ", maxitems);
        }

        private static void LoadConfFile()
        {
            Console.Clear();
            DrawTitle();
            Console.WriteLine(
                " Enter the full path for the configuration file where this will loaded from or generated:");
            Console.WriteLine(" If click Enter, a file named \"BitPay.config.json\" will be generated and");
            Console.WriteLine(" any file with the same name in this directory will be overwritten.");
            Console.WriteLine();
            Console.Write(" > ");
            var newConfFilePath = Console.ReadLine().Trim();

            try
            {
                if (string.IsNullOrEmpty(newConfFilePath))
                {
                    GenerateConfFile();
                    SetNotification(" The new configuration file has been generated in: \n \"" + confFilePath + "\"",
                        1);
                }
                else
                {
                    if (!File.Exists(newConfFilePath))
                    {
                        SetNotification(" The file entered not found in: \n \"" + newConfFilePath + "\"", 2);
                        Console.Clear();
                        DrawTitle();
                        Console.WriteLine(" Would you like to provide a different file path? [yes|no] (default: no)");
                        Console.WriteLine(
                            " If 'no', a new file will be generated in the entered location with the given name.");
                        Console.WriteLine();
                        Console.Write(" > ");
                        var answer = Console.ReadLine();
                        while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                            answer = Console.ReadLine();

                        if (answer.ToLower() == "yes")
                        {
                            LoadConfFile();
                            return;
                        }

                        GenerateConfFile(newConfFilePath);
                        SetNotification(
                            " The new configuration file has been generated in: \n \"" + confFilePath + "\"", 1);

                        return;
                    }

                    confFilePath = newConfFilePath;
                    GetConfFromFile(newConfFilePath);
                }
            }
            catch (Exception e)
            {
                SetNotification(
                    " An error occurred while loading the configuration.\n Error Details: " +
                    e.Message, 2);
                LoadConfFile();
            }
        }

        private static void GetConfFromFile(string newConfFilePath)
        {
            try
            {
                appConfig =
                    JsonConvert.DeserializeObject<BitPayConfigurationModel>(File.ReadAllText(newConfFilePath));
                confInitiated = true;
            }
            catch (Exception e)
            {
                SetNotification(
                    " An error occurred, please, check if the file format and structure are correct.\n Error Details: " +
                    e.Message, 2);
                LoadConfFile();
            }
        }

        private static void GenerateConfFile(string newConfFilePath = @"BitPay.config.json")
        {
            try
            {
                if (!confInitiated)
                {
                    appConfig = new BitPayConfigurationModel();
                    confInitiated = true;
                    appConfig.BitPayConfiguration.Environment = env;
                }

                try
                {
                    if (!Directory.Exists(newConfFilePath))
                    {
                        DirectoryInfo dir = Directory.CreateDirectory("output");
                        newConfFilePath = Path.Combine(dir.FullName, newConfFilePath);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // serialize JSON directly to a new config file
                using (var file = File.CreateText(newConfFilePath))
                {
                    var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                    serializer.Serialize(file, appConfig);
                }

                confFilePath = newConfFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}