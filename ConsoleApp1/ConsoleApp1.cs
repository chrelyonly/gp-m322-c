using System.Text;

public class ConsoleApp1
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("作者: chrelyonly\n语言: C#\n通过命令行传输参数\n第一个参数为功能类型: \n 1.检查USB并识别打印机\n 2.打印标签, 其他-退出 ,(如果传2那么填写价签信息)\n 例如: 1 或 2 门店名称 商品名称 单位 价格 条码");
            return;
        }
        string command = args[0];
        if (command == "1")
        {
            var usbOperation = new libUsbContorl.UsbOperation();
            var findUsbPrinter = usbOperation.FindUSBPrinter();
            if (findUsbPrinter)
            {
                Console.WriteLine("USB 打印机已找到。");
                var usbOperationUsbPortCount = usbOperation.USBPortCount;
                Console.WriteLine($"USB 端口数量: {usbOperationUsbPortCount}");
            }
            else
            {
                Console.WriteLine("未找到 USB 打印机。");
            }
            // 关闭 USB 端口
            usbOperation.CloseUSBPort();
        }
        else if (command == "2")
        {
            // 调用打印标签方法
            printLabel(args[1..]); // 传递除第一个参数外的其他参数
        }
        else
        {
            Console.WriteLine("退出程序。");
        }
    }


    /**
     * 打印标签
     * 参数：门店名称 商品名称 单位 价格 条码
     */
    private static void printLabel(string[] args)
    {

        // 检查命令行参数数量，应该是5个：4个文本 + 1个条码
        if (args.Length != 5)
        {
            Console.WriteLine("请提供6个参数：门店名称 商品名称 单位 价格 条码");
            return;
        }

        string text1 = args[0]; // 第一个文本
        string text2 = args[1]; // 第二个文本
        string text3 = args[2]; // 第三个文本
        string text4 = args[3]; // 第四个文本
        string barcode = args[4]; // 条码

        // 创建 USB 操作对象
        var usbOperation = new libUsbContorl.UsbOperation();

        // 查找 USB 打印机
        var findUsbPrinter = usbOperation.FindUSBPrinter();
        if (!findUsbPrinter)
        {
            Console.WriteLine("未找到 USB 打印机。");
            return;
        }

        Console.WriteLine("USB 打印机已找到。");

        // 获取 USB 端口数量
        var usbOperationUsbPortCount = usbOperation.USBPortCount;
        Console.WriteLine($"USB 端口数量: {usbOperationUsbPortCount}");

        // 连接到第一个 USB 打印机
        var linkUsb = usbOperation.LinkUSB(0);
        if (!linkUsb)
        {
            Console.WriteLine("连接 USB 打印机失败。");
            return;
        }

        // 构建 TSPL 打印指令
        string tsplCommand = $@"
SIZE 70 mm,40 mm
GAP 2 mm,0
CLS
DENSITY 8
SPEED 4
SET TEAR ON
REFERENCE 0,0

TEXT 240,24,""TSS24.BF2"",0,1,1,""{text1}""
TEXT 118,75,""TSS24.BF2"",0,1,1,""{text2}""
TEXT 220,130,""TSS24.BF2"",0,1,1,""{text3}""
TEXT 380,180,""TSS24.BF2"",0,2,2,""{text4}""
BARCODE 75,190,""128M"",45,1,0,1,2,""{barcode}""

PRINT 1
";

        // 注册编码提供器（支持 GB18030）
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 将 TSPL 指令编码为 GB18030
        byte[] command = Encoding.GetEncoding("GB18030").GetBytes(tsplCommand);

        // 发送指令到 USB 打印机
        usbOperation.SendData2USB(command, command.Length);

        // 关闭 USB 端口
        usbOperation.CloseUSBPort();
    }
}