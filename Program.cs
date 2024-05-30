using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Threading;

namespace sugarloaf_cmd_to_unit {
    class Program {
        static List<int> tx = new List<int>();
        static bool flag_data = false;
        static SerialPort mySerialPort;
        static bool flag_discom = false;
        private static System.Threading.Timer close_program;
        private static bool debug;
        /// <summary>
        /// Define head
        /// </summary>
        private static string head;
        /// <summary>
        /// Define command check from main
        /// </summary>
        private static string checking;
        static void Main(string[] args) {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            //throw new Exception("2");
            //throw new FileNotFoundException();

            //int asdas = Convert.ToInt32("sdf");
            //List<int> listTest = new List<int>();
            //listTest.Add(1);
            //if (listTest[1] == 0)
            //{

            //}

            head = "1";
            while (true) {
                try { head = File.ReadAllText("../../config/head.txt"); break; } catch { Thread.Sleep(50); }
            }
            File.Delete("../../config/head.txt");
            File.WriteAllText("call_exe_tric.txt", "");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_checking.txt", "equal");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_tx.txt", "0x21");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx.txt", "0x00");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_comport.txt", "COM6");
            //File.WriteAllText("../../config/test_head_" + head + "_timeout.txt", "3000");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_min.txt", "1");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_max.txt", "33");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_digit.txt", "11");
            //File.WriteAllText("../../config/test_head_" + head + "_debug.txt", "True");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_retest.txt", "3");
            //File.WriteAllText("../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_jumper.txt", "0x90,0x01");

            string datas = "0x21";
            string rxx = "0x00";
            string port_name = "COM5";
            checking = "equal";
            int timeout = 5000;
            debug = false;
            string rxx_min = "5";
            string rxx_max = "90";
            int digit = 1;
            int retest = 0;
            string rx_jumper = "0x90,0x01";
            string s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_tx.txt";
            try { datas = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx.txt";
            try { rxx = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_min.txt";
            try { rxx_min = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_max.txt";
            try { rxx_max = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_comport.txt";
            try { port_name = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_checking.txt";
            try { checking = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_jumper.txt";
            try { rx_jumper = File.ReadAllText(s); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/test_head_" + head + "_timeout.txt";
            try { timeout = Convert.ToInt32(File.ReadAllText(s)); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/test_head_" + head + "_debug.txt";
            try { debug = Convert.ToBoolean(File.ReadAllText(s)); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_data_rx_digit.txt";
            try { digit = Convert.ToInt32(File.ReadAllText(s)); } catch (Exception) { Console.WriteLine("No " + s); }
            s = "../../config/sugarloaf_cmd_to_unit_" + head + "_retest.txt";
            try { retest = Convert.ToInt32(File.ReadAllText(s)); } catch (Exception) { Console.WriteLine("No " + s); }
            close_program = new Timer(TimerCallback, null, 0, timeout + 80000);
            mySerialPort = new SerialPort(port_name);
            mySerialPort.BaudRate = 9600;
            mySerialPort.PortName = port_name;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            aaaa:
            Stopwatch t = new Stopwatch();
            t.Restart();
            while (t.ElapsedMilliseconds < 3000) {
                try {
                    mySerialPort.Open();
                    t.Stop();
                    break;
                } catch (Exception) {
                    Thread.Sleep(250);
                }
                try { mySerialPort.Close(); } catch (Exception) { }
            }
            if (t.IsRunning) {
                if (!flag_discom) {
                    try { mySerialPort.Close(); } catch { }
                    discom("disable", port_name);
                    discom("enable", port_name);
                    flag_discom = true;
                    goto aaaa;
                }
                File.WriteAllText("test_head_" + head + "_result.txt", "can not open port\r\nFAIL");
                return;
            }
            Console.WriteLine("head " + head);
            Console.WriteLine(mySerialPort.PortName);
            Console.WriteLine(mySerialPort.BaudRate);
            byte dataa = Convert.ToByte(datas.Substring(2, 2), 16);
            byte[] data = { 0xAA };
            data[0] = dataa;
            Stopwatch time = new Stopwatch();
            flag_data = false;
            time.Restart();
            while (time.ElapsedMilliseconds < 500) {
                if (flag_data == true) { flag_data = false; time.Restart(); }
            }
            for (int re = 0; re < retest; re++) {
                Console.WriteLine(re + ". send: 0x" + dataa.ToString("X2"));
                mySerialPort.DiscardInBuffer();
                mySerialPort.DiscardOutBuffer();
                tx.Clear();
                try { mySerialPort.Write(data, 0, 1); } catch {
                    mySerialPort.Dispose();
                    goto aaaa;
                }
                time.Restart();
                while (time.ElapsedMilliseconds < timeout) {
                    if (flag_data != true) { Thread.Sleep(100); continue; }
                    flag_data = false;
                    time.Stop();
                    break;
                }
                if (time.IsRunning) {
                    if (re < retest - 1) continue;
                    if (!flag_discom) {
                        mySerialPort.Close();
                        discom("disable", port_name);
                        discom("enable", port_name);
                        flag_discom = true;
                        goto aaaa;
                    }
                    Console.WriteLine("timeout!!!");
                    File.WriteAllText("test_head_" + head + "_result.txt", "timeout\r\nFAIL");
                    return;
                }
                byte rx;
                double rx_min = 0;
                double rx_max = 0;
                try { rx_min = Convert.ToDouble(rxx_min); } catch { }
                try { rx_max = Convert.ToDouble(rxx_max); } catch { }
                if (debug == true) {
                    Console.ReadLine();
                }

                bool result = false;
                string str_result = "";
                switch (checking) {
                    case "ascii":
                        foreach (int i in tx) {
                            if (i == 10 || i == 13)
                            {
                                continue;
                            }   
                            str_result += Convert.ToChar(i).ToString();
                        }
                        if (rxx == str_result && time.ElapsedMilliseconds < timeout)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case "equal":
                        rx = Convert.ToByte(rxx.Substring(2, 2), 16);
                        if(tx.Count > 1) {
                            if (rx == tx[1] && time.ElapsedMilliseconds < timeout)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            try { 
                                str_result = "0x" + tx[1].ToString("X2"); 
                                break; 
                            } catch { 
                                str_result = tx[1].ToString();
                            }
                            break;
                        } else {
                            if (rx == tx[0] && time.ElapsedMilliseconds < timeout)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            try { 
                                str_result = "0x" + tx[0].ToString("X2"); 
                                break; 
                            } catch { 
                                str_result = tx[0].ToString(); 
                            }
                            break;
                        }
                    case "bat115":
                        double valueBat = 0;
                        if (tx.Count >= 1)
                        {
                            valueBat = Convert.ToDouble(tx[0]);//แปลงจาก int เป็น double ไม่แคลชแน่
                            valueBat /= 10;
                            valueBat = map(valueBat, rx_min * 0.95, rx_max * 1.05, rx_min, rx_max);
                            valueBat = Convert.ToDouble(valueBat.ToString("#.0"));
                            if (valueBat >= rx_min && valueBat <= rx_max && time.IsRunning != true)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueBat.ToString("#.0");
                        }
                        else
                        {
                            result = false;
                            str_result = "not format:";
                            for (int loop = 0; loop < tx.Count; loop++)
                            {
                                str_result += tx[loop].ToString("X2") + " ";
                            }
                        }
                        break;
                    case "bat219":
                        if (tx.Count >= 1)
                        {
                            valueBat = Convert.ToDouble(tx[0]);
                            valueBat /= 10;
                            valueBat = map(valueBat, rx_min * 0.95, rx_max * 1.05, rx_min, rx_max);
                            valueBat = Convert.ToDouble(valueBat.ToString("#.0"));
                            if (valueBat >= rx_min && valueBat <= rx_max && time.IsRunning != true)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueBat.ToString("#.0");
                        }
                        else
                        {
                            result = false;
                            str_result = "not format:";
                            for (int loop = 0; loop < tx.Count; loop++)
                            {
                                str_result += tx[loop].ToString("X2") + " ";
                            }
                        }
                        break;
                    case "bat046":
                        if (tx.Count >= 1)
                        {
                            valueBat = Convert.ToDouble(tx[0]);
                            valueBat /= 10;
                            valueBat = map(valueBat, rx_min * 0.90, rx_max * 1.1, rx_min, rx_max);
                            valueBat = Convert.ToDouble(valueBat.ToString("#.0"));
                            if (valueBat >= rx_min && valueBat <= rx_max && time.IsRunning != true)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueBat.ToString("#.0");
                        }
                        else
                        {
                            result = false;
                            str_result = "not format:";
                            for (int loop = 0; loop < tx.Count; loop++)
                            {
                                str_result += tx[loop].ToString("X2") + " ";
                            }
                        }
                        break;
                    case "light":
                        if (tx.Count >= 1)
                        {
                            double valueLight = Convert.ToDouble(tx[0]);
                            valueLight /= 100;
                            if (valueLight >= rx_min && valueLight <= rx_max && time.IsRunning != true)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueLight.ToString();
                        }
                        else
                        {
                            result = false;
                            str_result = "not format:";
                            for (int loop = 0; loop < tx.Count; loop++)
                            {
                                str_result += tx[loop].ToString("X2") + " ";
                            }
                        }
                        break;
                    case "temp":
                        string valueToInt = string.Empty;
                        bool flagCheckTx = true;
                        if (tx.Count != 2)
                        {
                            flagCheckTx = false;
                        }
                        //LogTempError(tx);
                        if (flagCheckTx) {
                            valueToInt = tx[1].ToString("X2") + tx[0].ToString("X2");
                            int tempSup = Convert.ToInt32(valueToInt, 16);
                            double valueTemp = Convert.ToDouble(tempSup);
                            valueTemp /= 10;
                            if (valueTemp >= rx_min && valueTemp <= rx_max && !time.IsRunning)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueTemp.ToString();
                        } else {
                            result = false;
                            str_result = "not format:";
                            for (int loop = 0; loop < tx.Count; loop++) {
                                str_result += tx[loop].ToString("X2") + " ";
                            }
                        }
                        break;
                    case "jumper":
                        valueToInt = string.Empty;
                        flagCheckTx = true;
                        if (tx.Count != 2)
                        {
                            flagCheckTx = false;
                        } 
                        if (flagCheckTx) {
                            valueToInt = tx[0].ToString("X2") + " " + tx[1].ToString("X2");
                            if (valueToInt == rx_jumper)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueToInt;
                        } else {
                            result = false;
                            str_result = "not format";
                        }
                        break;
                    case "rssi":
                        valueToInt = string.Empty;
                        flagCheckTx = true;
                        if (tx.Count != 2)
                        {
                            flagCheckTx = false;
                        }
                        if (flagCheckTx) {
                            valueToInt = Convert.ToChar(tx[0]).ToString() + Convert.ToChar(tx[1]).ToString();
                            double valueRSSI = Convert.ToDouble(valueToInt);
                            if (valueRSSI >= rx_min && valueRSSI <= rx_max && time.IsRunning != true)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                            str_result = valueRSSI.ToString();
                        } else {
                            result = false;
                            str_result = "not format";
                        }
                        break;
                    case "digit":
                        if (digit == tx.Count && time.IsRunning != true)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        foreach (int i in tx) {
                            str_result += Convert.ToChar(i).ToString();
                        }
                        break;
                }
                if (result) {
                    File.WriteAllText("test_head_" + head + "_result.txt", str_result + "\r\nPASS");
                    mySerialPort.Close();
                    break;
                } else {
                    if (re < retest - 1) continue;
                    File.WriteAllText("test_head_" + head + "_result.txt", str_result + "\r\nFAIL");
                    mySerialPort.Close();
                }
            }
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
            Thread.Sleep(150);
            int length = 0;
            mySerialPort = (SerialPort)sender;
            try { 
                length = mySerialPort.BytesToRead;
            } catch {
                return; 
            }
            int buf = 0;
            for (int i = 0; i < length; i++) {
                buf = mySerialPort.ReadByte();
                //LogReceived(buf);
                Console.WriteLine("read: 0x" + buf.ToString("X2"));
                if (checking != "temp")
                {
                    if (buf == 10 || buf == 13)
                    {
                        //LogReceivedSkip(buf);
                        continue;
                    }
                }
                if (checking == "temp" && tx.Count >= 2)
                {
                    if (buf == 10 || buf == 13)
                    {
                        continue;
                    }
                }
                tx.Add(buf);
            }
            mySerialPort.DiscardInBuffer();
            mySerialPort.DiscardOutBuffer();
            if (tx.Count != 0)
            {
                flag_data = true;
            }
        }

        private static void discom(string cmd, string comport) {//enable disable//
            ManagementObjectSearcher objOSDetails2 =
               new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'");
            ManagementObjectCollection osDetailsCollection2 = objOSDetails2.Get();

            foreach (ManagementObject usblist in osDetailsCollection2) {
                string arrport = usblist.GetPropertyValue("NAME").ToString();
                if (arrport.Contains(comport)) {
                    Process devManViewProc = new Process();
                    devManViewProc.StartInfo.FileName = @"D:\svn\2020_SENSITECTH_SugarLoaf_Automation\2.Design Documents\7.Test Application Program\Bat file for programming\devmanview-x64\DevManView.exe";
                    devManViewProc.StartInfo.Arguments = "/" + cmd + " \"" + arrport + "\"";
                    devManViewProc.Start();
                    devManViewProc.WaitForExit();
                }
            }
        }

        private static double map(double s, double min_in, double max_in, double min_out, double max_out) {
            return min_out + (s - min_in) * (max_out - min_out) / (max_in - min_in);
        }

        private static bool flag_close = false;
        private static void TimerCallback(Object o) {
            if (!flag_close) { flag_close = true; return; }
            if (debug) return;
            if (flag_close) Environment.Exit(0);
        }
        /// <summary>
        /// Save log error to csv
        /// </summary>
        public static void LogTempError(List<int> tx) {
            string path = "D:\\LogTempError";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DateTime now = DateTime.Now;
            StreamWriter swOut = new StreamWriter(path + "\\" + now.Year + "_" + now.Month + ".csv", true);
            string time = now.Day.ToString("00") + ":" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
            string txSup = string.Empty;
            for (int loop = 0; loop < tx.Count; loop++)
            {
                txSup += tx[loop].ToString("X2");
            }
            swOut.WriteLine(time + ",Head=" + head + ",Tx=" + txSup);
            swOut.Close();
        }
        /// <summary>
        /// Save log rx to csv
        /// </summary>
        /// <param name="rx">is rx of serial read</param>
        public static void LogReceived(int rx) {
            string path = "D:\\LogReceived";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DateTime now = DateTime.Now;
            StreamWriter swOut = new StreamWriter(path + "\\" + now.Year + "_" + now.Month + ".csv", true);
            string time = now.Day.ToString("00") + ":" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
            swOut.WriteLine(time + ",Head=" + head + ",Rx=" + rx.ToString("X2"));
            swOut.Close();
        }
        /// <summary>
        /// Save log skip rx
        /// </summary>
        /// <param name="rx">is rx at skip</param>
        public static void LogReceivedSkip (int rx) {
            string path = "D:\\LogReceived";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DateTime now = DateTime.Now;
            StreamWriter swOut = new StreamWriter(path + "\\" + now.Year + "_" + now.Month + ".csv", true);
            string time = now.Day.ToString("00") + ":" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
            swOut.WriteLine(time + ",Head=" + head + ",Skip=" + rx.ToString("X2"));
            swOut.Close();
        }


        /// <summary>
        /// Log program catch to csv file
        /// </summary>
        /// <param name="text"></param>
        private static void LogProgramCatch(string text) {
            string path = "D:\\LogError\\SugarloafCmdCatch";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DateTime now = DateTime.Now;
            StreamWriter swOut = new StreamWriter(path + "\\" + now.Year + "_" + now.Month + ".csv", true);
            string time = now.Day.ToString("00") + ":" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
            swOut.WriteLine(time + "," + text);
            swOut.Close();
        }

        /// <summary>
        /// Event Exception Catch Program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MyHandler(object sender, UnhandledExceptionEventArgs args) {
            Exception e = (Exception)args.ExceptionObject;
            LogProgramCatch(e.StackTrace);
        }
    }
}
