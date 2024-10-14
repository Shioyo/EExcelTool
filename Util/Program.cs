namespace Util
{
    class Program
    {
        /// <summary>
        /// do
        /// </summary>
        /// <param name="args">参数0为Excel文件路径 T4文件路径 参数1为导出的Cs文件路径 参数2为导出的Json文件路径</param>
        static async Task Main(string[] args)
        {
            //Excel文件路径
            string excelFolder = args[0];
            //导出Cs文件路径
            string csScriptFolder = args[1];
            //导出Json文件路径
            string jsonFolder = args[2];

            CsGenerator csGenerator = new CsGenerator(excelFolder,csScriptFolder);
            JsonGenerator jsonGenerator = new JsonGenerator(excelFolder,jsonFolder);
            List<string> contents = await csGenerator.ProcessT4();
            jsonGenerator.GenJsonText(contents);
        }
    } 
}