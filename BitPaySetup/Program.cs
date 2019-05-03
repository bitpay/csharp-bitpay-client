using System;
using System.IO;
using BitPayAPI;
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

        private static void Main()
        {
            if (string.IsNullOrEmpty(confFilePath)) {LoadConfFile();}
            Console.Clear();
            DrawTitle();
            if (string.IsNullOrEmpty(env)) {SelectEnvironment();}
            var maxMenuItems = 5;
            var selector = 0;
            bool valid;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                DrawMenu(maxMenuItems);
                valid = int.TryParse(Console.ReadLine(), out selector);
                if (valid)
                    switch (selector)
                    {
                        case 1:
                            GenerateKeyPair(ecKey);
                            break;
                        case 2:
                            SetClientDescription();
                            break;
                        case 3:
                            GetPairingCodeAndToken();
                            break;
                        case 4:
                            SelectEnvironment();
                            break;
                        default:
                            if (selector != maxMenuItems) SetNotification();
                            break;
                    }
                else
                    SetNotification();
            }
        }

        private static void SetClientDescription()
        {
            Console.Clear();
            DrawTitle();

            string clientDefinition = null;
            if (env == Env.Test) clientDefinition = appConfig.BitPayConfiguration.EnvConfig.Test.ClientDescription;
            if (env == Env.Prod) clientDefinition = appConfig.BitPayConfiguration.EnvConfig.Prod.ClientDescription;

            if (!string.IsNullOrEmpty(clientDefinition))
            {
                Console.WriteLine(" The current set client description is: " + clientDefinition);
                Console.WriteLine(" Would you like to change it? [yes|no] (default: no)");
                Console.WriteLine();
                Console.Write(" > ");
                var answer = Console.ReadLine();
                while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                    answer = Console.ReadLine();

                if (answer.ToLower() == "no" || answer.ToLower() == "")
                {
                    SetNotification(" Selected client description: " + clientDefinition);

                    return;
                }
            }

            Console.WriteLine(" Enter a new client description:");
            Console.WriteLine();
            Console.Write(" > ");
            clientDefinition = Console.ReadLine();

            if (env == Env.Test) appConfig.BitPayConfiguration.EnvConfig.Test.ClientDescription = clientDefinition;
            if (env == Env.Prod) appConfig.BitPayConfiguration.EnvConfig.Prod.ClientDescription = clientDefinition;

            SetNotification(" Selected client description: " + clientDefinition);

            GenerateConfFile(confFilePath);
        }

        private static void SelectEnvironment()
        {
            var maxMenuItems = 3;
            var selector = 0;
            bool valid;
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
                        SetNotification(" Selected environment: " + env + "\n IMPORTANT: If this is the first time you set up your client, follow the below steps in order.");

                        return;
                    }
                }

                Console.WriteLine(" Select the working environment:");
                Console.WriteLine(" 1. Test");
                Console.WriteLine(" 2. Production");
                Console.WriteLine(" 3. Cancel");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                valid = int.TryParse(Console.ReadLine(), out selector);

                if (valid)
                {
                    switch (selector)
                    {
                        case 1:
                            env = Env.Test;
                            SetNotification(" Selected environment: " + env);

                            break;
                        case 2:
                            env = Env.Prod;
                            GenerateConfFile(confFilePath);
                            SetNotification(" Selected environment: " + env);

                            break;
                        case 3:
                            Main();
                            return;
                    }
                    SetNotification(notification + "\n IMPORTANT: If this is the first time you set up your client, follow the below steps in order.");

                    appConfig.BitPayConfiguration.Environment = env;
                    GenerateConfFile(confFilePath);

                    return;
                }

                SetNotification();
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
                        SetNotification(" Selected private key file: " + ecKeyFilePath);

                        return;
                    }

                    SetNotification(" The private key file does not longer exists in: \n \"" + ecKeyFilePath +
                                    "\"\n Please, proceed with the following instructions.");
                    Console.Clear();
                    DrawTitle();
                }
            }

            Console.WriteLine(" Enter the full path for the private key files where this will loaded from or generated:");
            Console.WriteLine(" If click Enter, a file named \"bitpay_private_" + env.ToLower() +
                              " will be generated in the root of this application and");
            Console.WriteLine(" any file with the same name in this directory will be overwritten.");
            Console.WriteLine();
            Console.Write(" > ");
            var newEcKeyPath = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(newEcKeyPath))
            {
                ecKeyFilePath = @"bitpay_private_" + env.ToLower() + ".key";
                if (KeyUtils.PrivateKeyExists(ecKeyFilePath))
                {
                    SetNotification(" The file name entered already exists: \n \"" + ecKeyFilePath +
                                    "\"\n Make sure you want to modify it and then delete it before trying again");

                    return;
                }

                ecKey = KeyUtils.CreateEcKey();
                KeyUtils.SaveEcKey(ecKey);
                SetNotification(" New private key generated successfully with public key:\n " + ecKey.PublicKeyHexBytes +
                                "\n in: \"" + ecKeyFilePath + "\"");
            }
            else
            {
                if (!File.Exists(newEcKeyPath))
                {
                    SetNotification(" The file entered not found in: \n \"" + newEcKeyPath + "\"");
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

                    if (KeyUtils.PrivateKeyExists(newEcKeyPath))
                    {
                        SetNotification(" The file name entered already exists: \n \"" + newEcKeyPath +
                                        "\"\n Be sure you want to modify it and then delete it manually before trying again");

                        return;
                    }
                    ecKeyFilePath = newEcKeyPath;
                }

                try
                {
                    ecKey = KeyUtils.CreateEcKey();
                    KeyUtils.SaveEcKey(ecKey);
                }
                catch (Exception e)
                {
                    SetNotification(
                        " An error occurred, please, check if the you have the right\n permissions to write in the specified directory.\n Error Details: " +
                        e.Message);

                    return;
                }

                SetNotification(" New key pair generated successfully with public key: " + ecKey.PublicKeyHexBytes +
                                " in: \n \"" + newEcKeyPath + "\"");
            }

            if (env == Env.Test) appConfig.BitPayConfiguration.EnvConfig.Test.PrivateKeyPath = ecKeyFilePath;
            if (env == Env.Prod) appConfig.BitPayConfiguration.EnvConfig.Prod.PrivateKeyPath = ecKeyFilePath;

            GenerateConfFile(confFilePath);
        }

        private static void GetPairingCodeAndToken()
        {
            string newToken;
            
            SetFacade();
            var apiTokens = new ApiTokens();
            if (env == Env.Test) apiTokens = appConfig.BitPayConfiguration.EnvConfig.Test.ApiTokens;
            if (env == Env.Prod) apiTokens = appConfig.BitPayConfiguration.EnvConfig.Prod.ApiTokens;

            if (TokenCurrentlyExists(facade, apiTokens))
            {
                SetNotification(" A token for this Facade (" + facade + ") on this Environment (" + env +
                                ") already exists.");
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Would you want to replace it for a new one? [yes|no] (default: no)");
                Console.WriteLine();
                Console.Write(" > ");
                var answer = Console.ReadLine();
                while (answer.ToLower() != "yes" && answer.ToLower() != "no" && answer.ToLower() != "")
                    answer = Console.ReadLine();

                if (answer.ToLower() == "no") return;
            }

            try
            {
                var bitpay = new BitPay(confFilePath);
    
                var pairingCodeObj = bitpay.RequestClientAuthorization(facade);
    
                pairingCode = pairingCodeObj.Result;
    
                newToken = bitpay.GetTokenByFacade(facade);
            }
            catch (Exception e)
            {
                SetNotification(
                    " An error occurred while generating a new token pair.\n Make sure you have created a/o selected a private key.\n Error Details: " +
                    e.Message);

                return;
            }
            
            switch (facade)
            {
                case Facade.PointOfSale:
                    apiTokens.pos = newToken;
                    break;
                case Facade.Merchant:
                    apiTokens.merchant = newToken;
                    break;
                case Facade.Payroll:
                    apiTokens.payroll = newToken;
                    break;
            }

            var envUrl = "";
            if (env == Env.Test)
            {
                appConfig.BitPayConfiguration.EnvConfig.Test.ApiTokens = apiTokens;
                envUrl = appConfig.BitPayConfiguration.EnvConfig.Test.ApiUrl;
            }

            if (env == Env.Prod)
            {
                appConfig.BitPayConfiguration.EnvConfig.Prod.ApiTokens = apiTokens;
                envUrl = appConfig.BitPayConfiguration.EnvConfig.Prod.ApiUrl;
            }

            GenerateConfFile(confFilePath);

            SetNotification(" New pairing code for " + facade + " facade: " + pairingCode + "\n Please, copy the above pairing code and approve on your BitPay Account at the following link:\n \"" +
                            envUrl + "dashboard/merchant/api-tokens\".\n Once this Pairing Code is approved, your .Net Client will be ready to work with the API.\n A new token for this pairing code has been added to the configuration file.");
        }

        private static void SetFacade()
        {
            var maxMenuItems = 3;
            var selector = 0;
            var valid = false;
            while (selector != maxMenuItems)
            {
                Console.Clear();
                DrawTitle();
                Console.WriteLine(" Select a facade for which you want to generate a new token pair:");
                Console.WriteLine(" 1. POS (Point-Of-Sale)");
                Console.WriteLine(" 2. Merchant");
                Console.WriteLine(" 3. Payroll");
                Console.WriteLine(" 4. Cancel");
                Console.WriteLine();
                Console.Write(" Select an option: ");

                valid = int.TryParse(Console.ReadLine(), out selector);

                if (valid)
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
                        case 4:
                            Main();
                            return;
                    }
                else
                    SetNotification();
            }
        }

        private static bool TokenCurrentlyExists(string facade, ApiTokens apiTokens)
        {
            var tokenExists = true;
            switch (facade)
            {
                case Facade.PointOfSale:
                    if (string.IsNullOrEmpty(apiTokens.pos)) tokenExists = false;
                    break;
                case Facade.Merchant:
                    if (string.IsNullOrEmpty(apiTokens.merchant)) tokenExists = false;
                    break;
                case Facade.Payroll:
                    if (string.IsNullOrEmpty(apiTokens.payroll)) tokenExists = false;
                    break;
            }

            return tokenExists;
        }

        private static void SetNotification(string message = " Typing error")
        {
            notification = message;
        }

        private static void DrawTitle()
        {
            Console.WriteLine(
                "\n   ___   _  __   ___             \n  / _ ) (_)/ /_ / _ \\ ___ _ __ __\n / _  |/ // __// ___// _ `// // /\n/____//_/ \\__//_/    \\_,_/ \\_, / \n                          /___/");
            Console.WriteLine("                          Setup");
            Console.WriteLine();
            if (string.IsNullOrEmpty(notification)) return;
            Console.WriteLine(notification);
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
                    SetNotification(" The new configuration file is been generated in: \n \"" + confFilePath + "\"");
                }
                else
                {
                    if (!File.Exists(newConfFilePath))
                    {
                        SetNotification(" The file entered not found in: \n \"" + newConfFilePath + "\"");
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
    
                        if (answer.ToLower() == "yes") LoadConfFile();
    
                        GenerateConfFile(newConfFilePath);
                        SetNotification(" The new configuration file is been generated in: \n \"" + confFilePath + "\"");
    
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
                    e.Message);
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
                    e.Message);
                LoadConfFile();
            }
        }

        private static void GenerateConfFile(string newConfFilePath = @"BitPay.config.json")
        {
            if (!confInitiated)
            {
                appConfig = new BitPayConfigurationModel();
                confInitiated = true;
                appConfig.BitPayConfiguration.Environment = env;
            }

            // serialize JSON directly to a new config file
            using (var file = File.CreateText(newConfFilePath))
            {
                var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                serializer.Serialize(file, appConfig);
            }
            
            confFilePath = newConfFilePath;
        }
    }
}