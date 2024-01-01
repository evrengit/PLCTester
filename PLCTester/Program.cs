using PLCTester.Prompter;
using Revo.SiemensDrivers.Sharp7;

namespace PLCTester
{
    static internal class Program
    {
        private static readonly PromptContext prompter = new PromptContext(new DefaultPromptStrategy());
        private static readonly PromptContext valuePrompter = new PromptContext(new ValuePromptStrategy());
        private static readonly PromptContext errorPrompter = new PromptContext(new ErrorPromptStrategy());

        static void Main(string[] args)
        {
            var arguments = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(arguments))
            {
                errorPrompter.Display("invalid parameter");
                return;
            }

            var argumentsSplitted = arguments.Split(":");

            int rack = 0;
            int slot = 0;

            if (argumentsSplitted.Length == 2)
            {
                rack = int.Parse(argumentsSplitted[1]);
                slot = int.Parse(argumentsSplitted[2]);
            }

            var ipAddress = argumentsSplitted[0];

            var client = new S7Client();

            int result = client.ConnectTo(ipAddress, rack, slot);

            if (result == 0)
            {
                prompter.Display("Connected to Siemens PLC");
            }
            else
            {
                errorPrompter.Display("Failed to connect to Siemens PLC");
            }

            while (true)
            {
                prompter.Display("Address: format <<dbnumber>>:<<dataype>>:<<offset>> Sample: 1-int-1.0  :");
                var addressRaw = Console.ReadLine();

                try
                {
                    var addressSplitted = addressRaw.Split("-");

                    var dbNumber = Convert.ToInt32(addressSplitted[0]);
                    var dataType = addressSplitted[1];

                    var offsetIntegral = Convert.ToInt32(addressSplitted[2].Split(".")[0]);
                    var offsetDecimal = Convert.ToInt32(addressSplitted[2].Split(".")[1]);

                    var dataSize = DataFieldProperties(dataType).DataSize;

                    prompter.Display("enter for reading same tag value. any key to quit!");

                    do
                    {
                        byte[] buffer = new byte[dataSize];

                        int readResult = client.DBRead(dbNumber, offsetIntegral, dataSize, buffer);

                        var value = GetDynamicValue(buffer, dataType, 0, offsetDecimal);

                        valuePrompter.Display($"readresult:{readResult} value: {value}");

                        var keyInfoRepeat = Console.ReadKey();
                        if (keyInfoRepeat.Key != ConsoleKey.Enter)
                        {
                            break;
                        }
                    } while (true);
                }
                catch (Exception ex)
                {
                    errorPrompter.Display($"Parametre okuma hatası. Exception:{ex}");
                }

                prompter.Display("\n\nRead another DB and tag: Help enter: continue. q: quit");

                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    continue;
                }

                if (keyInfo.Key == ConsoleKey.Q)
                {
                    prompter.Display("Disconnected from Siemens PLC");
                    client.Disconnect();
                    break;
                }

                client.Disconnect();
                Console.ReadLine();
            }
        }

        private static (string DataType, int DataSize) DataFieldProperties(string dataType)
        {
            dataType = dataType.ToLower();
            dataType = dataType == "bit" ? "bool" : dataType;
            dataType = dataType.Replace("ı", "i");

            int dataSize = 0;

            switch (dataType)
            {
                case "bool": dataSize = 1; break;
                case "sint": dataSize = 1; break;
                case "int": dataSize = 2; break;
                case "dint": dataSize = 4; break;
                case "udint": dataSize = 4; break;
                case "real": dataSize = 4; break;
                case "lreal": dataSize = 8; break;
                case "usint": dataSize = 1; break;
                case "byte": dataSize = 1; break;
                case "uint": dataSize = 2; break;
                case "word": dataSize = 2; break;
                case "dword": dataSize = 4; break;
                case "time": dataSize = 4; break;
                case "lword": dataSize = 8; break;
                case "ulint": dataSize = 8; break;
                case "lint": dataSize = 8; break;
                case "ltime": dataSize = 8; break;
                case "ldate": dataSize = 8; break;
                case "ltod": dataSize = 8; break;
                case "ldt": dataSize = 8; break;
            }

            if (dataType.Contains("string"))
            {
                int additionalBufferSizeForSiemensPLCString = 2;

                dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(dataType, @"\d+").ToString())
                    + additionalBufferSizeForSiemensPLCString;

                dataType = "string";
            }

            return (dataType, dataSize);
        }

        public static dynamic GetDynamicValue(byte[] buffer, string dataType, int offsetIntegral, int offsetDecimal)
        {
            dynamic tagValue = null;
            try
            {
                switch (dataType.ToLower())
                {
                    case "bool":
                        tagValue = S7.GetBitAt(buffer, offsetIntegral, offsetDecimal);
                        break;

                    case "sint":
                        tagValue = S7.GetSIntAt(buffer, offsetIntegral);
                        break;

                    case "int":
                        tagValue = S7.GetIntAt(buffer, offsetIntegral);
                        break;

                    case "dint":
                        tagValue = S7.GetIntAt(buffer, offsetIntegral);
                        break;

                    case "udint":
                        tagValue = S7.GetUIntAt(buffer, offsetIntegral);
                        break;

                    case "real":
                        tagValue = S7.GetRealAt(buffer, offsetIntegral);
                        break;

                    case "lreal":
                        tagValue = S7.GetLRealAt(buffer, offsetIntegral);
                        break;

                    case "usint":
                        tagValue = S7.GetUSIntAt(buffer, offsetIntegral);
                        break;

                    case "byte":
                        tagValue = S7.GetByteAt(buffer, offsetIntegral);
                        break;

                    case "uint":
                        tagValue = S7.GetByteAt(buffer, offsetIntegral);//s7de yok
                        break;

                    case "word":
                        tagValue = S7.GetWordAt(buffer, offsetIntegral);
                        break;

                    case "dword":
                        tagValue = S7.GetDWordAt(buffer, offsetIntegral);
                        break;

                    case "time":
                        tagValue = S7.GetLTimeAt(buffer, offsetIntegral);//s7de yok
                        break;

                    case "lword":
                        tagValue = S7.GetLWordAt(buffer, offsetIntegral);
                        break;

                    case "ulint":
                        tagValue = S7.GetULIntAt(buffer, offsetIntegral);
                        break;

                    case "lint":
                        tagValue = S7.GetLIntAt(buffer, offsetIntegral);
                        break;

                    case "ltime":
                        tagValue = S7.GetLTimeAt(buffer, offsetIntegral);
                        break;

                    case "ldate":
                        tagValue = S7.GetLDTAt(buffer, offsetIntegral);
                        break;

                    case "ltod":
                        tagValue = S7.GetLTODAt(buffer, offsetIntegral);
                        break;

                    case "ldt":
                        tagValue = S7.GetLDTAt(buffer, offsetIntegral);
                        break;

                    case string typeName when typeName.Contains("string"):
                        tagValue = S7.GetStringAt(buffer, offsetIntegral);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tagValue;
        }
    }
}