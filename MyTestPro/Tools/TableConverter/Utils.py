import xlrd
import os
import os.path
import codecs
import re
from Globals import *
import importlib
import shutil
import sys


class Utils:

    @staticmethod
    def get_class_name(sheet):
        return sheet.name

    @staticmethod
    def create_folder(file_path):
        directory = os.path.dirname(file_path)
        try:
            os.stat(directory)
        except:
            os.makedirs(directory)

    @staticmethod
    def clear_folder_files(folder_path):
        directory = folder_path
        try:
            os.stat(directory)
            file_list = os.listdir(directory)
            for file_name in file_list:
                os.remove(folder_path + '/' + file_name)
        except:
            os.makedirs(directory)

    @staticmethod
    def copy_files_to_folder(src_folder, dest_folder):
        file_list = os.listdir(src_folder)
        for file_name in file_list:
            shutil.copy2(src_folder + '/' + file_name, dest_folder)

    @staticmethod
    def get_attribute_info(text):
        attribute_name = None
        attribute_type = None
        reference_name = None

        strs = re.findall(r'(.*)<(.*)>', text)
        if len(strs) != 1 or len(strs[0]) != 2:
            return None, None, None

        data = strs[0]
        attribute_name = data[0]

        attribute_type_str = data[1]
        if attribute_type_str.upper().startswith("E_"):
            attribute_type = ENUM
            reference_name = attribute_type_str[2:]
        elif attribute_type_str.upper().startswith("LE_"):
            attribute_type = LIST_ENUM
            reference_name = attribute_type_str[3:]
        else:
            attribute_type = type_dic[attribute_type_str.upper()]
        return attribute_type, attribute_name, reference_name

    @staticmethod
    def get_enum_class_name(text):
        if text.upper().endswith("ID") and len(text) > 2:
            return text[:-2]
        return None

    @staticmethod
    def get_cscode_name(sheet):
        return sheet.name+".cs"


class DataChecker(object):
    def __init__(self):
        self.sheet_name = ''

    def Check(self, data):
        pass


class DataCheckLoader(object):
    def __init__(self):
        pass

    def load(self, path):

        abspath = os.path.abspath(path)
        parent_folder_path = os.path.dirname(abspath)
        if parent_folder_path not in sys.path:
            sys.path.insert(0, parent_folder_path)

        folder_name = os.path.basename(abspath)
        pysearchre = re.compile('\.py$', re.IGNORECASE)
        files = list(filter(pysearchre.search, os.listdir(abspath)))
        module_names = list(
            map(lambda fp: '.' + os.path.splitext(fp)[0], files))

        importlib.import_module(folder_name)
        modules = []
        for module_name in module_names:
            modules.append(importlib.import_module(
                module_name, package=folder_name))

        classes = {}
        for module in modules:
            temp = dict([name, cls] for name, cls in module.__dict__.items(
            ) if isinstance(cls, type(DataChecker)) and name != 'DataChecker')
            classes = {**classes, **temp}

        class_instances = dict()
        for name, cls in classes.items():
            temp_class = cls()
            class_instances[temp_class.sheet_name] = temp_class

        return class_instances


class BaseClass(object):
    def __init__(self, classtype):
        self._type = classtype


def ClassFactory(name, argnames, BaseClass=BaseClass):
    def __init__(self, **kwargs):
        for key, value in kwargs.items():
            if key not in argnames:
                raise TypeError("Argument %s not valid for %s"
                                % (key, self.__class__.__name__))
            setattr(self, key, value)
        BaseClass.__init__(self, name)
    newclass = type(name, (BaseClass,), {"__init__": __init__})
    return newclass


class ProgressInfo(object):
    __instance = None
    __first_init = False

    def __new__(cls):
        if not cls.__instance:
            cls.__instance = object.__new__(cls)
        return cls.__instance

    def __init__(self):
        if not self.__first_init:
            self.fileName = ""
            self.sheet = ""
            self.attributeName = ""
            self.lineNum = 0
            ProgressInfo.__first_init = True

    def __str__(self):
        return "*错误* --> 文件名: " + self.fileName + " 表名: " + self.sheet + " 属性名: " + self.attributeName + " 所在行: " + str(self.lineNum)
    