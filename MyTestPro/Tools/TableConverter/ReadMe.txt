TableConverter是一个根据指定目录下的Excel文件生成相对应的二进制数据文件以及配套的代码的工具.

TableConverter的使用依赖Python, 在编写该工具时使用的Python版本为3.6
通过运行TableConverter.py, 并传入相对应的参数来使用该工具

TableConverter的参数以及意义:
    --excel_path        指向Excel文件所在的目录
    --out_code_path     指向生成代码的目录, 该目录将会在生成时并清空, 所以请保持该目录仅作为生成使用
    --out_data_path     指向生成数据所在的目录, 一般指向StreamingAssets或者StreamingAssets的子目录, 生成数据文件以table.bin为文件名
    --out_data_file     指定生成数据的的文件名
    --checker_path      指向数据检测所在目录
    --namespace         生成代码使用的命名空间

TableConverter的使用提示:
    1. 两个表单不能有相同的名字
    2. 不希望被处理的表单以"Sheet"开头
    3. "des_"开头的列是注释列, 允许该列为空, 且不需要标识类型
    4. 列表类型的元素与元素之间使用 | 作为分隔符

TableConverter所支持的类型标识:
    E: ENUM, 枚举
    I: INT, 整数
    F: FLOAT, 浮点数
    S: STRING, 字符串
    B: BOOL, 布尔值
    LI: LIST_INT, 整数列表
    LF: LIST_FLOAT, 浮点数列表
    LS: LIST_STRING, 字符串列表
    LE: LIST_ENUM, 枚举类型列表