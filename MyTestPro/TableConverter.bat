ECHO OFF
set ProjectName=RetroSnake
"./Tools/Python/python.exe" ./Tools/TableConverter/TableConverter.py --excel_path ../design/Table^
                         --out_code_path ./Assets/RetroSnake/Scripts/Tables^
                         --out_data_path ./Assets/%ProjectName%/Resources/%ProjectName%/Tables^
                         --checker_path ./DataChecker^
						 --out_data_file tables.bytes^
						 --namespace %ProjectName%

pause