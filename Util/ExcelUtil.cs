using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OfficeOpenXml;

namespace Util
{
    internal class ExcelUtil
    {
        public static List<string> GetAllExcelFilePath(string excelDirectory)
        {
            List<string> paths = Directory.GetFiles(excelDirectory).Where((s) =>
            {
                string fileName = Path.GetFileNameWithoutExtension(s);
                if (fileName.StartsWith("~"))
                {
                    return false;
                }else if (fileName.EndsWith("@pass") || fileName.EndsWith("@pm"))
                {
                    return false;
                }

                return true;
            }).ToList();

            return paths;
        }

        /// <summary>
        /// 编译生成链接生成的Cs文件
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        public static Assembly CompilerCs(List<string> fileContents)
        {
            var syntaxTrees = new List<SyntaxTree>();
            foreach (var content in fileContents)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(content));
            }

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                "GeneratedAssembly",
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            /*string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssembly.dll");

            using (var ms = new FileStream(outputPath, FileMode.Create))
            {
                compilation.Emit(ms);
            }*/

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic.ToString());
                    }

                    throw new Exception("Compilation failed.");
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
            //return Assembly.LoadFrom(outputPath);
        }

        public static void GetFileNameAndFileType(ExcelWorksheet workSheet, List<CsGenerator.ExcelCell> fileNames,
            List<CsGenerator.ExcelCell> fileTypes)
        {
            // 获取行数和列数
            int rowCount = workSheet.Dimension.Rows;
            int colCount = workSheet.Dimension.Columns;
            for (int col = 1; col <= colCount; col++)
            {
                //这一行的是字段名
                string propertyName = workSheet.Cells[1, col].Value.ToString();
                string propertyType = workSheet.Cells[3, col].Value.ToString();

                if (propertyName.EndsWith("@pass") || propertyName.EndsWith("@pm"))
                {
                    return;
                }

                fileNames.Add(new CsGenerator.ExcelCell(1, col, propertyName));
                fileTypes.Add(new CsGenerator.ExcelCell(3, col, propertyType));
            }
        }

        private ExcelUtil()
        {
        }
    }
}