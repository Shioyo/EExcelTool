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
        private readonly string t4EnumFileContent;

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
            resourceName = "Util.T4Template.GenCsEnumTemplate.tt";
            // 使用 ManifestResourceStream 读取嵌入资源
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 读取嵌入资源中的内容
                    t4EnumFileContent = reader.ReadToEnd();
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
                List<ExcelCell> orNames = new List<ExcelCell>();
                List<ExcelCell> orTypes = new List<ExcelCell>();
                ExcelUtil.GetFileNameAndFileType(workSheet, propertyNames, propertyTypes,orNames,orTypes);
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
                
                
                await using (StreamWriter streamWriter = new StreamWriter(File.OpenWrite(outputPath)))
                {
                    await streamWriter.WriteAsync(data.content);
                }

                Console.WriteLine($"{className}.cs");
                genClassContent.Add(data.content);
                
                //查看Enum是否需要生成

                for (int i = 0; i < orNames.Count; i++)
                {
                    if (orNames[i].Content.EndsWith("@enum"))
                    {
                        className = orTypes[i].Content;
                        string enumNames="";
                        for (int row = 5; row <= workSheet.Dimension.Rows; row++)
                        {
                            object cellValue = workSheet.Cells[row, orNames[i].Col].Value;
                            if (cellValue!=null)
                            {
                                if (!string.IsNullOrEmpty(cellValue.ToString()))
                                {
                                    if (!string.IsNullOrEmpty(enumNames))
                                    {
                                        enumNames += ";";
                                    }
                                    enumNames+=cellValue;
                                } 
                            }
                        }
                        Dictionary<string, object> enumProperties = new Dictionary<string, object>()
                        {
                            { "className", className },
                            { "enumNames", enumNames },
                            { "genFileTime", DateTime.UtcNow.ToString() }
                        };
                        session.Clear();
                        foreach (var prop in enumProperties)
                        {
                            session[prop.Key] = prop.Value;
                        }
                        outputPath = $"{outputDirectory}\\{className}.cs";
                        data = await generator.ProcessTemplateAsync("GenCsEnumTemplate.tt", t4EnumFileContent, className + ".cs");
                
                
                        await using (StreamWriter streamWriter = new StreamWriter(File.OpenWrite(outputPath)))
                        {
                            await streamWriter.WriteAsync(data.content);
                        }

                        Console.WriteLine($"{className}.cs");
                        genClassContent.Add(data.content);
                    }
                }
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