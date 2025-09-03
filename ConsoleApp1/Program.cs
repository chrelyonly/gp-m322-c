using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    static void Main()
    {
        var usbOperation = new libUsbContorl.UsbOperation();
        var findUsbPrinter = usbOperation.FindUSBPrinter();
        if (!findUsbPrinter)
        {
            Console.WriteLine("No USB printer found.");
            return;
        }
        Console.WriteLine("USB printer found.");


        var usbOperationUsbPortCount = usbOperation.USBPortCount;
        Console.WriteLine($"Number of USB ports: {usbOperationUsbPortCount}");
        var linkUsb = usbOperation.LinkUSB(0);
        if (!linkUsb)
        {
            Console.WriteLine("Failed to link USB printer.");
            return;
        }
        // TSPL 指令示例
        string tsplCommand = @"
SIZE 70 mm,40 mm
GAP 2 mm,0
CLS
DENSITY 8
SPEED 4
SET TEAR ON
REFERENCE 0,0
TEXT 240,24,""TSS24.BF2"",0,1,1,""雪飘人间-测试门店""

TEXT 118,70,""TSS24.BF2"",0,1,1,""雕牌清新柠檬洗衣液 2kg""

TEXT 220,130,""TSS24.BF2"",0,1,1,""kg""

TEXT 380,220,""TSS24.BF2"",0,2,2,""29.9""

BARCODE 75,200,""128M"",45,1,0,1,2,""41234123412342232452""

PRINT 1 

";
        // 注册编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // TSPL 通常用 ASCII 编码
        // byte[] command = Encoding.UTF8.GetBytes(tsplCommand);
        byte[] command = Encoding.GetEncoding("GB18030").GetBytes(tsplCommand);
        usbOperation.SendData2USB(command, command.Length);
        usbOperation.CloseUSBPort();
    }
}
