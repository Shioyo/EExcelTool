# 使用方法
### 双击GenAll.bat文件开始导出
#### 默认Excel文件放当前的Excel目录下
#### 导出的Json文件和Cs都在当前对应的目录下

# Excel文件规则
#### 以~开头的Excel文件不导出,目的是防止导出编辑中的Excel临时文件
#### 已@pass和@pm结尾的文件不导出
#### Excel表格中带有@pass和@pm的字段不导出
#### 只能导出Excel文件的第一张表格,导出的Cs和Json文件名为Excel文件名而不是表格名

Todo
设置默认值暂时无效
Excel字段支持的字段目前只有int,string,float,double,后续考虑添加更多类型
目前只支持同时导出Cs和Json,后续会将这两个步骤分开
