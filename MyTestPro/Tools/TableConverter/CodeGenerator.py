import re
import abc
import xlrd
import codecs
from Globals import *
from Utils import *


class CSCode:
    def __init__(self, file_name):
        self.name = file_name
        self.namespace = None

    def set_namespace(self, namespace):
        self.namespace = namespace


class CNamespace:
    def __init__(self, name):
        self.name = name
        self.classes = []

    def add_class(self, cclass):
        self.classes.append(cclass)


class CClass:
    def __init__(self, name):
        self.name = name
        self.attributes = []
        self.structures = []
        self.enums = []

    def add_attribute(self, attribute):
        self.attributes.append(attribute)

    def add_structure(self, subclass):
        self.structures.append(subclass)

    def add_enum(self, enum):
        self.enums.append(enum)


class CStructure:
    def __init__(self, name):
        self.name = name
        self.attributes = []

    def add_attribute(self, attribute):
        self.attributes.append(attribute)


class CAttribute:
    def __init__(self, name, type, reference_class):
        self.name = name
        self.type = type
        self.reference_class = reference_class
        self.column = -1

    def set_column_index(self, index):
        self.column = index


class CEnum:
    def __init__(self, name):
        self.name = name
        self.values = []

    def add_value(self, value):
        self.values.append(value)


class CodeWriter:
    def __init__(self, cscode):
        self.cscode = cscode
        self.filestream = None
        self.intent = 0

    def write_code(self):
        file_path = TEMP_CODE_FOLDER_NAME + '/' + self.cscode.name
        Utils.create_folder(file_path)
        self.filestream = codecs.open(file_path, 'w', 'utf-8_sig')
        self.write_header()
        self.write_namespace()
        self.filestream.close()

    def write_namespace(self):
        self.write("\n")
        self.write("namespace {namespace}\n{{\n".format(
            namespace=self.cscode.namespace.name))
        for c in self.cscode.namespace.classes:
            self.write_class(1, c)
        self.write("} //namespace\n")
        pass

    def write_class(self, indent, cclass):
        self.filestream.write("{indent}public class {className} \n{indent}{{\n".format(
            indent=self.get_indent(indent), className=cclass.name))

        for e in cclass.enums:
            self.write_enum(indent + 1, e)

        for s in cclass.structures:
            self.write_struct(indent + 1, s)

        for a in cclass.attributes:
            self.write_attribute(indent + 1, a)

        self.write_init_function(indent + 1, cclass)

        self.write_get_function(indent + 1, cclass)

        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))
        pass

    def write_struct(self, indent, structure):
        self.write("\n{indent}public class {name}\n{indent}{{\n".format(
            indent=self.get_indent(indent), name=structure.name))

        for a in structure.attributes:
            self.write_attribute(indent + 1, a)

        self.write_struct_init_function(indent + 1, structure)

        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))

    def write_struct_init_function(self, indent, structure):
        self.write("\n{indent}public void Init(\n".format(
            indent=self.get_indent(indent)))

        for a in structure.attributes:
            type_name = self.get_attribute_type_name(a)
            length = len(structure.attributes)
            end = "," if structure.attributes.index(a) < length - 1 else ""
            self.write("{indent}{attribute_type_name} {attribute_name}{end}\n".format(
                indent=self.get_indent(indent + 1),
                attribute_type_name=type_name,
                attribute_name=a.name,
                end=end))

        self.write("{indent})\n{indent}{{\n".format(
            indent=self.get_indent(indent)))
        for a in structure.attributes:
            self.write("{indent}this.{attribute_name} = {attribute_name};\n".format(
                indent=self.get_indent(indent + 1),
                attribute_name=a.name))
        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))

    def write_attribute(self, indent, attribute):
        type_name = self.get_attribute_type_name(attribute)
        self.write("{indent}public {attribute_type_name}  {attribute_name} {{get; private set;}}\n".format(
            indent=self.get_indent(indent),
            attribute_type_name=type_name,
            attribute_name=attribute.name))

    def get_attribute_type_name(self, attribute):
        type_name = None
        if attribute.type == ENUM:
            type_name = "{class_name}.{enum_name}".format(
                class_name=attribute.reference_class.namespace.classes[0].name,
                enum_name=EDATA)
        elif attribute.type == LIST_ENUM:
            type_name = "List<{class_name}.{enum_name}>".format(
                class_name=attribute.reference_class.namespace.classes[0].name,
                enum_name=EDATA)
        else:
            type_name = attribute.type
        return type_name

    def write_init_function(self, indent, cclass):
        pass

    def write_get_function(self, indent, cclass):
        pass

    def write_header(self):
        self.write("using System;\n"
                   "using System.Collections;\n"
                   "using System.Collections.Generic;\n"
                   "using UnityEngine;\n"
                   "using Framework.Common;\n")
        pass

    def write(self, string):
        self.filestream.write(string)

    def write_enum(self, indent, enum):
        self.write("{indent}public enum {name}\n{indent}{{\n".format(
            indent=self.get_indent(indent), name=enum.name))
        self.write("{indent}{enum_value} = 0,\n".format(
            indent=self.get_indent(indent + 1), enum_value=enum.values[0]))
        for i in range(1, len(enum.values)):
            self.write("{indent}{enum_value},\n".format(
                indent=self.get_indent(indent + 1), enum_value=enum.values[i]))
        self.write("{indent}}};\n".format(indent=self.get_indent(indent)))

    def get_indent(self, indent):
        str = ''
        for i in range(indent * 4):
            str += ' '
        return str


class ItemCodeWriter(CodeWriter):
    def write_init_function(self, indent, cclass):
        self.write("\n{indent}private List<Data> _dataList = null;\n".format(
            indent=self.get_indent(indent)))
        self.write("\n{indent}".format(
            indent=self.get_indent(indent)))
        self.write("public List<Data> DataList { get { return _dataList; } }\n")

        if cclass.structures[0] != None:
            structure = cclass.structures[0]
            self.write("\n{indent}public void Init(BufferHelper helper)\n{indent}{{\n".format(
                indent=self.get_indent(indent)))
            self.write("{indent}var num = helper.ReadInt32();\n".format(
                indent=self.get_indent(indent + 1)))
            self.write('{indent}if(num < (int)EData.Count)\n{indent}{{\n{indent2}throw new Exception(" invalid ");\n{indent}}}\n'.format(
                indent=self.get_indent(indent + 1), indent2=self.get_indent(indent + 2)))

            self.write("\n{indent}_dataList = new List<Data>(num);\n".format(
                indent=self.get_indent(indent + 1)))

            self.write("{indent}for(var i = 0; i < num; ++i)\n{indent}{{\n".format(
                indent=self.get_indent(indent + 1)))
            self.write("{indent}var data = new Data();\n".format(
                indent=self.get_indent(indent + 2)))

            # write init function
            self.write("{indent}data.Init(\n".format(
                indent=self.get_indent(indent + 2)))
            for a in structure.attributes:
                length = len(structure.attributes)
                i = self.get_indent(indent + 3)
                end = "," if structure.attributes.index(a) < length - 1 else ""
                if a.type == INT:
                    self.write("{indent}{attributeName} : helper.ReadInt32(){end}\n".format(
                        indent=i, attributeName=a.name, end=end))
                elif a.type == FLOAT:
                    self.write("{indent}{attributeName} : helper.ReadFloat(){end}\n".format(
                        indent=i, attributeName=a.name, end=end))
                elif a.type == STRING:
                    self.write("{indent}{attributeName} : helper.ReadStr(){end}\n".format(
                        indent=i, attributeName=a.name, end=end))
                elif a.type == BOOL:
                    self.write("{indent}{attributeName} : helper.ReadBool(){end}\n".format(
                        indent=i, attributeName=a.name, end=end))
                elif a.type == ENUM:
                    self.write("{indent}{attributeName} : ({cclass}.{edata})helper.ReadInt32(){end}\n".format(indent=i,
                                                                                                              attributeName=a.name,
                                                                                                              cclass=a.reference_class.namespace.classes[
                                                                                                                  0].name,
                                                                                                              edata=EDATA, end=end))
                elif a.type == LIST_INT:
                    self.write("{indent}{attribute_name} : helper.ReadInt32List(){end}\n".format(
                        indent=i, attribute_name=a.name, end=end))
                elif a.type == LIST_FLOAT:
                    self.write("{indent}{attribute_name} : helper.ReadFloatList(){end}\n".format(
                        indent=i, attribute_name=a.name, end=end))
                elif a.type == LIST_STRING:
                    self.write("{indent}{attribute_name} : helper.ReadStrList(){end}\n".format(
                        indent=i, attribute_name=a.name, end=end))
                elif a.type == LIST_ENUM:
                    self.write("{indent}{attribute_name} : helper.ReadEnumList({cclass}.{edata}.Count){end}\n".format(
                        indent=i, attribute_name=a.name, cclass=a.reference_class.namespace.classes[0].name, edata=EDATA, end=end))
                else:
                    raise Exception(
                        "Write init function has unsupported value type.")

            self.write("{indent});\n".format(
                indent=self.get_indent(indent + 2)))
            self.write("\n{indent}_dataList.Add(data);\n".format(
                indent=self.get_indent(indent + 2)))
            self.write("{indent}}}\n".format(
                indent=self.get_indent(indent + 1)))

        self.write("{}}}\n".format(self.get_indent(indent)))
        pass

    def write_get_function(self, indent, cclass):
        self.write("\n{indent}public Data getData(EData e)\n{indent}{{\n{indent2}return _dataList[Convert.ToInt32(e)];\n{indent}}}\n".format(
            indent=self.get_indent(indent), indent2=self.get_indent(indent + 1)))
        self.write("\n{indent}public Data getData(int id)\n{indent}{{\n{indent2}return _dataList[id];\n{indent}}}\n".format(
            indent=self.get_indent(indent), indent2=self.get_indent(indent + 1)))
        pass


class ManagerCodeWriter(CodeWriter):
    def write_init_function(self, indent, cclass):
        self.write("\n")
        self.write("{indent}public IEnumerator Init(string filePath, Action onFinish = null)\n{indent}{{\n".format(
            indent=self.get_indent(indent)))
        self.write('{indent}var www = UnityEngine.Networking.UnityWebRequest.Get(filePath);\n{indent}yield return www.SendWebRequest();\n\n{indent}var helper = new BufferHelper(www.downloadHandler.data);\n'.format(
            indent=self.get_indent(indent + 1),
            data_file_name=DATA_FILE_NAME))

        for a in cclass.attributes:
            self.write("\n{indent}this.{attribute_name} = new {attribute_name}();\n{indent}{attribute_name}.Init(helper);\n".format(
                indent=self.get_indent(indent + 1), attribute_name=a.name))

        self.write('\n{indent}if (onFinish != null) onFinish();\n'.format(
            indent=self.get_indent(indent + 1)))

        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))

        # add init function (parameter is buffer)
        self.write("\n")
        self.write('{indent}public void Init(byte[] buffer)\n{indent}{{\n'.format(
            indent=self.get_indent(indent)))
        self.write('{indent}var helper = new BufferHelper(buffer);'.format(indent=self.get_indent(indent+1)))

        for a in cclass.attributes:
            self.write("\n{indent}this.{attribute_name} = new {attribute_name}();\n{indent}{attribute_name}.Init(helper);\n".format(
                indent=self.get_indent(indent + 1), attribute_name=a.name))
        
        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))

        # add init func from Resources. add by zSz
        temp = '''
        public bool InitByResources(string filePath)
        {
            var txtAsset = Resources.Load<TextAsset>(filePath);
            if (txtAsset == null || txtAsset.bytes == null)
                return false;
            
            var helper = new BufferHelper(txtAsset.bytes);
        '''
        self.write(temp)
        for a in cclass.attributes:
            self.write("\n{indent}this.{attribute_name} = new {attribute_name}();\n{indent}{attribute_name}.Init(helper);\n".format(
                indent=self.get_indent(indent + 1), attribute_name=a.name))

        self.write('\n{indent}return true;\n'.format(
            indent=self.get_indent(indent + 1)))
            
        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))

        temp = '''
        public bool InitByBytes(byte[] bytes)
        {
            var helper = new BufferHelper(bytes);
        '''
        self.write(temp)
        for a in cclass.attributes:
            self.write("\n{indent}this.{attribute_name} = new {attribute_name}();\n{indent}{attribute_name}.Init(helper);\n".format(
                indent=self.get_indent(indent + 1), attribute_name=a.name))

        self.write('\n{indent}return true;\n'.format(
            indent=self.get_indent(indent + 1)))

        self.write("{indent}}}\n".format(indent=self.get_indent(indent)))
    pass


class CodeGenerator:
    def __init__(self, namespace):
        self.namespace = namespace
        self.sheet = None
        self.cscodes = []

    def generate_sheet_code(self, sheet):
        self.sheet = sheet

        enum = CEnum("EData")
        enums = self.get_enum_values()
        for item in enums:
            enum.add_value(item)
        enum.add_value("Count")

        csname = Utils.get_cscode_name(sheet)
        cscode = CSCode(csname)
        namespace = CNamespace(self.namespace)
        root_class = CClass(self.sheet.name)
        structure = CStructure("Data")

        for i in range(1, self.sheet.ncols):
            attributes = self.sheet.row_values(ATTRIBUTE_ROW_INDEX)

            attribute = attributes[i]
            if attribute.upper().startswith(DES):
                continue

            attribute_type, attribute_name, reference_class_name = Utils.get_attribute_info(
                attributes[i])
            reference_class = None

            if (attribute_type == LIST_ENUM or attribute_type == ENUM) and attribute_name != ID:
                if reference_class_name == None:
                    raise Exception("表单{}{}没有指定索引表单".format(
                        sheet.name, attribute))
                reference_class = self.get_cscode_by_name(reference_class_name)
                if reference_class == None:
                    raise Exception("表单{}{}没有找到索引表单".format(
                        sheet.name, attribute))

            attribute = CAttribute(
                attribute_name, attribute_type, reference_class)
            attribute.set_column_index(i)
            structure.add_attribute(attribute)

        root_class.add_structure(structure)
        root_class.add_enum(enum)
        namespace.add_class(root_class)
        cscode.set_namespace(namespace)

        self.cscodes.append(cscode)

        codewriter = ItemCodeWriter(cscode)
        codewriter.write_code()

    def finish(self):
        pass

    def generate_manager_code(self):
        cscode = CSCode(TABLE_MANAGER + ".cs")
        namespace = CNamespace(self.namespace)
        cclass = CClass(TABLE_MANAGER)
        for c in self.cscodes:
            class_name = c.namespace.classes[0].name
            attribute = CAttribute(class_name, class_name, None)
            cclass.add_attribute(attribute)

        cscode.set_namespace(namespace)
        namespace.add_class(cclass)
        codewriter = ManagerCodeWriter(cscode)
        codewriter.write_code()

    def get_cscode_by_name(self, class_name):
        for c in self.cscodes:
            if c.name.upper() == (class_name + ".cs").upper():
                return c
        return None

    def get_enum_values(self):
        enum_values = self.sheet.col_values(0)
        if(len(enum_values)) < 3:
            return None

        enum_values = enum_values[2:]
        for i in range(1, len(enum_values)):
            if type(enum_values[i]) != type(enum_values[i - 1]):
                return None

        str_values = []
        if type(enum_values[0]) is float:
            for i in range(0, len(enum_values)):
                str_values.append("E_" + str(int(enum_values[i])))
        elif type(enum_values[0]) is int:
            for i in range(0, len(enum_values)):
                str_values.append("E_" + str(enum_values[i]))
        elif type(enum_values[0]) is str:
            for i in range(0, len(enum_values)):
                str_values.append(enum_values[i])
        else:
            raise Exception("填写的枚举值不是有效的类型.")

        return str_values

    def get_attribute(self, index):
        types = self.sheet.row_values(1)
        item = types[index]
        strs = re.findall(r'(.*)<(.*)>', item)
        if len(strs) == 1 and len(strs[0]) == 2:
            data = strs[0]
            attribute_name = data[0]
            attribute_type = type_dic[data[1].upper()]
            return attribute_type, attribute_name
        else:
            raise Exception(
                "表 {} 存在无效的属性名或者属性类型.".format(self.sheet.name))
        pass

    def check_valid(self):
        if(self.sheet.nrows <= 1):
            return False

        row_types = self.sheet.row_values(1)
        for item in row_types:
            strs = re.findall(r'(.*)<(.*)>', item)

            if len(strs) == 1 and len(strs[0]) == 2:
                data = strs[0]
                type = type_dic[data[1].upper()]
                if type == None:
                    return False

        return True
