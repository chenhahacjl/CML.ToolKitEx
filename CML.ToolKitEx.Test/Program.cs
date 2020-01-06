using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 编程工具包测试类
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 消息长度
        /// </summary>
        private static readonly int MsgLength = 60;

        /// <summary>
        /// 测试类主方法
        /// </summary>
        private static void Main()
        {
            //获取类型列表
            List<Type> testClassTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();

            //整理测试类
            for (int i = testClassTypes.Count - 1; i >= 0; i--)
            {
                if (!testClassTypes[i].IsSubclassOf(typeof(ToolkitTestBase)))
                {
                    testClassTypes.Remove(testClassTypes[i]);
                }
            }

            //测试类排序
            testClassTypes.Sort((x, y) => x.Name.CompareTo(y.Name));

            if (testClassTypes.Count == 0)
            {
                Console.WriteLine(PadCenter(string.Empty, '='));
                Console.WriteLine(PadCenter("测试项目为空，请先编写测试项目"));
                Console.WriteLine(PadCenter(string.Empty, '='));
            }
            else
            {
                //测试实例字典
                Dictionary<string, ToolkitTestBase> dicInstance = new Dictionary<string, ToolkitTestBase>();
                //测试结果字典
                Dictionary<string, bool> dicTestResult = new Dictionary<string, bool>();

                //显示测试项目
                Console.WriteLine(PadCenter(string.Empty, '='));
                Console.WriteLine(PadCenter($"检测到{testClassTypes.Count}个测试项目"));
                Console.WriteLine(PadCenter(string.Empty, '-'));
                for (int i = 0; i < testClassTypes.Count; i++)
                {
                    //实例化测试项目
                    ToolkitTestBase toolKitTest = (ToolkitTestBase)Activator.CreateInstance(testClassTypes[i], true);
                    //保存实例
                    dicInstance.Add(Guid.NewGuid().ToString(), toolKitTest);
                    //输出名称
                    Console.WriteLine($"[{i + 1}]{toolKitTest.ClassName} => {toolKitTest.UpdateDate}");
                }
                Console.WriteLine(PadCenter(string.Empty, '=') + "\n");

                //判断输入
                Console.Write("输入'Y'开始测试: ");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    //进行测试，显示Y
                    Console.WriteLine("Y\n");

                    Console.WriteLine(PadCenter(string.Empty, '='));
                    Console.WriteLine(PadCenter("开始测试"));
                    Console.WriteLine(PadCenter(string.Empty, '=') + "\n");

                    //依次执行测试
                    for (int i = 0; i < dicInstance.Count; i++)
                    {
                        //分隔符
                        Console.WriteLine(PadCenter(string.Empty, '='));

                        //显示测试包测试类名
                        Console.WriteLine("[测试类名]" + dicInstance[dicInstance.Keys.ToArray()[i]].ClassName);

                        //显示测试包更新时间
                        Console.WriteLine("[更新时间]" + dicInstance[dicInstance.Keys.ToArray()[i]].UpdateDate);

                        //显示测试包版本信息
                        Console.WriteLine("[版本信息]" + dicInstance[dicInstance.Keys.ToArray()[i]].VersionInfo);

                        //分隔符
                        Console.WriteLine(PadCenter(string.Empty, '-'));
                        Console.WriteLine(PadCenter("测试信息", ' '));
                        Console.WriteLine(PadCenter(string.Empty, '-'));

                        //执行测试
                        bool testResult = dicInstance[dicInstance.Keys.ToArray()[i]].ExecuteTest();

                        //保存测试结果
                        dicTestResult.Add(dicInstance.Keys.ToArray()[i], testResult);

                        //显示测试结果
                        Console.WriteLine(PadCenter(string.Empty, '-'));
                        Console.ForegroundColor = testResult ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine(PadCenter($"测试项目{dicInstance[dicInstance.Keys.ToArray()[i]].ClassName}测试{(testResult ? "通过" : "未通过")}！"));
                        Console.ResetColor();
                        Console.WriteLine(PadCenter(string.Empty, '=') + "\n");
                    }

                    //打印日志
                    Console.WriteLine(PadCenter(string.Empty, '='));
                    Console.WriteLine(PadCenter("编程工具包测试结果"));
                    Console.WriteLine(PadCenter(string.Empty, '-'));
                    for (int i = 0; i < dicTestResult.Count; i++)
                    {
                        Console.Write($"{dicInstance[dicTestResult.Keys.ToArray()[i]].ClassName.PadRight(MsgLength - 6, '.')}[");
                        if (dicTestResult.Values.ToArray()[i])
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("PASS");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("FAIL");
                        }

                        Console.ResetColor();
                        Console.WriteLine("]");
                    }
                    Console.WriteLine(PadCenter(string.Empty, '='));

                }
                else
                {
                    //取消测试，显示N
                    Console.WriteLine("N\n");

                    //打印日志
                    Console.WriteLine(PadCenter(string.Empty, '='));
                    Console.WriteLine(PadCenter("取消测试"));
                    Console.WriteLine(PadCenter(string.Empty, '='));
                }
            }

            //暂停展示结果
            Console.Write("\n任意键退出测试程序。。。");
            Console.ReadKey();
        }

        /// <summary>
        /// 获取居中消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="padChar">填充字符</param>
        /// <returns>填充消息</returns>
        private static string PadCenter(string msg, char padChar = ' ')
        {
            if (MsgLength < GetMsgLength(msg))
            {
                return msg;
            }
            else
            {
                int padLeft = (MsgLength - GetMsgLength(msg)) / 2;
                int padRight = MsgLength - GetMsgLength(msg) - padLeft;

                return string.Empty.PadLeft(padLeft, padChar) + msg + string.Empty.PadRight(padRight, padChar);
            }
        }

        /// <summary>
        /// 获取消息长度（汉字算2个长度）
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns>消息长度</returns>
        public static int GetMsgLength(string msg)
        {
            int length = 0;

            new ASCIIEncoding().GetBytes(msg)
                .ToList()
                .ForEach(item => length += item == 63 ? 2 : 1);

            return length;
        }
    }
}
