import xlrd
import os
import os.path
import codecs
import re
import argparse
import shutil

from struct import *
from Utils import *
from CodeGenerator import *
from DataGenerator import *
from shutil import copyfile

FLAGS = None


class CheckerClassImporter:
    def __init__(self, path):
        self.checker_path = path


class CSheet:
    def __init__(self, sheet, file_name):
        self.sheet = sheet
        self.file_name = file_name
        self.dependences = []
        self.is_sorted = False

    def add_dependence(self, csheet):
        self.dependences.append(csheet)


class TableConverter:
    def __init__(self, source_dir, data_checker_path):
        self.clear_temp_folder()
        self.source_dir = source_dir
        self.data_checker_path = data_checker_path
        self.files = []
        self.csheets = []
        self.code_generator = CodeGenerator(FLAGS.namespace)
        self.data_generator = DataGenerator(self.code_generator, FLAGS.out_data_file)
        self.data_checkers = []

    def convert(self):
        self.collect_data_checkers()
        self.collect_files()
        self.calculate_dependences()
        self.shorting_with_dependence()

        try:
            for csheet in self.csheets:
                print('处理表单：{}'.format(csheet.sheet.name))  # print info

                # ***************************** 标记当前位置 *******************************#
                ProgressInfo().sheet = csheet.sheet.name
                ProgressInfo().fileName = csheet.file_name
                # ***************************** 标记当前位置 *******************************#
                
                is_success = self.code_generator.generate_sheet_code(csheet.sheet)
                if is_success == False:
                    return False

                data_checker = self.get_data_checker(csheet.sheet.name)
                is_success = self.data_generator.generate_sheet_data(
                    csheet.sheet, data_checker)
                if is_success == False:
                    return False
        except Exception as e:
            print(ProgressInfo())
            print("*错误信息* -->", end=' ')
            print(e)
            return False

        self.code_generator.generate_manager_code()
        self.data_generator.finish()
        self.code_generator.finish()

        return True

    def clear_temp_folder(self):
        temp_folders = [TEMP_CODE_FOLDER_NAME, TEMP_DATA_FOLDER_NAME]
        for i in range(len(temp_folders)):
            temp_folder = temp_folders[i]
            try:
                shutil.rmtree(temp_folder)
            except:
                pass

    def copy_to_dest(self):
        pass

    def collect_data_checkers(self):
        print('收集错误检查脚本')  # print info
        loader = DataCheckLoader()
        self.data_checkers = loader.load(self.data_checker_path)

    def get_data_checker(self, name):
        for key, value in self.data_checkers.items():
            if key == name:
                return value
        return None

    def collect_files(self):
        print('收集Excel文件')  # print info
        # collect excel files
        for parent, dirnames, filenames in os.walk(self.source_dir):
            for filename in filenames:
                if filename.endswith(".xlsx") and not filename.startswith("~$"):
                    filepath = os.path.join(parent, filename)
                    self.files.append(filepath)

        # collect sheets
        sheetNames = []
        for fileName in self.files:
            filename = os.path.splitext(os.path.basename(fileName))[0]
            csheets = []
            workbook = xlrd.open_workbook(fileName)
            sheets = workbook.sheets()
            for sheet in sheets:
                if not sheet.name.startswith(SHEET):
                    if sheet.name not in sheetNames:
                        sheetNames.append(sheet.name)
                        csheet = CSheet(sheet, filename)
                        csheets.append(csheet)
                        self.csheets.append(csheet)
                    else:
                        raise Exception(r"两个表单名字相同： " + sheet.name)

    def calculate_dependences(self):
        print('检查Excel表单项的依赖关系')  # print info

        for csheet in self.csheets:
            row_values = csheet.sheet.row_values(1)
            for item in row_values:

                if item.upper().startswith(DES):
                    continue

                attribute_type, attribute_name, reference_name = Utils.get_attribute_info(
                    item)
                if attribute_name == None or attribute_type == None:
                    raise Exception("'{}' 无效的表单名字 '{}'".format(
                        csheet.sheet.name, item))
                if (attribute_type == ENUM or attribute_type == LIST_ENUM) and attribute_name.upper() != ID:
                    if reference_name == None:
                        raise Exception(
                            "'{0}' 表单项 '{1}' 没有引用".format(csheet.name, item))
                    dependence_csheet = self.get_csheet_with_name(
                        reference_name)
                    if dependence_csheet == None:
                        raise Exception("'{0}' '{1}' 没有找到目标类".format(
                            csheet.sheet.name, item))
                    csheet.add_dependence(dependence_csheet)

    def shorting_with_dependence(self):
        self.csheets = sorted(self.csheets, key=lambda x: len(x.dependences))

        # TODO
        isHaveMove = True
        roundNum = 0
        while isHaveMove:
            isHaveMove = False
            for i in range(0, len(self.csheets)):
                csheet = self.csheets[i]
                csheets_tomove = []
                for dependence in csheet.dependences:
                    dependence_index = self.csheets.index(dependence)
                    if dependence_index > i:
                        csheets_tomove.append(dependence)
                if len(csheets_tomove) > 0:
                    for csheet in csheets_tomove:
                        index = csheets_tomove.index(csheet)
                        self.csheets.remove(csheet)
                        self.csheets.insert(i, csheet)
                    isHaveMove = True

            roundNum += 1
            if roundNum > len(self.csheets):
                raise Exception("存在循环引用")

    def get_csheet_with_name(self, name):
        for csheet in self.csheets:
            if(csheet.sheet.name.upper() == name.upper()):
                return csheet
        return None


def main():
    table_converter = TableConverter(FLAGS.excel_path, FLAGS.checker_path)

    is_success = table_converter.convert()
    if is_success:
        if not FLAGS.dont_clear_folder:
            Utils.clear_folder_files(FLAGS.out_code_path)
            Utils.clear_folder_files(FLAGS.out_data_path)
        Utils.copy_files_to_folder(TEMP_CODE_FOLDER_NAME, FLAGS.out_code_path)
        Utils.copy_files_to_folder(TEMP_DATA_FOLDER_NAME, FLAGS.out_data_path)
        shutil.rmtree(TEMP_CODE_FOLDER_NAME)
        shutil.rmtree(TEMP_DATA_FOLDER_NAME)
        print('表格导出成功')
    else:
        print("表格导出*失败*")


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument(
        '--excel_path',
        type=str,
        help='Excel 文件的位置')

    parser.add_argument(
        '--out_code_path',
        type=str,
        help='输出代码的存放位置')

    parser.add_argument(
        '--out_data_path',
        type=str,
        help='输出数据文件的存放位置')

    parser.add_argument(
        '--checker_path',
        type=str,
        help='检查数据文件正确性的代码位置')

    parser.add_argument(
        '--namespace',
        type=str,
        help='输出代码的命名空间')

    parser.add_argument(
        '--out_data_file',
        type=str,
        default=DATA_FILE_NAME,
        help='输出数据文件的名字及后缀')

    parser.add_argument(
        '--dont_clear_folder',
        action="store_true",
        default=False,
        help='不删除目标目录')

    FLAGS, unparsed = parser.parse_known_args()
    main()