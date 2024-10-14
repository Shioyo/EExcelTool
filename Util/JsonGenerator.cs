using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Util
{
    public class JsonGenerator
    {
        private readonly string exportDirectory;
        private readonly string excelDirectory;

        public JsonGenerator(string excelDirectory, string exportDirectory)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.excelDirectory = excelDirectory;
            this.exportDirectory = exportDirectory;
        }

        public void GenJsonText(List<string> csScripts)
        {
            Console.WriteLine("Compiler cs files");
            Assembly assembly = ExcelUtil.CompilerCs(csScripts);
            Console.WriteLine("End Compiler");
            GenJsonText(assembly);
        }

        /// <summary>
        /// Excel文件导出成Json文件
        /// </summary>
        /// <param name="assembly">Excel生成的Cs文件的Assembly</param>
        public void GenJsonText(Assembly assembly)
        {
            Console.WriteLine("\n ------Start Gen Json...------\n");

            List<string> filePaths = ExcelUtil.GetAllExcelFilePath(excelDirectory);
            foreach (var filePath in filePaths)
            {
                Dictionary<int, PropertyInfo> propertyInfos = new Dictionary<int, PropertyInfo>();
                Dictionary<int, string> propertyTypeInfos = new Dictionary<int, string>();
                using var excelPackage = new ExcelPackage(new FileInfo(filePath));
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                var type = assembly.GetType(Path.GetFileNameWithoutExtension(filePath));

                List<CsGenerator.ExcelCell> fileNames = new List<CsGenerator.ExcelCell>();
                List<CsGenerator.ExcelCell> fileTypes = new List<CsGenerator.ExcelCell>();
                ExcelUtil.GetFileNameAndFileType(workSheet, fileNames, fileTypes);
                foreach (var cell in fileNames)
                {
                    propertyInfos.Add(cell.Col, type.GetProperty(fileNames[cell.Col - 1].Content));
                    propertyTypeInfos.Add(cell.Col, fileTypes[cell.Col - 1].Content);
                }

                ArrayList serializeList = new ArrayList();
                for (int row = 5; row <= workSheet.Dimension.Rows; row++)
                {
                    var @target = Activator.CreateInstance(type);
                    foreach (var cell in fileNames)
                    {
                        propertyInfos[cell.Col].SetValue(@target,
                            Convert.ChangeType(workSheet.Cells[row, cell.Col].Value,
                                MapStringToMap(propertyTypeInfos[cell.Col])));
                    }

                    serializeList.Add(@target);
                }

                string s = JsonConvert.SerializeObject(serializeList);
                Console.WriteLine($"{type}.json");
                File.WriteAllText(exportDirectory + $"\\{type}.json", s);
            }
        }

        /// <summary>
        /// 将string,int等的数据类型映射到对应类型
        /// </summary>
        /// <returns></returns>
        public static Type MapStringToMap(string name)
        {
            switch (name)
            {
                case "string":
                    return typeof(string);
                case "int":
                    return typeof(int);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                default:
                    throw new Exception($"无法识别的类型{name}");
                    return typeof(string);
            }
        }
    }
}