import re
import abc
import sys
import xlrd
import codecs
import struct
from Globals import *
from Utils import *
from collections import namedtuple


class BinaryFileHelper:
    def __init__(self, file_stream):
        self.file_stream = file_stream
        pass

    def write_byte(self, value):
        self.file_stream.write(struct.pack('c', value.encode()))

    def write_int(self, value):
        self.file_stream.write(struct.pack('<i', value))

    def write_float(self, value):
        self.file_stream.write(struct.pack('<f', value))

    def write_string(self, value):
        value = value.strip()
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            sbytes = value.encode(encoding='utf_8')
            length = len(sbytes)
            pattern = '<i{len}s'.format(len = length)
            self.file_stream.write(struct.pack(pattern, length, sbytes))

    def write_byte_list(self, value):
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            self.write_int(length)
            for c in value:
                self.write_byte(c)

    def write_int_list(self, value):
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            self.file_stream.write(struct.pack('<i%ui' % len(value), len(value), *value))

    def write_float_list(self, value):
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            self.file_stream.write(struct.pack('<i%uf' % len(value), len(value), *value))

    def write_string_list(self, value):
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            self.file_stream.write(struct.pack('<i', length))
            for str_value in value:
                self.write_string(str_value)

    def write_enum_list(self, value):
        length = len(value)
        if length == 0:
            self.file_stream.write(struct.pack('<i', 0))
        else:
            self.file_stream.write(struct.pack('<i', length))
            for enum_value in value:
                self.write_int(enum_value)


class DataGenerator:
    def __init__(self, code_generator, out_data_file):
        self.code_generator = code_generator
        file_path = TEMP_DATA_FOLDER_NAME + '/' + out_data_file
        Utils.create_folder(file_path)
        self.data_file_stream = open(file_path, "wb")
        self.info_file_stream = codecs.open(INFO_FILE_NAME, 'w', 'utf-8')
        self.binary_helper = BinaryFileHelper(self.data_file_stream)

    def generate_sheet_data(self, sheet, data_checker = None):
        cscode = self.code_generator.get_cscode_by_name(sheet.name)
        if cscode == None:
            raise Exception("表单{}没有找到对应的代码项".format(sheet.name))
            return False

        data_struct = cscode.namespace.classes[0].structures[0]

        start_row_index = ATTRIBUTE_ROW_INDEX + 1

        raw_data = []
        for i in range(start_row_index, sheet.nrows):
            data = sheet.row_values(i)
            raw_data.append(data)
        if data_checker:
            data_for_checker = self.get_data_for_checker(sheet.name, data_struct.attributes, raw_data)
            is_success = data_checker.Check(data_for_checker)
            if is_success == False:
                return False

        # write data
        self.binary_helper.write_int(len(range(start_row_index, sheet.nrows)))
        for i in range(len(raw_data)):

            # ***************************** 标记当前位置 *******************************#
            ProgressInfo().lineNum = i + ATTRIBUTE_ROW_INDEX + 2
            # ***************************** 标记当前位置 *******************************#
            
            single_piece_raw_data = raw_data[i]
            for j in range(len(data_struct.attributes)):
                attribute = data_struct.attributes[j]

                # ***************************** 标记当前位置 *******************************#
                ProgressInfo().attributeName = attribute.name
                # ***************************** 标记当前位置 *******************************#

                single_raw_data = single_piece_raw_data[attribute.column]
                single_data = self.get_attribute_data(attribute, single_raw_data)
                self.write_single_data(attribute, single_data)

        return True

    def get_data_for_checker(self, sheet_name,  attributes, raw_data):
        
        # define class
        attribute_names = []
        for i in range(len(attributes)):
            attribute_names.append(attributes[i].name)

        items = []
        item_class = ClassFactory(sheet_name, attribute_names)

        # create item
        for i in range(len(raw_data)):
            item = item_class()
            v = raw_data[i]
            for j in range(len(attributes)):
                attribute = attributes[j]
                rdata = v[j + 1]
                d = self.get_attribute_data(attribute, rdata)
                if isinstance(d, str):
                    d = "'{}'".format(d)
                exec("item.{} = {}".format(attribute.name, d))
                
            items.append(item)

        return items
        

    def get_attribute_data(self, attribute, rawdata):
            type = attribute.type
            value = rawdata

            if type == BOOL:
                if isinstance(value, float):
                    value = chr(int(value))

                if isinstance(value, str) == False:
                    raise Exception('数据{}无法转换成布尔类型'.format(value))

                return value

            elif type == INT:
                if isinstance(value, float):
                    value = int(value)

                if isinstance(value, int) == False:
                    value = int(value)
				
                if isinstance(value, int) == False:
                    raise Exception('数据{}无法转换成整型'.format(value))

                return value

            elif type == FLOAT:
                if isinstance(value, str) and value.upper() == "null".upper():
                    value = 0.0

                if isinstance(value, float) == False:
                    value = float(value)
					
                if isinstance(value, float) == False:
                    raise Exception('数据{}不能转成浮点类型'.format(value))

                value = value * 0.0001
                return value

            elif type == STRING:
                if isinstance(value, float):
                    value = str(int(value))

                if isinstance(value, float) and value == float(0) or (isinstance(value, str) and value.upper() == 'null'.upper()):
                    value = ''

                if isinstance(value, str) == False:
                    raise Exception('数据{}不能转成字符串类型'.format(value))

                return value

            elif type == ENUM:
                if not isinstance(value, str):
                    if isinstance(value, float):
                        value = str(int(value))
                    else:
                        raise Exception('数据{}不能转成枚举对应的整型'.format(value))

                str_enums = attribute.reference_class.namespace.classes[0].enums[0].values
                index = -1
                for i in range(0, len(str_enums)):
                    if value.upper() == str_enums[i].upper() or ("E_" + value).upper() == str_enums[i].upper():
                        index = i
                        break
                if index < 0:
                    raise Exception('数据{}找不到对应的枚举'.format(value))

                return index

            elif type == LIST_INT:
                int_list = []
                if isinstance(value, str):
                    str_list = value.split('|')
                    int_list = list(map(int, str_list))
                elif isinstance(value, float):
                    int_list.append(int(value))
                else:
                    raise Exception('数据 {0}{1} 不能转成整形数组'.format(value, type(value)[0]))

                return int_list

            elif type == LIST_FLOAT:
                float_list = []
                if isinstance(value, str):
                    str_list = value.split('|')
                    int_list = list(map(int, str_list))
                    float_list = [float(i) * 0.0001 for i in int_list]
                elif isinstance(value, float):
                    float_list = [value * 0.0001]
                else:
                    raise Exception('数据 {} 无法转成浮点序列'.format(value))

                return float_list

            elif type == LIST_STRING:
                if isinstance(value, str):
                    str_list = value.split('|')
                elif isinstance(value, float):
                    str_list = [str(value).split('.')[0]]
                else:
                    raise Exception('数据 {0}{1} 不能转成字符串数组'.format(value, type(value)[0]))
                
                return str_list
            
            elif type == LIST_ENUM:
                if isinstance(value, str):
                    str_list = value.split('|')
                elif isinstance(value, float):
                    str_list = [str(value).split('.')[0]]
                else:
                    raise Exception('数据 {0}{1} 不能转成字符串数组, 所以无法转成枚举列表'.format(value, type(value)[0]))

                enumList = []
                for enumStr in str_list:
                    str_enums = attribute.reference_class.namespace.classes[0].enums[0].values
                    index = -1
                    for i in range(0, len(str_enums)):
                        if enumStr.upper() == str_enums[i].upper() or ("E_" + enumStr).upper() == str_enums[i].upper():
                            index = i
                            break
                    if index < 0:
                        raise Exception('数据{}找不到对应的枚举'.format(enumStr))

                    enumList.append(index)

                return enumList

            else:
                raise Exception('数据 {} 类型不支持'.format(value))

    def write_single_data(self, attribute, data):
        if attribute.type == BOOL:
            self.binary_helper.write_byte(data)
        elif attribute.type == INT:
            self.binary_helper.write_int(data)
        elif attribute.type == FLOAT:
            self.binary_helper.write_float(data)
        elif attribute.type == STRING:
            self.binary_helper.write_string(data)
        elif attribute.type == ENUM:
            self.binary_helper.write_int(data)
        elif attribute.type == LIST_INT:
            self.binary_helper.write_int_list(data)
        elif attribute.type == LIST_FLOAT:
            self.binary_helper.write_float_list(data)
        elif attribute.type == LIST_STRING:
            self.binary_helper.write_string_list(data)
        elif attribute.type == LIST_ENUM:
            self.binary_helper.write_enum_list(data)
        else:
            raise Exception('数据格式不支持: {}'.format(attribute.type))

        return True


    def finish(self):
        self.info_file_stream.close()
        self.data_file_stream.close()
        pass