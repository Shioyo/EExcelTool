using System.Reflection;
using Mono.TextTemplating;
using OfficeOpenXml;

namespace Util
{
    public class CsGenerator
    {
        private readonly string excelDirectory;
        private readonly string outputDirectory;
        private readonly string t4FileContent;

        public CsGenerator(string excelDirectory, string outputDirectory)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.excelDirectory = excelDirectory;
            this.outputDirectory = outputDirectory;
            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Util.T4Template.GenCsTemplate.tt";
            // 使用 ManifestResourceStream 读取嵌入资源
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 读取嵌入资源中的内容
                    t4FileContent = reader.ReadToEnd();
                }
            }
        }

        // <summary>
        /// 通过T4文件生成Cs文件
        /// </summary>
        /// <param name="exportDirectory">生成Cs文件Assembly</param>
        /// <returns></returns>
        public async Task<List<string>> ProcessT4()
        {
            Console.WriteLine("\n ------Start Gen Cs... ------ \n");
            List<string> genClassContent = new List<string>();

            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var generator = new TemplateGenerator();
            //处理所有Excel文件
            List<string> filePaths = ExcelUtil.GetAllExcelFilePath(excelDirectory);
            foreach (var filePath in filePaths)
            {
                using var excelPackage = new ExcelPackage(new FileInfo(filePath));
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                //获取到需要处理的字段以及对应的类型
                List<ExcelCell> propertyNames = new List<ExcelCell>();
                List<ExcelCell> propertyTypes = new List<ExcelCell>();
                ExcelUtil.GetFileNameAndFileType(workSheet, propertyNames, propertyTypes);
                //处理后传入T4模版作为参数
                //准备T4文件生成
                var session = generator.GetOrCreateSession();
                string propNameString = string.Empty;
                string propTypeString = string.Empty;
                for (int i = 0; i < propertyNames.Count; i++)
                {
                    propNameString += propertyNames[i].Content;
                    propTypeString += propertyTypes[i].Content;
                    if (i != propertyNames.Count - 1)
                    {
                        propNameString += ";";
                        propTypeString += ";";
                    }
                }

                string className = Path.GetFileNameWithoutExtension(filePath);
                Dictionary<string, object> properties = new Dictionary<string, object>()
                {
                    { "className", className },
                    { "propertyNameString", propNameString },
                    { "propertyTypeString", propTypeString },
                    { "genFileTime", DateTime.UtcNow.ToString() }
                };
                foreach (var prop in properties)
                {
                    session[prop.Key] = prop.Value;
                }

                string outputPath = $"{outputDirectory}\\{className}.cs";
                var data = await generator.ProcessTemplateAsync("GenCsTemplate.tt", t4FileContent, className + ".cs");
                using (StreamWriter streamWriter = new StreamWriter(File.OpenWrite(outputPath)))
                {
                    await streamWriter.WriteAsync(data.content);
                }

                Console.WriteLine($"{className}.cs");
                genClassContent.Add(data.content);
            }

            return genClassContent;
        }

        internal class ExcelCell
        {
            public int Row;
            public int Col;
            public string Content;

            public ExcelCell(int row, int col, string content)
            {
                Row = row;
                Col = col;
                Content = content;
            }
        }
    }
}